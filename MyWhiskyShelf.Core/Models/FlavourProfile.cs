namespace MyWhiskyShelf.Core.Models;

[Serializable]
public record FlavourProfile(
    int Sweet, int Fruit, int Floral, int Body, int Smoke, int Tobacco,
    int Medicinal, int Wine, int Spice, int Malt, int Nut, int Honey
)
{
    public FlavourProfile() : this(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
    {
    }
}
    