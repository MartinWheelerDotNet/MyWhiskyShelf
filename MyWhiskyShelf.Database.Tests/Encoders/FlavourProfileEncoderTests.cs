using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Encoders;

namespace MyWhiskyShelf.Database.Tests.Encoders;

public class FlavourProfileEncoderTests
{
    // The maximum encoded value occurs when all 12 flavour profile attributes have the highest possible value (4).
    // Each attribute uses 3 bits (since 0–4 fits in 3 bits), so we pack 12 values × 3 bits = 36 bits total.
    // This value (39268272420) represents 12 consecutive 3-bit values all set to binary 100 (decimal 4).
    private const ulong MaximumEncodedFlavourProfile = 39268272420ul;

    [Fact]
    public void When_EncodeWithAFlavourProfileWithAllValuesSetToZero_Expect_0()
    {
        var encodedFlavourProfile = FlavourProfileEncoder.Encode(new FlavourProfile());

        Assert.Equal(0ul, encodedFlavourProfile);
    }

    [Fact]
    public void When_EncodeWithAFlavourProfileWithAllValuesSetTo4_Expect_MaximumEncodedFlavourProfile()
    {
        var encodedFlavourProfile = FlavourProfileEncoder.Encode(new FlavourProfile
        {
            Sweet = 4, Fruit = 4, Floral = 4, Body = 4, Smoke = 4, Tobacco = 4,
            Medicinal = 4, Wine = 4, Spice = 4, Malt = 4, Nut = 4, Honey = 4
        });

        Assert.Equal(MaximumEncodedFlavourProfile, encodedFlavourProfile);
    }

    [Fact]
    public void When_EncodeWithAFlavourProfileWithAValueLessThanZero_Expect_ExceptionThrown()
    {
        var flavourProfile = new FlavourProfile { Sweet = -1 };

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => FlavourProfileEncoder.Encode(flavourProfile));

        Assert.Equal(
            "Flavour profile values cannot be lower than 0 or greater than 4 (Parameter 'Sweet')",
            exception.Message);
    }

    [Fact]
    public void When_EncodeWithAFlavourProfileWithAValueGreaterThanFour_Expect_ExceptionThrown()
    {
        var flavourProfile = new FlavourProfile { Malt = 5 };

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => FlavourProfileEncoder.Encode(flavourProfile));

        Assert.Equal(
            "Flavour profile values cannot be lower than 0 or greater than 4 (Parameter 'Malt')",
            exception.Message);
    }

    [Fact]
    public void When_DecodeWithWithZero_Expect_FlavourProfileWithAllValueSetToFour()
    {
        var expectedFlavourProfile = new FlavourProfile();

        var flavourProfile = FlavourProfileEncoder.Decode(0ul);

        Assert.Equal(expectedFlavourProfile, flavourProfile);
    }

    [Fact]
    public void When_DecodeWithMaximumEncodedFlavourProfile_Expect_FlavourProfileWithAllValuesSetToFour()
    {
        var expectedFlavourProfile = new FlavourProfile
        {
            Sweet = 4, Fruit = 4, Floral = 4, Body = 4, Smoke = 4, Tobacco = 4,
            Medicinal = 4, Wine = 4, Spice = 4, Malt = 4, Nut = 4, Honey = 4
        };

        var flavourProfile = FlavourProfileEncoder.Decode(MaximumEncodedFlavourProfile);

        Assert.Equal(expectedFlavourProfile, flavourProfile);
    }

    [Fact]
    public void When_DecodeWithAValueGreaterThanMaximumEncodedFlavourProfile_Expect_ExceptionThrown()
    {
        var exception =
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                FlavourProfileEncoder.Decode(MaximumEncodedFlavourProfile + 1));

        Assert.Equal(
            "Value cannot be greater than '39268272420' (Parameter 'encodedFlavourProfile')",
            exception.Message);
    }
}