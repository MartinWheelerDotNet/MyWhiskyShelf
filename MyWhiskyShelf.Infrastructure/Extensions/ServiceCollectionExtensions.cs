using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Infrastructure.Persistence.Repositories;

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
}