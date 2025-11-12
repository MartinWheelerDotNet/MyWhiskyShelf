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
    extension(IServiceCollection services)
    {
        public void AddInfrastructureRepositories()
        {
            services.AddScoped<IDistilleryReadRepository, DistilleryReadRepository>();
            services.AddScoped<IDistilleryWriteRepository, DistilleryWriteRepository>();
            services.AddScoped<IWhiskyBottleReadRepository, WhiskyBottleReadRepository>();
            services.AddScoped<IWhiskyBottleWriteRepository, WhiskyBottleWriteRepository>();
            services.AddScoped<IGeoReadRepository, GeoReadRepository>();
            services.AddScoped<IGeoWriteRepository, GeoWriteRepository>();
            services.AddScoped<IBrandReadRepository, BrandReadRepository>();
        }

        public void AddOptionalDataSeeding()
        {
            services.AddHostedService<DataSeederHostedService>();
            services.AddSingleton<IJsonFileLoader, JsonFileLoader>();
        }
    }
}