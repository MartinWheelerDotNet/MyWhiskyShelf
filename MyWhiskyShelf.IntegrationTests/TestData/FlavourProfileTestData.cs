using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.IntegrationTests.TestData;

public static class FlavourProfileTestData
{
    public static readonly FlavourProfile Generic = new()
    {
        Sweet = 3,
        Fruit = 7,
        Peat = 0,
        Spice = 9,
        Body = 4
    };
}