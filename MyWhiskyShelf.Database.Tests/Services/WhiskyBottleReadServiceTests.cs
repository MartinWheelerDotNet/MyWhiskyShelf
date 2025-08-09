using Moq;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.Database.Services;
using MyWhiskyShelf.Database.Tests.Resources;
using MyWhiskyShelf.Database.Tests.TestHelpers;
using MyWhiskyShelf.TestHelpers.Data;

namespace MyWhiskyShelf.Database.Tests.Services;

public class WhiskyBottleReadServiceTests
{
    [Fact]
    public async Task When_GetByIdAsyncAndWhiskyBottleExists_Expect_WhiskyBottleReturned()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync(WhiskyBottleEntityTestData.AllValuesPopulated);

        var mockRequestToEntityMapper = new Mock<IMapper<WhiskyBottleEntity, WhiskyBottleResponse>>();

        mockRequestToEntityMapper
            .Setup(mapper => mapper.Map(It.IsAny<WhiskyBottleEntity>()))
            .Returns(WhiskyBottleResponseTestData.AllValuesPopulated)
            .Verifiable(Times.Once);

        var whiskyBottleReadService = new WhiskyBottleReadService(dbContext, mockRequestToEntityMapper.Object);
        var whiskyBottle = await whiskyBottleReadService.GetByIdAsync(WhiskyBottleEntityTestData.AllValuesPopulated.Id);

        Assert.Multiple(
            () => Assert.Equivalent(WhiskyBottleResponseTestData.AllValuesPopulated, whiskyBottle),
            () => mockRequestToEntityMapper.Verify());
    }

    [Fact]
    public async Task When_GetByIdAsyncAndWhiskyBottleDoesNotExist_Expect_WhiskyBottleIsNotReturned()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder.CreateDbContextAsync<WhiskyBottleEntity>();

        var mockRequestToEntityMapper = new Mock<IMapper<WhiskyBottleEntity, WhiskyBottleResponse>>();

        mockRequestToEntityMapper
            .Setup(mapper => mapper.Map(It.IsAny<WhiskyBottleEntity>()))
            .Verifiable(Times.Never);

        var whiskyBottleReadService = new WhiskyBottleReadService(dbContext, mockRequestToEntityMapper.Object);
        var whiskyBottle = await whiskyBottleReadService.GetByIdAsync(WhiskyBottleEntityTestData.AllValuesPopulated.Id);

        Assert.Multiple(
            () => Assert.Null(whiskyBottle),
            () => mockRequestToEntityMapper.Verify());
    }
}