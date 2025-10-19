using MyWhiskyShelf.Infrastructure.Persistence.Mapping;
using MyWhiskyShelf.Infrastructure.Tests.TestData;

namespace MyWhiskyShelf.Infrastructure.Tests.Persistence.Mapping;

public class FlavourProfileMappingTests
{
    [Fact]
    public void When_MappingFlavourProfileToVector_ExpectCorrectVectorIsReturned()
    {
        var flavourProfile = FlavourProfileTestData.Generic;

        var flavourVector = flavourProfile.ToVector();

        Assert.Equal(FlavourProfileTestData.GenericVector, flavourVector);
    }

    [Fact]
    public void When_MappingVectorToFlavourProfile_ExpectCorrectFlavourProfileIsReturned()
    {
        var flavourVector = FlavourProfileTestData.GenericVector;

        var flavourProfile = flavourVector.ToFlavourProfile();

        Assert.Equal(FlavourProfileTestData.Generic, flavourProfile);
    }
}