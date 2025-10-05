using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.WebApi.Tests.TestData;

public static class FlavourProfileTestData
{
    public static readonly FlavourProfile Generic = new()
    {
        Sweet = 6,
        Fruit = 3,
        Peat = 1,
        Spice = 10,
        Body = 6
    };
}