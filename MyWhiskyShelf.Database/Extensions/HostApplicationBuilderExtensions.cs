using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.Database.Mappers;
using MyWhiskyShelf.Database.Services;

namespace MyWhiskyShelf.Database.Extensions;

[ExcludeFromCodeCoverage]
public static class HostApplicationBuilderExtensions
{
    public static void UsePostgresDatabase(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<MyWhiskyShelfDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("postgresDb")
                              ?? throw new InvalidOperationException("Connection string not found")));

        builder.EnrichNpgsqlDbContext<MyWhiskyShelfDbContext>(settings =>
        {
            settings.DisableRetry = false;
            settings.CommandTimeout = 30;
            settings.DisableHealthChecks = false;
            settings.DisableMetrics = false;
            settings.DisableTracing = false;
        });

        builder.AddMappers();
        builder.AddServices();
    }

    private static void AddServices(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddSingleton<IDistilleryNameCacheService, DistilleryNameCacheService>();
        builder.Services
            .AddScoped<IDistilleryReadService, DistilleryReadService>()
            .AddScoped<IDistilleryWriteService, DistilleryWriteService>();
        builder.Services
            .AddScoped<IWhiskyBottleWriteService, WhiskyBottleWriteService>()
            .AddScoped<IWhiskyBottleReadService, WhiskyBottleReadService>();
    }

    private static void AddMappers(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddSingleton<IMapper<DistilleryEntity, DistilleryResponse>, DistilleryEntityToResponseMapper>()
            .AddSingleton<IMapper<CreateDistilleryRequest, DistilleryEntity>, DistilleryRequestToEntityMapper>();
        builder.Services
            .AddSingleton<IMapper<WhiskyBottleEntity, WhiskyBottleResponse>, WhiskyBottleEntityToResponseMapper>()
            .AddSingleton<IMapper<WhiskyBottleRequest, WhiskyBottleEntity>, WhiskyBottleRequestToEntityMapper>();
    }
}