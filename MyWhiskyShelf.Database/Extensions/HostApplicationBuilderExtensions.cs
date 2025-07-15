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

        builder.Services.AddSingleton<IMapper<Distillery, DistilleryEntity>, DistilleryMapper>();
        builder.Services.AddSingleton<IMapper<WhiskyBottle, WhiskyBottleEntity>, WhiskyBottleMapper>();
        builder.Services.AddSingleton<IDistilleryNameCacheService, DistilleryNameCacheService>();
        builder.Services.AddScoped<IDistilleryReadService, DistilleryReadService>();
        builder.Services.AddScoped<IDistilleryWriteService, DistilleryWriteService>();
        builder.Services.AddScoped<IWhiskyBottleWriteService, WhiskyBottleWriteService>();
    }
}