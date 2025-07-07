using MyWhiskyShelf.Core;
using MyWhiskyShelf.Models;

namespace MyWhiskySelf.Core.Tests;

public class FlavourProfileEncoderTests
{
    // The maximum encoded value occurs when all 12 flavour profile attributes have the highest possible value (4).
    // Each attribute uses 3 bits (since 0–4 fits in 3 bits), so we pack 12 values × 3 bits = 36 bits total.
    // This value (39268272420) represents 12 consecutive 3-bit values all set to binary 100 (decimal 4).
    private const ulong MaximumEncodedFlavourProfile = 39268272420ul;
    
    private const string SweetnessFlavourProfileOutOfRangeMessage = 
        "Flavour profile values cannot be lower than 0 or greater than 4 (Parameter 'Sweet')";
    
    private const string MaltFlavourProfileOutOfRangeMessage = 
        "Flavour profile values cannot be lower than 0 or greater than 4 (Parameter 'Malt')";
    
    private const string EncodedValueGreaterThanMaximumMessage = 
        "Encoded flavour profile values cannot be greater than '39268272420' (Parameter 'encodedFlavourProfile')";
    
    
    [Fact]
    public void When_EncodingFlavourProfileWithAllValuesSetToZero_Expect_EncodeReturnsZero()
    {
        var flavourProfile = new FlavourProfile();
        
        var encodedFlavourProfile = FlavourProfileEncoder.Encode(flavourProfile);
        
        Assert.Equal(0ul, encodedFlavourProfile);
    }
    
    [Fact]
    public void When_EncodingFlavourProfileWithAllValuesSetToFour_Expect_EncodeReturnsMaximumEncodedFlavourProfile()
    {
        var flavourProfile = new FlavourProfile
        (
            Sweet: 4, Fruit: 4, Floral: 4, Body: 4, Smoke: 4, Tobacco: 4, 
            Medicinal: 4, Wine: 4, Spice: 4, Malt: 4, Nut: 4, Honey: 4
        );

        var encodedFlavourProfile = FlavourProfileEncoder.Encode(flavourProfile);
        
        Assert.Equal(MaximumEncodedFlavourProfile, encodedFlavourProfile);
    }
    
    //     
    [Fact]
    public void When_EncodingSweetnessFlavourProfileWithAFlavourLessThanZero_Expect_ExceptionThrown()
    {
        var flavourProfile = new FlavourProfile { Sweet = -1 };
        
        var message = Assert.Throws<ArgumentOutOfRangeException>(() => FlavourProfileEncoder.Encode(flavourProfile));
        
        Assert.Equal(SweetnessFlavourProfileOutOfRangeMessage, message.Message);
    }
    
    [Fact]
    public void When_EncodingMaltFlavourProfileWithAFlavourLessThanFour_Expect_ExceptionThrown()
    {
        var flavourProfile = new FlavourProfile { Malt = 5 };
        
        var message = Assert.Throws<ArgumentOutOfRangeException>(() => FlavourProfileEncoder.Encode(flavourProfile));
        
        Assert.Equal(MaltFlavourProfileOutOfRangeMessage, message.Message);
    }

    [Fact]
    public void When_DecodingFlavourProfileWithAnEncodeValueOfZero_Expect_FlavourProfileWithAllValuesSetToZero()
    {
        const ulong encodedFlavourProfile = 0ul;

        var flavourProfile = FlavourProfileEncoder.Decode(encodedFlavourProfile);
        
        Assert.Equal(new FlavourProfile(), flavourProfile);
    }
    
    [Fact]
    public void When_DecodingFlavourProfileWithMaximumEncodedValue_Expect_FlavourProfileWithAllValuesSetToFour()
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
    public void When_DecodingFlavourProfileWithEncodedValueGreaterThanMaximum_ExpectExceptionsThrown()
    {
        const ulong encodedFlavourProfile = 40000000000ul;
        
        var exception = 
            Assert.Throws<ArgumentOutOfRangeException>(() => FlavourProfileEncoder.Decode(encodedFlavourProfile));
        
        Assert.Equal(EncodedValueGreaterThanMaximumMessage, exception.Message);
    }

    [Fact]
    public void WhenEncodingAndThenDecodingFlavourProfile_TheSameFlavourProfileIsReturned()
    {
        var flavourProfile = new FlavourProfile
        {
            Sweet = 0, Fruit = 1, Floral = 2, Body = 3, Smoke = 4, Tobacco = 4,
            Medicinal = 3, Wine = 2, Spice = 1, Malt = 0, Nut = 2, Honey = 4
        };
        
        var encodedFlavourProfile = FlavourProfileEncoder.Encode(flavourProfile);
        var decodedFlavourProfile = FlavourProfileEncoder.Decode(encodedFlavourProfile);
        
        Assert.Equal(flavourProfile, decodedFlavourProfile);
    }
}