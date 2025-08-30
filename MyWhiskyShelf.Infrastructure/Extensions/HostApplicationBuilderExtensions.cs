using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyWhiskyShelf.Infrastructure.Interfaces;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;
using MyWhiskyShelf.Infrastructure.Seeding;

namespace MyWhiskyShelf.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class HostApplicationBuilderExtensions
{
    public static void UsePostgresDatabase(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<MyWhiskyShelfDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("myWhiskyShelfDb")
                              ?? throw new InvalidOperationException("Connection string not found")));

        builder.EnrichNpgsqlDbContext<MyWhiskyShelfDbContext>(settings =>
        {
            settings.DisableRetry = false;
            settings.CommandTimeout = 30;
            settings.DisableHealthChecks = false;
            settings.DisableMetrics = false;
            settings.DisableTracing = false;
        });
    }

    public static void UseDataLoader(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IJsonFileLoader, JsonFileLoader>();
    }
}