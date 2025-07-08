using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MyWhiskyShelf.DataLoader.Extensions;

[ExcludeFromCodeCoverage]
public static class HostApplicationBuilderExtensions
{
    public static void UseDataLoader(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IJsonFileLoader, JsonFileLoader>();
    }
}