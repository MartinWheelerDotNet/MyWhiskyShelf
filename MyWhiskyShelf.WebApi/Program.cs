using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using MyWhiskyShelf.Application.Extensions;
using MyWhiskyShelf.Infrastructure.Extensions;
using MyWhiskyShelf.ServiceDefaults;
using MyWhiskyShelf.WebApi.Endpoints;
using MyWhiskyShelf.WebApi.Extensions;
using MyWhiskyShelf.WebApi.Interfaces;
using MyWhiskyShelf.WebApi.Services;


namespace MyWhiskyShelf.WebApi;

[ExcludeFromCodeCoverage]
internal static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).ConfigureDefaultServices();
        builder.UsePostgresDatabase();
        builder.AddRedisClient("cache");
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureRepositories();
        builder.Services.AddOptionalDataSeeding();

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("UiCors", policy =>
                {
                    policy
                        .WithOrigins(
                            "http://localhost:5173",
                            "https://localhost:5173",
                            "http://127.0.0.1:5173",
                            "https://127.0.0.1:5173"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        builder.Services.AddOpenApi();

        builder.SetupAuthorization();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseCors("UiCors");
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapDistilleryEndpoints();
        app.MapWhiskyBottleEndpoints();
        app.MapGeoEndpoints();

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
}