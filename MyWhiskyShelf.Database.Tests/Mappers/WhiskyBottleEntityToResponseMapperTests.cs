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
        var whiskyBottleEntity = WhiskyBottleEntityTestData.AllValuesPopulated;
        whiskyBottleEntity.YearBottled = null;
        var whiskyBottleMapper = new WhiskyBottleEntityToResponseMapper();
        var whiskyBottle = whiskyBottleMapper
            .Map(whiskyBottleEntity);

        Assert.Equal(WhiskyBottleEntityTestData.AllValuesPopulated.DateBottled?.Year, whiskyBottle.YearBottled);
    }

    [Fact]
    public void When_MapToDomainWithoutDateBottledOrYearBottled_Expect_DomainModelWithoutYearBottledSet()
    {
        var whiskyBottleEntity = WhiskyBottleEntityTestData.AllValuesPopulated;
        whiskyBottleEntity.YearBottled = null;
        whiskyBottleEntity.DateBottled = null;
            
        var whiskyBottleMapper = new WhiskyBottleEntityToResponseMapper();
        var whiskyBottle = whiskyBottleMapper
            .Map(whiskyBottleEntity);

        Assert.Null(whiskyBottle.YearBottled);
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
        var whiskyBottleEntity = WhiskyBottleEntityTestData.AllValuesPopulated;
        whiskyBottleEntity.Status = status;
        
        var whiskyBottleMapper = new WhiskyBottleEntityToResponseMapper();
        var whiskyBottle = whiskyBottleMapper.Map(whiskyBottleEntity);

        Assert.Equal(expectedStatus.ToString(), whiskyBottle.Status);
    }
}