using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.IntegrationTests.TestData;

public static class CountryEntityTestData
{
    public static CountryEntity Generic(string name)
    {
        return new CountryEntity
        {
            Name = name,
            IsActive = true
        };
    }
}