using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Infrastructure.Tests.TestData;

public static class FlavourProfileTestData
{
    // This has been precalculated against the 'Mixed' flavour profile.
    public const ulong EncodedMixed = 28054242952;

    // The maximum encoded value occurs when all 12 flavour profile attributes have the highest possible value (4).
    // Each attribute uses 3 bits (since 0–4 fits in 3 bits), so we pack 12 values × 3 bits = 36 bits total.
    // This value (39268272420) represents 12 consecutive 3-bit values all set to binary 100 (decimal 4).
    public const ulong EncodedMax = 39268272420;

    public const ulong EncodedMin = 0;

    public static readonly FlavourProfile Mixed = new()
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

    public static readonly FlavourProfile Max = new()
    {
        Sweet = 4,
        Fruit = 4,
        Floral = 4,
        Body = 4,
        Smoke = 4,
        Tobacco = 4,
        Medicinal = 4,
        Wine = 4,
        Spice = 4,
        Malt = 4,
        Nut = 4,
        Honey = 4
    };

    public static readonly FlavourProfile Min = new();
}