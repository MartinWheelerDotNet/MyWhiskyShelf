using Microsoft.EntityFrameworkCore;
using Moq;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.Database.Services;
using MyWhiskyShelf.Database.Tests.Resources;
using MyWhiskyShelf.Database.Tests.TestHelpers;
using MyWhiskyShelf.TestHelpers.Data;

namespace MyWhiskyShelf.Database.Tests.Services;

public class WhiskyBottleWriteServiceTests
{
    [Fact]
    public async Task When_AddWhiskyBottle_ExpectMapToEntityIsCalledWithProvidedWhiskyBottle()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync<WhiskyBottleEntity>();

        var mockWhiskyBottleMapper = new Mock<IMapper<WhiskyBottle, WhiskyBottleEntity>>();
        mockWhiskyBottleMapper
            .Setup(mapper => mapper.MapToEntity(WhiskyBottleTestData.AllValuesPopulated))
            .Returns(WhiskyBottleEntityTestData.AllValuesPopulated)
            .Verifiable(Times.Once);

        var whiskyBottleService = new WhiskyBottleWriteService(dbContext, mockWhiskyBottleMapper.Object);
        var bottleAdded = await whiskyBottleService.TryAddAsync(WhiskyBottleTestData.AllValuesPopulated);

        Assert.Multiple(
            () => Assert.True(bottleAdded),
            () => mockWhiskyBottleMapper.Verify());
    }

    [Fact]
    public async Task When_AddWhiskyBottle_ExpectMappedWhiskyBottleEntityIsAddedToDatabase()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync<WhiskyBottleEntity>();

        var mockWhiskyBottleMapper = new Mock<IMapper<WhiskyBottle, WhiskyBottleEntity>>();
        mockWhiskyBottleMapper
            .Setup(mapper => mapper.MapToEntity(WhiskyBottleTestData.AllValuesPopulated))
            .Returns(WhiskyBottleEntityTestData.AllValuesPopulated);

        var whiskyBottleService = new WhiskyBottleWriteService(dbContext, mockWhiskyBottleMapper.Object);
        await whiskyBottleService.TryAddAsync(WhiskyBottleTestData.AllValuesPopulated);

        var whiskyBottleEntity = dbContext
            .Set<WhiskyBottleEntity>()
            .Where(whiskyBottle => whiskyBottle.Name == WhiskyBottleTestData.AllValuesPopulated.Name);

        Assert.Single(whiskyBottleEntity);
    }

    [Theory]
    [InlineData(typeof(DbUpdateException))]
    [InlineData(typeof(DbUpdateConcurrencyException))]
    [InlineData(typeof(Exception))]
    public async Task When_AddWhiskyBottleAndDatabaseThrowsAnException_Expect_ReturnsFalse(Type exceptionType)
    {
        await using var dbContext = MyWhiskyShelfContextBuilder
            .CreateFailingDbContextAsync(exceptionType);

        var mockWhiskyBottleMapper = new Mock<IMapper<WhiskyBottle, WhiskyBottleEntity>>();
        mockWhiskyBottleMapper
            .Setup(mapper => mapper.MapToEntity(WhiskyBottleTestData.AllValuesPopulated))
            .Returns(WhiskyBottleEntityTestData.AllValuesPopulated);

        var whiskyBottleService = new WhiskyBottleWriteService(dbContext, mockWhiskyBottleMapper.Object);
        var tryAddResult = await whiskyBottleService.TryAddAsync(WhiskyBottleTestData.AllValuesPopulated);

        Assert.False(tryAddResult);
    }
}