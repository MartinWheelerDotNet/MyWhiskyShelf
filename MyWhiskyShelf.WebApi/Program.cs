using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Extensions;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.DataLoader;
using MyWhiskyShelf.DataLoader.Extensions;
using MyWhiskyShelf.ServiceDefaults;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

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
            async (IDistilleryReadService distilleryReadService) =>
            {
                var distilleries = await distilleryReadService.GetAllDistilleriesAsync();
                return Results.Ok(distilleries);
            });

        app.MapGet(
            "/distilleries/names",
            (IDistilleryReadService distilleryReadService) =>
            {
                var distilleryNames = distilleryReadService.GetDistilleryNames();
                return Results.Ok(distilleryNames);
            });
        
        app.MapGet(
            "/distilleries/name/search",
            (IDistilleryReadService distilleryReadService, string? pattern, HttpContext httpContext) =>
            {
                if (string.IsNullOrWhiteSpace(pattern))
                    return Results.Problem(
                        new ProblemDetails
                        {
                            Type = "urn:mywhiskyshelf:errors:missing-or-invalid-query-pattern",
                            Title = "Missing or invalid query parameter",
                            Status = StatusCodes.Status400BadRequest,
                            Detail = "Query parameter 'pattern' is required and cannot be empty.",
                            Instance = httpContext.Request.Path
                        });
                
                var names = distilleryReadService.SearchByName(pattern);
                
                return Results.Ok(names);
            }
        );

        app.MapPost(
            "/distilleries/add",
            async (IDistilleryWriteService distilleryWriteService, Distillery distillery, HttpContext httpContext) =>
            {
                if (await distilleryWriteService.TryAddDistilleryAsync(distillery))
                    return Results.Created(
                        $"/distilleries/{Uri.EscapeDataString(distillery.DistilleryName)}",
                        distillery);

                return Results.Problem(
                    new ProblemDetails
                    {
                        Type = "urn:mywhiskyshelf:errors:distillery-already-exists",
                        Title = "Distillery already exists.", 
                        Status = StatusCodes.Status409Conflict,
                        Detail = $"Cannot add distillery '{distillery.DistilleryName} as it already exists.",
                        Instance = httpContext.Request.Path
                    });
            });

        app.MapDelete(
            "/distilleries/remove/{distilleryName}",
            async (IDistilleryWriteService distilleryWriteService, string distilleryName, HttpContext httpContext) =>
            {
                if (await distilleryWriteService.TryRemoveDistilleryAsync(Uri.UnescapeDataString(distilleryName)))
                    return Results.Ok(); 
                
                return Results.Problem(
                    new ProblemDetails
                    {
                        Type = "urn:mywhiskyshelf:errors:distillery-does-not-exist",
                        Title = "Distillery does not exist.", 
                        Status = StatusCodes.Status404NotFound,
                        Detail = $"Cannot remove distillery '{distilleryName}' as it does not exist.",
                        Instance = httpContext.Request.Path
                    });
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
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper<Distillery, DistilleryEntity>>();
        
        await dbContext.Database.EnsureCreatedAsync();

        if (await dbContext.Set<DistilleryEntity>().AnyAsync())
        {
            dbContext.Set<DistilleryEntity>().RemoveRange(dbContext.Set<DistilleryEntity>());
        }

        if (useDataSeeding)
        {
            var distilleries = await dataLoader.GetDistilleriesFromJsonAsync("Resources/distilleries.json");
            var mappedDistilleries = distilleries.Select(distillery => mapper.MapToEntity(distillery));
        
            dbContext.Set<DistilleryEntity>().AddRange(mappedDistilleries);
            await dbContext.SaveChangesAsync();    
        }
        
        var cacheService = scope.ServiceProvider.GetRequiredService<IDistilleryNameCacheService>();
        await cacheService.InitializeFromDatabaseAsync(dbContext);
    }
}