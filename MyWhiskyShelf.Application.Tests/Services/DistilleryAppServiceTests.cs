using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Application.Results;
using MyWhiskyShelf.Application.Services;
using MyWhiskyShelf.Application.Tests.TestData;
using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Tests.Services;

public class DistilleryAppServiceTests
{
    private readonly FakeLogger<DistilleryAppService> _fakeLogger = new();
    private readonly Mock<IDistilleryReadRepository> _mockRead = new();
    private readonly Mock<IDistilleryWriteRepository> _mockWrite = new();
    private readonly DistilleryAppService _service;

    public DistilleryAppServiceTests()
    {
        _service = new DistilleryAppService(_mockRead.Object, _mockWrite.Object, _fakeLogger);
    }

    [Fact]
    public async Task When_GetByIdAndDistilleryNotFound_Expect_NullAndLogWarning()
    {
        var id = Guid.NewGuid();
        _mockRead.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Distillery?)null);

        var result = await _service.GetByIdAsync(id);

        Assert.Null(result);
        Assert.Contains(_fakeLogger.Collector.GetSnapshot(), l => l.Level == LogLevel.Warning);
    }

    [Fact]
    public async Task When_GetByIdAndDistilleryExists_Expect_ReturnDistilleryAndLogDebug()
    {
        var id = Guid.NewGuid();
        _mockRead.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DistilleryTestData.Generic with { Id = id });

        var result = await _service.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Contains(_fakeLogger.Collector.GetSnapshot(), l => l.Level == LogLevel.Debug);
    }

    [Fact]
    public async Task When_GetAllAndTwoDistilleriesExist_Expect_ListOfTwoDistilleriesAndLogDebug()
    {
        List<Distillery> list =
        [
            DistilleryTestData.Generic with { Name = "First Distillery" },
            DistilleryTestData.Generic with { Name = "Second Distillery" }
        ];
        _mockRead.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var result = await _service.GetAllAsync();

        Assert.Equal(list, result);
        Assert.Contains(_fakeLogger.Collector.GetSnapshot(), l => l.Level == LogLevel.Debug);
    }

    [Fact]
    public async Task When_GetAllAndNoDistilleriesExist_Expect_EmptyListAndLogDebug()
    {
        _mockRead.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Distillery>());

        var result = await _service.GetAllAsync();

        Assert.Empty(result);
        Assert.Contains(_fakeLogger.Collector.GetSnapshot(), l => l.Level == LogLevel.Debug);
    }

    [Fact]
    public async Task When_CreateAndDistilleryWithThatNameAlreadyExists_Expect_AlreadyExistsAndLogWarning()
    {
        var newDistillery = DistilleryTestData.Generic with { Name = "New Distillery" };
        _mockRead.Setup(r => r.ExistsByNameAsync("New Distillery", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.CreateAsync(newDistillery);

        Assert.Equal(CreateDistilleryOutcome.AlreadyExists, result.Outcome);
        Assert.Contains(_fakeLogger.Collector.GetSnapshot(), l => l.Level == LogLevel.Warning);
        _mockWrite.Verify(w => w.AddAsync(It.IsAny<Distillery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task When_CreateAndDistilleryCreated_Expect_CreatedAndLogDebug()
    {
        var newDistillery = DistilleryTestData.Generic with { Name = "New Distillery" };
        var savedDistillery = DistilleryTestData.Generic with { Id = Guid.NewGuid(), Name = "New Distillery" };

        _mockRead.Setup(r => r.ExistsByNameAsync(newDistillery.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockWrite.Setup(w => w.AddAsync(newDistillery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(savedDistillery);

        var result = await _service.CreateAsync(newDistillery);

        Assert.Equal(CreateDistilleryOutcome.Created, result.Outcome);
        Assert.Equal(savedDistillery, result.Distillery);
        Assert.Contains(_fakeLogger.Collector.GetSnapshot(), l => l.Level == LogLevel.Debug);
    }

    [Fact]
    public async Task When_CreateAndExceptionIsThrown_Expect_ErrorAndLogError()
    {
        var newDistillery = DistilleryTestData.Generic with { Name = "New Distillery" };
        _mockRead.Setup(r => r.ExistsByNameAsync(newDistillery.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockWrite.Setup(w => w.AddAsync(newDistillery, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Exception"));

        var result = await _service.CreateAsync(newDistillery);

        Assert.Equal(CreateDistilleryOutcome.Error, result.Outcome);
        Assert.Equal("Exception", result.Error);
        Assert.Contains(_fakeLogger.Collector.GetSnapshot(), l => l.Level == LogLevel.Error);
    }

    [Fact]
    public async Task When_UpdateAndDistilleryNotFound_Expect_NotFoundAndLogWarning()
    {
        var id = Guid.NewGuid();
        var updatedDistillery = DistilleryTestData.Generic with { Id = id, Name = "Updated Distillery" };
        _mockRead.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Distillery?)null);

        var result = await _service.UpdateAsync(id, updatedDistillery);

        Assert.Equal(UpdateDistilleryOutcome.NotFound, result.Outcome);
        Assert.Contains(_fakeLogger.Collector.GetSnapshot(), l => l.Level == LogLevel.Warning);
    }

    [Fact]
    public async Task When_UpdateAndDistilleryNameChangedToExistingDistilleryName_Expect_NameConflictAndLogWarning()
    {
        var id = Guid.NewGuid();
        var currentDistillery = DistilleryTestData.Generic with { Id = id, Name = "Current Distillery" };
        var updatedDistillery = DistilleryTestData.Generic with { Id = id, Name = "Updated Distillery" };

        _mockRead.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentDistillery);
        _mockRead.Setup(r => r.ExistsByNameAsync(updatedDistillery.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.UpdateAsync(id, updatedDistillery);

        Assert.Equal(UpdateDistilleryOutcome.NameConflict, result.Outcome);
        Assert.Contains(_fakeLogger.Collector.GetSnapshot(), l => l.Level == LogLevel.Warning);
    }

    [Fact]
    public async Task When_UpdateAndDistilleryUpdated_Expect_UpdatedAndLogDebug()
    {
        var id = Guid.NewGuid();
        var currentDistillery = DistilleryTestData.Generic with { Id = id, Name = "Current Distillery" };
        var updatedDistillery = DistilleryTestData.Generic with { Id = id, Name = "Updated Distillery" };

        _mockRead.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentDistillery);
        _mockRead.Setup(r => r.ExistsByNameAsync(updatedDistillery.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockWrite.Setup(w => w.UpdateAsync(id, updatedDistillery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.UpdateAsync(id, updatedDistillery);

        Assert.Multiple(
            () => Assert.Equal(UpdateDistilleryOutcome.Updated, result.Outcome),
            () => Assert.Contains(_fakeLogger.Collector.GetSnapshot(), l => l.Level == LogLevel.Debug));
    }

    [Fact]
    public async Task When_UpdateAndDistilleryDeletedAfterLookup_Expect_NotFoundAndLogWarning()
    {
        var id = Guid.NewGuid();
        var currentDistillery = DistilleryTestData.Generic with { Id = id, Name = "Current Distillery" };
        var updatedDistillery = DistilleryTestData.Generic with { Id = id, Name = "Updated Distillery" };
        _mockRead.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentDistillery);
        _mockWrite.Setup(w => w.UpdateAsync(id, updatedDistillery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.UpdateAsync(id, updatedDistillery);

        Assert.Multiple(
            () => Assert.Equal(UpdateDistilleryOutcome.NotFound, result.Outcome),
            () => Assert.Contains(_fakeLogger.Collector.GetSnapshot(), l => l.Level == LogLevel.Warning));
    }

    [Fact]
    public async Task When_UpdateAndExceptionIsThrown_Expect_ErrorAndLogWarning()
    {
        var id = Guid.NewGuid();
        var currentDistillery = DistilleryTestData.Generic with { Id = id, Name = "Current Distillery" };
        var updatedDistillery = DistilleryTestData.Generic with { Id = id, Name = "Updated Distillery" };

        _mockRead.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentDistillery);
        _mockRead.Setup(r => r.ExistsByNameAsync(updatedDistillery.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockWrite.Setup(w => w.UpdateAsync(id, updatedDistillery, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Exception"));

        var result = await _service.UpdateAsync(id, updatedDistillery);

        Assert.Multiple(
            () => Assert.Equal(UpdateDistilleryOutcome.Error, result.Outcome),
            () => Assert.Equal("Exception", result.Error),
            () => Assert.Contains(_fakeLogger.Collector.GetSnapshot(), l => l.Level == LogLevel.Warning));
    }

    [Fact]
    public async Task When_DeleteAndDistilleryFound_Expect_DeletedAndLogDebug()
    {
        var id = Guid.NewGuid();
        _mockWrite.Setup(w => w.DeleteAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _service.DeleteAsync(id);

        Assert.Multiple(
            () => Assert.Equal(DeleteDistilleryOutcome.Deleted, result.Outcome),
            () => Assert.Contains(_fakeLogger.Collector.GetSnapshot(), l => l.Level == LogLevel.Debug));
    }

    [Fact]
    public async Task When_DeleteAndDistilleryNotFound_Expect_NotFoundAndLogWarning()
    {
        var id = Guid.NewGuid();
        _mockWrite.Setup(w => w.DeleteAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _service.DeleteAsync(id);

        Assert.Multiple(
            () => Assert.Equal(DeleteDistilleryOutcome.NotFound, result.Outcome),
            () => Assert.Contains(_fakeLogger.Collector.GetSnapshot(), l => l.Level == LogLevel.Warning));
    }

    [Fact]
    public async Task When_DeleteAndExceptionIsThrown_Expect_ErrorAndLogError()
    {
        var id = Guid.NewGuid();
        _mockWrite.Setup(w => w.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Exception"));

        var result = await _service.DeleteAsync(id);

        Assert.Multiple(
            () => Assert.Equal(DeleteDistilleryOutcome.Error, result.Outcome),
            () => Assert.Equal("Exception", result.Error),
            () => Assert.Contains(_fakeLogger.Collector.GetSnapshot(), l => l.Level == LogLevel.Error));
    }
}