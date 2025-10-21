using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyWhiskyShelf.Infrastructure.Interfaces;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;
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
    private static Guid JapanId => Guid.Parse("f5712c32-1d89-444d-8cf7-9c444c55ce61");
    private static Guid ScotlandId => Guid.Parse("e1a604b5-8aa5-4eac-89f0-64f37a3a3290");

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var enabled = configuration.GetValue("MYWHISKYSHELF_DATA_SEEDING_ENABLED", false);

        if (!enabled || !environment.IsDevelopment()) return;

        using var scope = scopeFactory.CreateScope();
        var distilleries = await loader.GetDistilleriesFromJsonAsync("Resources/distilleries.json", cancellationToken);
        var dbContext = scope.ServiceProvider.GetRequiredService<MyWhiskyShelfDbContext>();
        
        await SeedGeoDataAsync(dbContext);
        
        var entities = distilleries.Select(d => d.ToEntity()).ToList();
        dbContext.Distilleries.AddRange(entities);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded {Count} distilleries.", entities.Count);

        
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static async Task SeedGeoDataAsync(MyWhiskyShelfDbContext dbContext)
    {
        var scotland = new CountryEntity
        {
            Id = ScotlandId,
            Name = "Scotland",
            Slug = "scotland",
            IsActive = true,
            Regions =
            [
                new RegionEntity
                {
                    Id = Guid.Parse("b4d04c7b-ba24-4918-84fa-d7c84f0137d0"),
                    Name = "Campbeltown",
                    Slug = "campbeltown",
                    IsActive = true,
                    CountryId = ScotlandId
                },
                new RegionEntity
                {
                    Id = Guid.Parse("1e454b6b-9c36-46e1-8ceb-02c8e1b10670"),
                    Name = "Highlands",
                    Slug = "highlands",
                    IsActive = true,
                    CountryId = ScotlandId
                },
                new RegionEntity
                {
                    Id = Guid.Parse("60c9d172-9385-4aa6-a908-88696842b74c"),
                    Name = "Islay",
                    Slug = "islay",
                    IsActive = true,
                    CountryId = ScotlandId
                },
                new RegionEntity
                {
                    Id = Guid.Parse("b1344459-e9f2-4517-9200-83933b3892ff"),
                    Name = "Lowland",
                    Slug = "lowland",
                    IsActive = true,
                    CountryId = ScotlandId
                },
                new RegionEntity
                {
                    Id = Guid.Parse("c6b3dd09-35af-4e00-9fbe-8c68cad061fd"),
                    Name = "Speyside",
                    Slug = "speyside",
                    IsActive = true,
                    CountryId = ScotlandId
                },
                new RegionEntity
                {
                    Id = Guid.Parse("53fd7179-aed8-453e-99f9-3bf14b633770"),
                    Name = "Islands",
                    Slug = "island",
                    IsActive = true,
                    CountryId = ScotlandId
                }
            ]
        };
        var japan = new CountryEntity
        {
            Id = JapanId,
            Name = "Japan",
            Slug = "japan",
            IsActive = true,
            Regions =
            [
                new RegionEntity
                {
                    Id = Guid.Parse("cad054e9-5d20-4c42-b85f-3ec4b9f14086"),
                    Name = "Hokkaido",
                    Slug = "hokkaido",
                    IsActive = true,
                    CountryId = JapanId
                },
                new RegionEntity
                {
                    Id = Guid.Parse("817a7f15-7017-46c9-9b82-4ac84c41e4e5"),
                    Name = "Tohoku",
                    Slug = "tohoku",
                    IsActive = true,
                    CountryId = JapanId
                },
                new RegionEntity
                {
                    Id = Guid.Parse("dc1d72a4-2828-4484-b145-7899a41af23a"),
                    Name = "Kanto",
                    Slug = "kanto",
                    IsActive = true,
                    CountryId = JapanId
                },
                new RegionEntity
                {
                    Id = Guid.Parse("0099b8bc-0e49-4fc8-a994-483f900f2d82"),
                    Name = "Chubu",
                    Slug = "chubu",
                    IsActive = true,
                    CountryId = JapanId
                },
                new RegionEntity
                {
                    Id = Guid.Parse("dd4fea04-b027-48b4-9a81-6e7eab321456"),
                    Name = "Kansai",
                    Slug = "kansai",
                    IsActive = true,
                    CountryId = JapanId
                },
                new RegionEntity
                {
                    Id = Guid.Parse("f352910e-3f26-42f8-8aea-50bee0b54331"),
                    Name = "Setouchi",
                    Slug = "setouchi",
                    IsActive = true,
                    CountryId = JapanId
                },
                new RegionEntity
                {
                    Id = Guid.Parse("9cdddda7-a138-430d-bc51-93db10a70e56"),
                    Name = "Kyushu-Okinawa",
                    Slug = "kyushu-okinawa",
                    IsActive = true,
                    CountryId = JapanId
                }
            ]
        };

        dbContext.AddRange(scotland, japan);
        await dbContext.SaveChangesAsync();
    }
}