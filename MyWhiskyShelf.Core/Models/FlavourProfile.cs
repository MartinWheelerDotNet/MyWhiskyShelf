namespace MyWhiskyShelf.Core.Models;

[Serializable]
public record FlavourProfile(
    int Sweet,
    int Fruit,
    int Peat,
    int Spice,
    int Body
)
{
    public FlavourProfile() : this(0, 0, 0, 0, 0)
    {
    }
}