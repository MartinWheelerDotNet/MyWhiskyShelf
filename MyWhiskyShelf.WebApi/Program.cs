using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using MyWhiskyShelf.Application.Extensions;
using MyWhiskyShelf.Infrastructure.Extensions;
using MyWhiskyShelf.Infrastructure.Interfaces;
using MyWhiskyShelf.Infrastructure.Mapping;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;
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

        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureRepositories();

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
            if (useDataSeeding) await SeedData(app.Services);
        }

        app.UseHttpsRedirection();

        app.MapDistilleryEndpoints();
        app.MapWhiskyBottleEndpoints();

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

    private static async Task SeedData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<MyWhiskyShelfDbContext>();
        var dataLoader = scope.ServiceProvider
            .GetRequiredService<IJsonFileLoader>();

        var distilleries = await dataLoader.GetDistilleriesFromJsonAsync("Resources/distilleries.json");
        var mappedDistilleries = distilleries.Select(distillery => distillery.ToEntity());

        await dbContext.Set<DistilleryEntity>().AddRangeAsync(mappedDistilleries);


        await dbContext.SaveChangesAsync();
    }
}