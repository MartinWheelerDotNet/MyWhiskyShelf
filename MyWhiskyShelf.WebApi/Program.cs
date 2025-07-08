using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Extensions;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.Database.Services;
using MyWhiskyShelf.DataLoader;
using MyWhiskyShelf.DataLoader.Extensions;
using MyWhiskyShelf.ServiceDefaults;

namespace MyWhiskyShelf.WebApi;

[ExcludeFromCodeCoverage]
internal static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication
            .CreateBuilder(args)
            .ConfigureDefaultServices();
        builder.UsePostgresDatabase();

        // If this project is being used as part of an Aspire Environment, the environment variable
        // MYWHISKYSHELF_DATA_SEEDING_ENABLED is configured in <MyWhiskyShelf.AppHost>.
        // This should be forwarded to this project to be used here. Otherwise, the standard configuration resources are
        // used provide this value.
        var useDataSeeding = builder.Configuration.GetValue("MYWHISKYSHELF_DATA_SEEDING_ENABLED", false);

        if (builder.Environment.IsDevelopment())
        {
            builder.UseDataLoader();
        }

        var app = builder.Build();

        app.MapDefaultEndpoints();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            await EnsureDatabaseCreated(app.Services, useDataSeeding);
        }
        
        app.UseHttpsRedirection();

        app.MapGet(
            "/distilleries/{distilleryName}",
            async (IDistilleryReadService distilleryReadService, string distilleryName) =>
            {
                var distillery = await distilleryReadService.GetDistilleryByNameAsync(distilleryName);
                
                return distillery is null 
                    ? Results.NotFound()
                    : Results.Ok(distillery);
            });
        
        app.MapGet(
            "/distilleries", 
            async (IDistilleryReadService distilleryReadService) 
                => await distilleryReadService.GetAllDistilleriesAsync());
        
        app.MapGet(
            "/distilleries/names",
            (IDistilleryReadService distilleryReadService) 
                => distilleryReadService.GetDistilleryNames());

        app.MapPost("/distilleries/add",
            async (IDistilleryWriteService distilleryWriteService, Distillery distillery) =>
            {
                if (await distilleryWriteService.TryAddDistilleryAsync(distillery))
                    return Results.Created($"/distilleries/{distillery.DistilleryName}", distillery);
               
                return Results.Conflict("Distillery could not be added, it may already exist.");
            });
        
        await app.RunAsync();
    }
    
    private static WebApplicationBuilder ConfigureDefaultServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
        builder.Services.AddOpenApi();
        builder.AddServiceDefaults();

        return builder;
    }

    private static async Task EnsureDatabaseCreated(IServiceProvider serviceProvider, bool useDataSeeding)
    {
        using var scope = serviceProvider.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<MyWhiskyShelfDbContext>();
        var dataLoader = scope.ServiceProvider.GetRequiredService<IJsonFileLoader>();
        var mapper = scope.ServiceProvider.GetRequiredService<IDistilleryMapper>();
        
        await dbContext.Database.EnsureCreatedAsync();
    
        if (await dbContext.Set<DistilleryEntity>().AnyAsync()) return;

        if (useDataSeeding)
        {
            var distilleries = await dataLoader.GetDistilleriesFromJsonAsync("Resources/distilleries.json");
            var mappedDistilleries = distilleries.Select(distillery => mapper.MapToEntity(distillery));
        
            dbContext.Set<DistilleryEntity>().AddRange(mappedDistilleries);
            await dbContext.SaveChangesAsync();    
        }
        
        var cacheService = scope.ServiceProvider.GetRequiredService<IDistilleryNameCacheService>();
        await cacheService.LoadCacheFromDbAsync(dbContext);
    }
}