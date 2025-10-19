using MyWhiskyShelf.Core.Models;
using Pgvector;

namespace MyWhiskyShelf.Infrastructure.Tests.TestData;

public static class FlavourProfileTestData
{
    public static readonly Vector GenericVector = new(new[] { 1f, 0.8f, 0.6f, 0.4f, 0f });

    public static readonly FlavourProfile Generic = new()
    {
        Sweet = 10,
        Fruit = 8,
        Peat = 6,
        Spice = 4,
        Body = 0
    };
}