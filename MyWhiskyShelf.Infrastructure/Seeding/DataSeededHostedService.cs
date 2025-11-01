using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Interfaces;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;
using MyWhiskyShelf.Infrastructure.Persistence.Mapping;

namespace MyWhiskyShelf.Infrastructure.Seeding;

// This is used when seeding data in a development environment only and will be removed at a later time.
[ExcludeFromCodeCoverage]
public sealed class DataSeederHostedService(
    ILogger<DataSeederHostedService> logger,
    IJsonFileLoader loader,
    IConfiguration configuration,
    IServiceScopeFactory scopeFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken ct)
    {
        if (!configuration.GetValue("MYWHISKYSHELF_DATA_SEEDING_ENABLED", false)) return;
        
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MyWhiskyShelfDbContext>();
        await SeedGeoDataAsync(dbContext, "Resources/geoData.json", ct);
        //await SeedBrandsAsync(dbContext, "Resources/brands.json", ct);
        await SeedDistilleriesAsync(dbContext, "Resources/distilleries.json", ct);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task SeedGeoDataAsync(MyWhiskyShelfDbContext dbContext, string filepath, CancellationToken ct)
    {
        var geoData = await loader.FetchFromJsonAsync<Country>(filepath, ct);
       
        dbContext.AddRange(geoData.Select(gd => gd.ToEntity()));
        await dbContext.SaveChangesAsync(ct);
        
        logger.LogInformation(
            "Seeded {CountryCount} countries and {RegionCount} regions.",
            geoData.Count,
            geoData.Sum(gd => gd.Regions.Count));
    }
    
    private async Task SeedDistilleriesAsync(MyWhiskyShelfDbContext dbContext, string filepath, CancellationToken ct)
    {
        var distilleries = await loader.FetchFromJsonAsync<Distillery>(filepath, ct);
        
        dbContext.Distilleries.AddRange(distilleries.Select(d => d.ToEntity()));
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Seeded {Count} distilleries.", distilleries.Count);
    } 
}