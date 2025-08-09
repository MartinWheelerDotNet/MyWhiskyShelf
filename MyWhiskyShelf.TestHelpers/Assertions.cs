using MyWhiskyShelf.Core.Models;
using Xunit.Sdk;

namespace MyWhiskyShelf.TestHelpers;

public static class Assertions
{
    public static bool EqualsIgnoringId(DistilleryResponse expected, DistilleryResponse actual)
    {
        return expected with { Id = Guid.Empty } == actual with { Id = Guid.Empty };
    }

    public static void AssertIsGuidAndNotEmpty(string guidString)
    {
        if (!Guid.TryParse(guidString, out var result))
            throw new XunitException($"Expected a valid GUID but got: '{guidString}'");

        if (Guid.Empty.Equals(result))
            throw new XunitException($"Expected a none-empty GUID but got: '{result}'");
    }
}