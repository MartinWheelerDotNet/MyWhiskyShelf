using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using MyWhiskyShelf.Application.Abstractions.Cursor;
using MyWhiskyShelf.Application.Abstractions.Services;
using MyWhiskyShelf.Application.Codecs;
using MyWhiskyShelf.Application.Services;

namespace MyWhiskyShelf.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDistilleryAppService, DistilleryAppService>();
        services.AddScoped<IWhiskyBottleAppService, WhiskyBottleAppService>();
        services.AddScoped<IGeoAppService, GeoAppService>();
        services.AddScoped<IBrandAppService, BrandAppService>();
        services.AddSingleton<ICursorCodec, Base64JsonCursorCodec>();
    }
}