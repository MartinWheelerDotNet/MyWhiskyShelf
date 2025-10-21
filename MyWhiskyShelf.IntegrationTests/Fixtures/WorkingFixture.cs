using Aspire.Hosting;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;
using MyWhiskyShelf.Infrastructure.Persistence.Mapping;
using MyWhiskyShelf.IntegrationTests.TestData;
using MyWhiskyShelf.WebApi.Contracts.Distilleries;
using MyWhiskyShelf.WebApi.Contracts.GeoResponse;
using MyWhiskyShelf.WebApi.Contracts.WhiskyBottles;
using MyWhiskyShelf.WebApi.Mapping;
using Npgsql;

namespace MyWhiskyShelf.IntegrationTests.Fixtures;

[UsedImplicitly]
public class WorkingFixture : IAsyncLifetime
{
    public readonly Guid FirstSeededCountryId = Guid.Parse("caafacf4-c8f8-4b72-bfb7-226deafbfdd6");
    public readonly Guid SecondSeededCountryId = Guid.Parse("2cc817db-03e4-4221-ae63-ef7505ab400e");
    public readonly Guid FirstRegionFirstCountryId = Guid.NewGuid();
    public readonly Guid SecondRegionFirstCountryId = Guid.NewGuid();
    public readonly Guid FirstRegionSecondCountryId = Guid.NewGuid();
    
    public enum EntityType
    {
        Distillery,
        WhiskyBottle,
        Country,
        Region
    }

    public DistributedApplication Application { get; private set; } = null!;
    private MyWhiskyShelfDbContext DbContext { get; set; } = null!;

    public virtual async Task InitializeAsync()
    {
        Application = await FixtureFactory.StartAsync(FixtureFactory.DefaultTestingArguments);
        var connectionString = await Application.GetConnectionStringAsync("myWhiskyShelfDb");
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.UseVector();
        var dataSource = dataSourceBuilder.Build();

        var options = new DbContextOptionsBuilder<MyWhiskyShelfDbContext>()
            .UseNpgsql(dataSource, o => o.UseVector())
            .EnableSensitiveDataLogging()
            .Options;

        DbContext = new MyWhiskyShelfDbContext(options);
    }

    public virtual async Task DisposeAsync()
    {
        try
        {
            await ClearDatabaseAsync();
            await Application.StopAsync();
        }
        finally
        {
            await Application.DisposeAsync();
        }
    }

    public async Task<List<DistilleryResponse>> SeedDistilleriesAsync(List<DistilleryEntity> entities)
    {
        await SetupCountriesForTests();
        
        DbContext.AddRange(entities);
        await DbContext.SaveChangesAsync();

        return entities.Select(e => e.ToDomain().ToResponse())
            .OrderBy(response => response.Name)
            .ThenBy(response => response.Id)
            .ToList();
    }

    public async Task<List<DistilleryResponse>> SeedDistilleriesAsync(int count)
    {
        var entities = Enumerable.Range(0, count)
            .Select(i => DistilleryEntityTestData.Generic(
                $"Distillery {i}",
                FirstSeededCountryId,
                FirstRegionFirstCountryId))
            .ToList();

        return await SeedDistilleriesAsync(entities);
    }

    public async Task<List<WhiskyBottleResponse>> SeedWhiskyBottlesAsync(List<WhiskyBottleEntity> entities)
    {
        DbContext.AddRange(entities);
        await DbContext.SaveChangesAsync();

        return entities.Select(e => e.ToDomain().ToResponse())
            .OrderBy(response => response.Name)
            .ThenBy(response => response.Id)
            .ToList();
    }

    public async Task<List<CountryResponse>> SeedCountriesAsync(List<CountryEntity> entities)
    {
        DbContext.AddRange(entities);
        await DbContext.SaveChangesAsync();

        return entities.Select(e => e.ToDomain().ToResponse())
            .OrderBy(response => response.Name)
            .ThenBy(response => response.Id)
            .ToList();
    }

    public async Task<List<RegionResponse>> SeedRegionsAsync(List<RegionEntity> entities)
    {
        await SetupCountriesForTests();
        
        DbContext.AddRange(entities);
        await DbContext.SaveChangesAsync();

        return entities.Select(e => e.ToDomain().ToResponse())
            .OrderBy(response => response.Name)
            .ThenBy(response => response.Id)
            .ToList();
    }

    public async Task<List<CountryResponse>> SeedGeoDataAsync()
    {
        var countryId = Guid.NewGuid();
        var countryEntity = new CountryEntity
        {
            Id = countryId,
            Name = "Geo Country 1",
            Slug = "geo-country-1",
            IsActive = true,
            Regions =
            [
                new RegionEntity
                {
                    Name = "Geo Region 1", Slug = "geo-region-1", IsActive = true,  CountryId = countryId
                },
                new RegionEntity
                {
                    Name = "Geo Region 2", Slug = "geo-region-2", IsActive = false, CountryId = countryId
                }
            ]
        };

        return await SeedCountriesAsync([countryEntity]);
    }

    public async Task ClearDatabaseAsync(CancellationToken ct = default)
    {
        DbContext.ChangeTracker.Clear();

        await DbContext.Set<DistilleryEntity>().ExecuteDeleteAsync(ct);
        await DbContext.Set<WhiskyBottleEntity>().ExecuteDeleteAsync(ct);
        await DbContext.Set<RegionEntity>().ExecuteDeleteAsync(ct);
        await DbContext.Set<CountryEntity>().ExecuteDeleteAsync(ct);
        
        await DbContext.SaveChangesAsync(ct);
    }

    public async Task SetupCountriesForTests()
    {
        await SeedCountriesAsync(
        [
            new CountryEntity
            {
                Id = FirstSeededCountryId,
                Name = "Geo Country 1",
                Slug = "geo-country-1",
                IsActive = true,
                Regions =
                [
                    new RegionEntity
                    {
                        Id = FirstRegionFirstCountryId,
                        CountryId = FirstSeededCountryId,
                        Name = "Geo Region 1",
                        Slug = "geo-region-1",
                        IsActive = true
                        
                    },
                    new RegionEntity
                    {
                        Id = SecondRegionFirstCountryId,
                        CountryId = FirstSeededCountryId,
                        Name = "Geo Region 2",
                        Slug = "geo-region-2",
                        IsActive = false
                    }
                ]
            }
            ,new CountryEntity
            {
                Id = SecondSeededCountryId,
                Name = "Geo Country 2",
                Slug = "geo-country-2",
                IsActive = true,
                Regions =
                [
                    new RegionEntity
                    {
                        Id = FirstRegionSecondCountryId,
                        CountryId = SecondSeededCountryId,
                        Name = "Geo Region 1",
                        Slug = "geo-region-1",
                        IsActive = true
                        
                    }
                ]
            }
        ]);
    }
}
