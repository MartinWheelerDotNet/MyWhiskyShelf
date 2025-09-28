using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyWhiskyShelf.Infrastructure.Interfaces;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;
using MyWhiskyShelf.Infrastructure.Persistence.Mapping;

namespace MyWhiskyShelf.Infrastructure.Seeding;

// This is used when seeding data in a development environment only and will be removed at a later time.
[ExcludeFromCodeCoverage]
public sealed class DataSeederHostedService(
    ILogger<DataSeederHostedService> logger,
    IJsonFileLoader loader,
    IHostEnvironment environment,
    IConfiguration configuration,
    IServiceScopeFactory scopeFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var enabled = configuration.GetValue("MYWHISKYSHELF_DATA_SEEDING_ENABLED", false);

        if (!enabled || !environment.IsDevelopment()) return;

        using var scope = scopeFactory.CreateScope();
        var distilleries = await loader.GetDistilleriesFromJsonAsync("Resources/distilleries.json", cancellationToken);
        var dbContext = scope.ServiceProvider.GetRequiredService<MyWhiskyShelfDbContext>();
        var entities = distilleries.Select(d => d.ToEntity()).ToList();
        dbContext.Distilleries.AddRange(entities);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded {Count} distilleries.", entities.Count);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

    
