using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.TestHelpers;

public static class Assertions
{
    public static bool EqualsIgnoringId(DistilleryResponse expected, DistilleryResponse actual)
    {
        return expected with { Id = Guid.Empty } == actual with { Id = Guid.Empty };
    }
}