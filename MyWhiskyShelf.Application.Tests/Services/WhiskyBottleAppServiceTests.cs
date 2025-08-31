using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Application.Results;
using MyWhiskyShelf.Application.Services;
using MyWhiskyShelf.Application.Tests.TestData;
using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Tests.Services;

public class WhiskyBottleAppServiceTests
{
    private readonly FakeLogger<WhiskyBottleAppService> _fakeLogger = new();
    private readonly Mock<IWhiskyBottleReadRepository> _mockRead = new();
    private readonly Mock<IWhiskyBottleWriteRepository> _mockWrite = new();
    private readonly WhiskyBottleAppService _service;

    public WhiskyBottleAppServiceTests()
    {
        _service = new WhiskyBottleAppService(_mockRead.Object, _mockWrite.Object, _fakeLogger);
    }

    [Fact]
    public async Task When_GetByIdAndWhiskyBottleNotFound_Expect_NullAndLogWarning()
    {
        var id = Guid.NewGuid();
        _mockRead.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WhiskyBottle?)null);

        var result = await _service.GetByIdAsync(id);

        Assert.Multiple(
            () => Assert.Null(result),
            () => Assert.Contains(_fakeLogger.Collector.GetSnapshot(), e => e.Level == LogLevel.Warning));
    }


    [Fact]
    public async Task When_GetByIdAndWhiskyBottleExists_Expect_ReturnWhiskyBottleAndLogDebug()
    {
        var id = Guid.NewGuid();
        var whiskyBottle = WhiskyBottleTestData.Generic with { Id = id };
        _mockRead.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(whiskyBottle);

        var result = await _service.GetByIdAsync(id);

        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(id, result!.Id),
            () => Assert.Contains(_fakeLogger.Collector.GetSnapshot(), e => e.Level == LogLevel.Debug));
    }

    [Fact]
    public async Task When_CreateAndWhiskyBottleCreated_Expect_CreatedAndLogDebug()
    {
        var newWhiskyBottle = WhiskyBottleTestData.Generic;
        var savedWhiskyBottle = WhiskyBottleTestData.Generic with { Id = Guid.NewGuid() };
        _mockWrite.Setup(w => w.AddAsync(newWhiskyBottle, It.IsAny<CancellationToken>()))
            .ReturnsAsync(savedWhiskyBottle);

        var result = await _service.CreateAsync(newWhiskyBottle);

        Assert.Multiple(
            () => Assert.Equal(CreateWhiskyBottleOutcome.Created, result.Outcome),
            () => Assert.Equal(savedWhiskyBottle, result.WhiskyBottle),
            () => Assert.Contains(_fakeLogger.Collector.GetSnapshot(), e => e.Level == LogLevel.Debug));
    }

    [Fact]
    public async Task When_CreateAndExceptionIsThrown_Expect_ErrorAndLogError()
    {
        var newWhiskyBottle = WhiskyBottleTestData.Generic;

        _mockWrite.Setup(w => w.AddAsync(newWhiskyBottle, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Exception"));

        var result = await _service.CreateAsync(newWhiskyBottle);

        Assert.Multiple(
            () => Assert.Equal(CreateWhiskyBottleOutcome.Error, result.Outcome),
            () => Assert.Equal("Exception", result.Error),
            () => Assert.Contains(_fakeLogger.Collector.GetSnapshot(), e => e.Level == LogLevel.Error));
    }

    [Fact]
    public async Task When_UpdateAndWhiskyBottleUpdated_Expect_UpdatedAndLogDebug()
    {
        var id = Guid.NewGuid();
        var updatedWhiskyBottle = WhiskyBottleTestData.Generic;
        _mockWrite.Setup(w => w.UpdateAsync(id, updatedWhiskyBottle, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.UpdateAsync(id, updatedWhiskyBottle);

        Assert.Multiple(
            () => Assert.Equal(UpdateWhiskyBottleOutcome.Updated, result.Outcome),
            () => Assert.Equal(updatedWhiskyBottle with { Id = id }, result.WhiskyBottle),
            () => Assert.Contains(_fakeLogger.Collector.GetSnapshot(), e => e.Level == LogLevel.Debug));
    }

    [Fact]
    public async Task When_UpdateAndWhiskyBottleNotFound_Expect_NotFoundAndLogWarning()
    {
        var id = Guid.NewGuid();
        var updatedWhiskyBottle = WhiskyBottleTestData.Generic;
        _mockWrite.Setup(w => w.UpdateAsync(id, updatedWhiskyBottle, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.UpdateAsync(id, updatedWhiskyBottle);

        Assert.Multiple(
            () => Assert.Equal(UpdateWhiskyBottleOutcome.NotFound, result.Outcome),
            () => Assert.Contains(_fakeLogger.Collector.GetSnapshot(), e => e.Level == LogLevel.Warning));
    }

    [Fact]
    public async Task When_UpdateAndExceptionIsThrown_Expect_ErrorAndLogWarning()
    {
        var id = Guid.NewGuid();
        var updatedWhiskyBottle = WhiskyBottleTestData.Generic;
        _mockWrite.Setup(w => w.UpdateAsync(id, updatedWhiskyBottle, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Exception"));

        var result = await _service.UpdateAsync(id, updatedWhiskyBottle);

        Assert.Multiple(
            () => Assert.Equal(UpdateWhiskyBottleOutcome.Error, result.Outcome),
            () => Assert.Equal("Exception", result.Error),
            () => Assert.Contains(_fakeLogger.Collector.GetSnapshot(), e => e.Level == LogLevel.Warning));
    }

    [Fact]
    public async Task When_DeleteAndWhiskyBottleFound_Expect_DeletedAndLogDebug()
    {
        var id = Guid.NewGuid();
        _mockWrite.Setup(w => w.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.DeleteAsync(id);

        Assert.Multiple(
            () => Assert.Equal(DeleteWhiskyBottleOutcome.Deleted, result.Outcome),
            () => Assert.Contains(_fakeLogger.Collector.GetSnapshot(), e => e.Level == LogLevel.Debug));
    }

    [Fact]
    public async Task When_DeleteAndWhiskyBottleNotFound_Expect_NotFoundAndLogWarning()
    {
        var id = Guid.NewGuid();
        _mockWrite.Setup(w => w.DeleteAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _service.DeleteAsync(id);

        Assert.Multiple(
            () => Assert.Equal(DeleteWhiskyBottleOutcome.NotFound, result.Outcome),
            () => Assert.Contains(_fakeLogger.Collector.GetSnapshot(), e => e.Level == LogLevel.Warning));
    }

    [Fact]
    public async Task When_DeleteAndExceptionIsThrown_Expect_ErrorAndLogError()
    {
        var id = Guid.NewGuid();
        _mockWrite.Setup(w => w.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Exception"));

        var result = await _service.DeleteAsync(id);

        Assert.Multiple(
            () => Assert.Equal(DeleteWhiskyBottleOutcome.Error, result.Outcome),
            () => Assert.Equal("Exception", result.Error),
            () => Assert.Contains(_fakeLogger.Collector.GetSnapshot(), e => e.Level == LogLevel.Error));
    }
}