using MyWhiskyShelf.Core.Models;
using Pgvector;

namespace MyWhiskyShelf.Infrastructure.Persistence.Mapping;

public static class FlavourProfileMapping
{
    public static Vector ToVector(this FlavourProfile flavourProfile)
    {
        return new Vector(new[]
        {
            flavourProfile.Sweet / 10f,
            flavourProfile.Fruit / 10f,
            flavourProfile.Peat / 10f,
            flavourProfile.Spice / 10f,
            flavourProfile.Body / 10f
        });
    }

    public static FlavourProfile ToFlavourProfile(this Vector flavourVector)
    {
        var roundedValues = flavourVector
            .ToArray()
            .Select(item => (int)Math.Round(item * 10, MidpointRounding.AwayFromZero))
            .ToArray();

        return new FlavourProfile
        {
            Sweet = roundedValues[0],
            Fruit = roundedValues[1],
            Peat = roundedValues[2],
            Spice = roundedValues[3],
            Body = roundedValues[4]
        };
    }
}