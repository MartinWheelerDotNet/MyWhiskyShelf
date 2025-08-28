using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.WebApi.Tests.TestData;

public static class FlavourProfileTestData
{
    public static readonly FlavourProfile Generic = new()
    {
        Sweet = 0,
        Fruit = 1,
        Floral = 2,
        Body = 3,
        Smoke = 4,
        Tobacco = 3,
        Medicinal = 2,
        Wine = 1,
        Spice = 0,
        Malt = 1,
        Nut = 2,
        Honey = 3
    };
}