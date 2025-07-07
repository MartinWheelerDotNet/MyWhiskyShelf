using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Extensions;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.Database.Mappers;
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

        if (builder.Environment.IsDevelopment())
        {
            builder.UseDataLoader();
        }
        

        var app = builder.Build();
        
        app.MapDefaultEndpoints();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            await EnsureDatabaseCreated(app.Services);
        }

        app.UseHttpsRedirection();

        app.MapGet("/distilleries/", async (IDistilleryReadService distilleryReadService) =>
            await distilleryReadService.GetAllDistilleriesAsync());

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

    private static async Task EnsureDatabaseCreated(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<MyWhiskyShelfDbContext>();
        var dataLoader = scope.ServiceProvider.GetRequiredService<IJsonFileLoader>();
        var mapper = scope.ServiceProvider.GetRequiredService<IDistilleryMapper>();
        
        await dbContext.Database.EnsureCreatedAsync();

        if (await dbContext.Set<DistilleryEntity>().AnyAsync()) return;
        
        var distilleries = await dataLoader.GetDistilleriesFromJsonAsync("Resources/distilleries.json");
        var mappedDistilleries = distilleries.Select(distillery => mapper.MapToEntity(distillery));
        
        dbContext.Set<DistilleryEntity>().AddRange(mappedDistilleries);
        await dbContext.SaveChangesAsync();

        var cacheService = scope.ServiceProvider.GetRequiredService<IDistilleryNameCacheService>();
        await cacheService.LoadCacheFromDbAsync(dbContext);
    }
}