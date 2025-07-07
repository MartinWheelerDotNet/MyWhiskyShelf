using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Database.Encoders;

public static class FlavourProfileEncoder
{
    private const int AmountOfFlavours = 12;
    private const int BitsPerFlavour = 3;
    private const int MaxValuePerFlavour = (1 << BitsPerFlavour) - 1;

    // The maximum encoded value occurs when all 12 flavour profile attributes have the highest possible value (4).
    // Each attribute uses 3 bits (since 0–4 fits in 3 bits), so we pack 12 values × 3 bits = 36 bits total.
    // This value (39268272420) represents 12 consecutive 3-bit values all set to binary 100 (decimal 4).
    private const ulong MaximumEncodedFlavourProfile = 39268272420ul;

    public static ulong Encode(FlavourProfile flavourProfile)
    {
        var encodedFlavourProfile = 0ul;
        var flavourProfileValues = ExtractFlavourProfileValues(flavourProfile);

        for (var index = 0; index < AmountOfFlavours; index++)
        {
            var value = flavourProfileValues[index].Value;
            
            if (value is < 0 or > 4)
                throw new ArgumentOutOfRangeException(
                    flavourProfileValues[index].ProfileName,
                    "Flavour profile values cannot be lower than 0 or greater than 4");
            
            encodedFlavourProfile |= (ulong) value << (index * BitsPerFlavour);
        }
            
        return encodedFlavourProfile;
    }

    private static (string ProfileName, int Value)[] ExtractFlavourProfileValues(FlavourProfile flavourProfile)
    {
        return [
            (nameof(FlavourProfile.Sweet), flavourProfile.Sweet), 
            (nameof(FlavourProfile.Fruit), flavourProfile.Fruit), 
            (nameof(FlavourProfile.Floral), flavourProfile.Floral), 
            (nameof(FlavourProfile.Body), flavourProfile.Body),
            (nameof(FlavourProfile.Smoke), flavourProfile.Smoke),
            (nameof(FlavourProfile.Tobacco), flavourProfile.Tobacco),
            (nameof(FlavourProfile.Medicinal), flavourProfile.Medicinal),
            (nameof(FlavourProfile.Wine), flavourProfile.Wine), 
            (nameof(FlavourProfile.Spice), flavourProfile.Spice),
            (nameof(FlavourProfile.Malt), flavourProfile.Malt),
            (nameof(FlavourProfile.Nut), flavourProfile.Nut),
            (nameof(FlavourProfile.Honey), flavourProfile.Honey)
        ]; 
    }

    public static FlavourProfile Decode(ulong encodedFlavourProfile)
    {
        if (encodedFlavourProfile > MaximumEncodedFlavourProfile)
            throw new ArgumentOutOfRangeException(
                nameof(encodedFlavourProfile),
                $"Value cannot be greater than '{MaximumEncodedFlavourProfile}'");
        
        var values = new int[12];

        for (var index = 0; index < AmountOfFlavours; index++)
        {
            values[index] = (int) (encodedFlavourProfile >> (index * BitsPerFlavour) & MaxValuePerFlavour);
        }

        return new FlavourProfile
        {
            Sweet = values[0],
            Fruit = values[1],
            Floral = values[2],
            Body = values[3],
            Smoke = values[4],
            Tobacco = values[5],
            Medicinal = values[6],
            Wine = values[7],
            Spice = values[8],
            Malt = values[9],
            Nut = values[10],
            Honey = values[11]
        };
    }
}