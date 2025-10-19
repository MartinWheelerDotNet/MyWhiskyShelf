using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.IntegrationTests.TestData;

public static class CountryEntityTestData
{
    public static CountryEntity Generic(string name, string slug)
    {
        return new CountryEntity
        {
            Name = name,
            Slug = slug,
            IsActive = true
        };
    }
}