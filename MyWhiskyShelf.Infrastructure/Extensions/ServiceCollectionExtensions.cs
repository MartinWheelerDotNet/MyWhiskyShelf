using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Infrastructure.Interfaces;
using MyWhiskyShelf.Infrastructure.Persistence.Repositories;
using MyWhiskyShelf.Infrastructure.Seeding;

namespace MyWhiskyShelf.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void AddInfrastructureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IDistilleryReadRepository, DistilleryReadRepository>();
        services.AddScoped<IDistilleryWriteRepository, DistilleryWriteRepository>();
        services.AddScoped<IWhiskyBottleReadRepository, WhiskyBottleReadRepository>();
        services.AddScoped<IWhiskyBottleWriteRepository, WhiskyBottleWriteRepository>();
    }

    public static void AddOptionalDataSeeding(this IServiceCollection services)
    {
        services.AddHostedService<DataSeederHostedService>();
        services.AddSingleton<IJsonFileLoader, JsonFileLoader>();
    }
}