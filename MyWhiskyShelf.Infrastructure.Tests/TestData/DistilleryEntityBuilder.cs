using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.Infrastructure.Tests.TestData;

public sealed class DistilleryEntityBuilder
{
    private Guid _id;
    private CountryEntity _country = new()
    {
        Id = Guid.NewGuid(),
        Name = "Added Country",
        Slug = "added-country",
        IsActive = true
    };

    public DistilleryEntityBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public DistilleryEntityBuilder AddRegion()
    {
        var countryId = Guid.NewGuid();
        _country = new CountryEntity
        {
            Id = countryId,
            Name = "Added Country",
            Slug = "added-country",
            IsActive = true,
            Regions = 
            [
                new RegionEntity
                { 
                    Id = Guid.NewGuid(), 
                    CountryId = countryId,
                    Name = "Added Region",
                    Slug = "added-region",
                    IsActive = true 
                }
            ]
        };
        return this;
    }

    public DistilleryEntity Build()
    {
        return new DistilleryEntity
        {
            Id = _id,
            Name = "Name",
            CountryId = _country.Id,
            Country = _country,
            RegionId = _country.Regions.SingleOrDefault()?.Id ?? null,
            Region = _country.Regions.SingleOrDefault(),
            Founded = 2000,
            Owner = "Owner",
            Type = "Type",
            Description = "Description",
            TastingNotes = "TastingNotes",
            FlavourVector = FlavourProfileTestData.GenericVector,
            Active = true
        };
    }
}