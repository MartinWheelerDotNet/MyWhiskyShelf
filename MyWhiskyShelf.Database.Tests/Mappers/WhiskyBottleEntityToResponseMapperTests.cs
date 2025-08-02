using MyWhiskyShelf.Core.Enums;
using MyWhiskyShelf.Database.Mappers;
using MyWhiskyShelf.Database.Tests.Resources;
using MyWhiskyShelf.TestHelpers.Data;

namespace MyWhiskyShelf.Database.Tests.Mappers;

public class WhiskyBottleEntityToResponseMapperTests
{
    [Fact]
    public void When_MapWithAllValuesPopulated_Expect_ResponseModelWithAllValuesPopulated()
    {
        var whiskyBottleMapper = new WhiskyBottleEntityToResponseMapper();

        var whiskyBottle = whiskyBottleMapper
            .Map(WhiskyBottleEntityTestData.AllValuesPopulated);

        Assert.Equal(WhiskyBottleResponseTestData.AllValuesPopulated, whiskyBottle);
    }

    [Fact]
    public void When_MapToDomainWithDateBottledButNotYearBottled_Expect_DomainModelWithYearBottledSetFromDate()
    {
        var whiskyBottleMapper = new WhiskyBottleEntityToResponseMapper();
        var whiskyBottle = whiskyBottleMapper
            .Map(WhiskyBottleEntityTestData.AllValuesPopulated with { YearBottled = null });

        Assert.Equal(WhiskyBottleEntityTestData.AllValuesPopulated.DateBottled?.Year, whiskyBottle.YearBottled);
    }

    [Fact]
    public void When_MapToDomainWithoutDateBottledOrYearBottled_Expect_DomainModelWithoutYearBottledSet()
    {
        var whiskyBottleMapper = new WhiskyBottleEntityToResponseMapper();
        var whiskyBottleEntity = whiskyBottleMapper
            .Map(WhiskyBottleEntityTestData.AllValuesPopulated with { DateBottled = null, YearBottled = null });

        Assert.Null(whiskyBottleEntity.YearBottled);
    }

    [Theory]
    [InlineData("Unknown", BottleStatus.Unknown)]
    [InlineData("Unopened", BottleStatus.Unopened)]
    [InlineData("Opened", BottleStatus.Opened)]
    [InlineData("Finished", BottleStatus.Finished)]
    public void When_MapToDomainWithStatusStrings_Expect_EntityDomainModelStatusSet(
        string status,
        BottleStatus expectedStatus)
    {
        var whiskyBottleMapper = new WhiskyBottleEntityToResponseMapper();
        var whiskyBottle = whiskyBottleMapper
            .Map(WhiskyBottleEntityTestData.AllValuesPopulated with { Status = status });

        Assert.Equal(expectedStatus.ToString(), whiskyBottle.Status);
    }
}