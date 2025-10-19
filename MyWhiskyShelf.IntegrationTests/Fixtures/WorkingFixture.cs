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

    public async Task<List<DistilleryResponse>> SeedDistilleriesAsync(List<DistilleryEntity> entities)
    {
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
            .Select(i => DistilleryEntityTestData.Generic($"Distillery Number {i}"))
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
            Name = "Geo Country",
            Slug = "geo-country",
            IsActive = true,
            Regions =
            [
                new RegionEntity
                {
                    Name = "Geo Region 1",
                    Slug = "geo-region-1",
                    IsActive = true,
                    CountryId = countryId
                },
                new RegionEntity
                {
                    Name = "Geo Region 2",
                    Slug = "geo-region-2",
                    IsActive = false,
                    CountryId = countryId
                }
            ]
        };

        return await SeedCountriesAsync([countryEntity]);
    }
    
    public async Task ClearDatabaseAsync()
    {
        DbContext.Set<DistilleryEntity>().RemoveRange(DbContext.Set<DistilleryEntity>());
        DbContext.Set<WhiskyBottleEntity>().RemoveRange(DbContext.Set<WhiskyBottleEntity>());
        DbContext.Set<RegionEntity>().RemoveRange(DbContext.Set<RegionEntity>());
        DbContext.Set<CountryEntity>().RemoveRange(DbContext.Set<CountryEntity>());
        await DbContext.SaveChangesAsync();
    }

    public virtual async Task DisposeAsync()
    {
        await ClearDatabaseAsync();
        await DbContext.DisposeAsync();
        await Application.DisposeAsync();
    }
}