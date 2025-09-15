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
        // If this project is being used as part of an Aspire Environment, the connection string is created by Aspire
        // and forwarded to this project to be used here.
        // Otherwise, the standard configuration resources are used provide this value.
        // If this is not found an exception should be thrown.
        var connectionString = builder.Configuration.GetConnectionString("myWhiskyShelfDb")
                               ?? throw new InvalidOperationException("Connection string not found");
        builder.Services.AddDbContext<MyWhiskyShelfDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly("MyWhiskyShelf.Migrations")));

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