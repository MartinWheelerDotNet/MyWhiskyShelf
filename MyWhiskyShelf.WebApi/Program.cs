using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Extensions;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.DataLoader;
using MyWhiskyShelf.DataLoader.Extensions;
using MyWhiskyShelf.ServiceDefaults;
using MyWhiskyShelf.WebApi.Endpoints;
using MyWhiskyShelf.WebApi.Interfaces;
using MyWhiskyShelf.WebApi.Services;
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
        builder.AddRedisClient("cache");
        
        
        // If this project is being used as part of an Aspire Environment, the environment variable
        // MYWHISKYSHELF_DATA_SEEDING_ENABLED is configured in <MyWhiskyShelf.AppHost>.
        // This should be forwarded to this project to be used here. Otherwise, the standard configuration resources are
        // used provide this value.
        var useDataSeeding = builder.Configuration.GetValue("MYWHISKYSHELF_DATA_SEEDING_ENABLED", false);

        if (builder.Environment.IsDevelopment())
        {
            builder.UseDataLoader();
            builder.Services.AddOpenApi();
        }

        var app = builder.Build();

        app.MapDefaultEndpoints();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            await EnsureDatabaseCreated(app.Services, useDataSeeding);
        }

        app.UseHttpsRedirection();

        app.MapDistilleryEndpoints();
        app.MapWhiskyBottleEndpoints();
        app.MapDistilleryNameEndpoints();

        await app.RunAsync();
    }

    private static WebApplicationBuilder ConfigureDefaultServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        builder.Services.AddSingleton<IIdempotencyService, RedisIdempotencyService>();
        builder.AddServiceDefaults();

        return builder;
    }

    private static async Task EnsureDatabaseCreated(IServiceProvider serviceProvider, bool useDataSeeding)
    {
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<MyWhiskyShelfDbContext>();
        var dataLoader = scope.ServiceProvider
            .GetRequiredService<IJsonFileLoader>();
        var mapper = scope.ServiceProvider
            .GetRequiredService<IMapper<DistilleryRequest, DistilleryEntity>>();

        await dbContext.Database.EnsureCreatedAsync();

        if (await dbContext.Set<DistilleryEntity>().AnyAsync())
            dbContext.Set<DistilleryEntity>().RemoveRange(dbContext.Set<DistilleryEntity>());

        if (useDataSeeding)
        {
            var distilleries = await dataLoader.GetDistilleriesFromJsonAsync("Resources/distilleries.json");
            var mappedDistilleries = distilleries.Select(mapper.Map);

            dbContext.Set<DistilleryEntity>().AddRange(mappedDistilleries);
        }

        await dbContext.SaveChangesAsync();

        var cacheService = scope.ServiceProvider.GetRequiredService<IDistilleryNameCacheService>();
        await cacheService.InitializeFromDatabaseAsync(dbContext);
    }
}