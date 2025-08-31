using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Infrastructure.Encoding;
using MyWhiskyShelf.Infrastructure.Tests.TestData;

namespace MyWhiskyShelf.Infrastructure.Tests.Encoding;

public class FlavourProfileEncoderTests
{
    [Fact]
    public void When_EncodeWithAFlavourProfileWithAllValuesSetToZero_Expect_EncodedMin()
    {
        var encodedFlavourProfile = FlavourProfileEncoder.Encode(FlavourProfileTestData.Min);

        Assert.Equal(FlavourProfileTestData.EncodedMin, encodedFlavourProfile);
    }

    [Fact]
    public void When_EncodeWithMixedFlavourProfile_Expect_EncodedMixed()
    {
        var encodedFlavourProfile = FlavourProfileEncoder.Encode(FlavourProfileTestData.Mixed);

        Assert.Equal(FlavourProfileTestData.EncodedMixed, encodedFlavourProfile);
    }

    [Fact]
    public void When_EncodeWithMaxFlavourProfile_Expect_EncodedMax()
    {
        var encodedFlavourProfile = FlavourProfileEncoder.Encode(FlavourProfileTestData.Max);

        Assert.Equal(FlavourProfileTestData.EncodedMax, encodedFlavourProfile);
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
    public void When_DecodeWithEncodedMin_Expect_MinFlavourProfile()
    {
        var flavourProfile = FlavourProfileEncoder.Decode(FlavourProfileTestData.EncodedMin);

        Assert.Equal(FlavourProfileTestData.Min, flavourProfile);
    }

    [Fact]
    public void When_DecodeWithEncodedMax_Expect_MaxFlavourProfile()
    {
        var flavourProfile = FlavourProfileEncoder.Decode(FlavourProfileTestData.EncodedMax);

        Assert.Equal(FlavourProfileTestData.Max, flavourProfile);
    }

    [Fact]
    public void When_DecodeWithAValueGreaterThanEncodedMax_Expect_ExceptionThrown()
    {
        var exception =
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                FlavourProfileEncoder.Decode(FlavourProfileTestData.EncodedMax + 1));

        Assert.Equal(
            "Value cannot be greater than '39268272420' (Parameter 'encodedFlavourProfile')",
            exception.Message);
    }

    [Fact]
    public void When_EncodeThenDecode_Expect_MixedFlavourProfile()
    {
        var encoded = FlavourProfileEncoder.Encode(FlavourProfileTestData.Mixed);
        var decoded = FlavourProfileEncoder.Decode(encoded);

        Assert.Equal(FlavourProfileTestData.Mixed, decoded);
    }
}