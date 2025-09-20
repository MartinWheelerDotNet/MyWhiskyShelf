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
    public async Task When_GetByIdAndDistilleryNotFound_Expect_NotFound()
    {
        var id = Guid.NewGuid();
        _mockRead.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Distillery?)null);

        var result = await _service.GetByIdAsync(id);

        Assert.Equal(GetDistilleryByIdOutcome.NotFound, result.Outcome);
    }

    [Fact]
    public async Task When_GetByIdAndDistilleryFound_Expect_SuccessWithDistillery()
    {
        var id = Guid.NewGuid();
        _mockRead.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DistilleryTestData.Generic with { Id = id });

        var result = await _service.GetByIdAsync(id);

        Assert.Multiple(
            () => Assert.Equal(GetDistilleryByIdOutcome.Success, result.Outcome),
            () => Assert.Equal(DistilleryTestData.Generic with { Id = id}, result.Distillery));
    }
    
    [Fact]
    public async Task When_GetByIdAndErrorOccurs_Expect_ErrorWithExceptionMessage()
    {
        var id = Guid.NewGuid();
        _mockRead
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Exception"));

        var result = await _service.GetByIdAsync(id);

        Assert.Multiple(
            () => Assert.Equal(GetDistilleryByIdOutcome.Error, result.Outcome),
            () => Assert.Equal("Exception", result.Error),
            () => Assert.Equal(LogLevel.Error, _fakeLogger.Collector.LatestRecord.Level),
            () => Assert.Equal(
                $"Error retrieving distillery with [Id: {id}]",
                _fakeLogger.Collector.LatestRecord.Message));
    }

    [Fact]
    public async Task When_GetAllAndTwoDistilleriesExist_Expect_SuccessWithListOfTwoDistilleries()
    {
        List<Distillery> expectedDistilleries =
        [
            DistilleryTestData.Generic with { Name = "First Distillery" },
            DistilleryTestData.Generic with { Name = "Second Distillery" }
        ];
        _mockRead.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDistilleries);

        var result = await _service.GetAllAsync();

        Assert.Multiple(
            () => Assert.Equal(GetAllDistilleriesOutcome.Success, result.Outcome),
            () => Assert.Equal(expectedDistilleries, result.Distilleries));
    }

    [Fact]
    public async Task When_GetAllAndNoDistilleriesExist_Expect_SuccessWithEmptyList()
    {
        _mockRead.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await _service.GetAllAsync();

        Assert.Multiple(
            () => Assert.Equal(GetAllDistilleriesOutcome.Success, result.Outcome),
            () => Assert.Empty(result.Distilleries!));
    }
    
    [Fact]
    public async Task When_GetAllAndErrorOccurs_Expect_ErrorWithMessage()
    {
        _mockRead
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Exception"));

        var result = await _service.GetAllAsync();

        Assert.Multiple(
            () => Assert.Equal(GetAllDistilleriesOutcome.Error, result.Outcome),
            () => Assert.Equal("Exception", result.Error),
            () => Assert.Equal(LogLevel.Error, _fakeLogger.Collector.LatestRecord.Level),
            () => Assert.Equal(
                "An error occured whilst retrieving all distilleries",
                _fakeLogger.Collector.LatestRecord.Message));
    }

    [Fact]
    public async Task When_SearchAndTwoDistilleriesFound_Expect_SuccessWithListOfTwoDistilleriesNames()
    {
        List<DistilleryName> expectedDistilleryNames =
        [
            new(Guid.NewGuid(), "Distillery A"),
            new(Guid.NewGuid(), "Distillery B")
        ];
        _mockRead.Setup(r => r.SearchByNameAsync("Distillery", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDistilleryNames);
        
        var result = await _service.SearchByNameAsync("Distillery");
        
        Assert.Multiple(
            () => Assert.Equal(SearchDistilleriesOutcome.Success, result.Outcome),
            () => Assert.Equal(expectedDistilleryNames, result.DistilleryNames));
    }
    
    [Fact]
    public async Task When_SearchAndNoDistilleriesFound_ExpectSuccessWithEmptyList()
    {
        const string pattern = "Distillery";
        _mockRead.Setup(r => r.SearchByNameAsync(pattern, It.IsAny<CancellationToken>())).ReturnsAsync([]);
        
        var result = await _service.SearchByNameAsync(pattern);
        
        Assert.Multiple(
            () => Assert.Equal(SearchDistilleriesOutcome.Success, result.Outcome),
            () => Assert.Empty(result.DistilleryNames!));
    }
    
    [Fact]
    public async Task When_SearchAndErrorOccurs_ExpectErrorWithMessage()
    {
        const string pattern = "Distillery";
        _mockRead
            .Setup(r => r.SearchByNameAsync(pattern, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Exception"));
        
        var result = await _service.SearchByNameAsync(pattern);
        
        Assert.Multiple(
            () => Assert.Equal(SearchDistilleriesOutcome.Error, result.Outcome),
            () => Assert.Equal("Exception", result.Error),
            () => Assert.Equal(LogLevel.Error, _fakeLogger.Collector.LatestRecord.Level),
            () => Assert.Equal(
                $"An error occured whilst searching for distilleries with [Pattern: {pattern}",
                _fakeLogger.Collector.LatestRecord.Message));
    }
    
    [Fact]
    public async Task When_CreateAndDistilleryWithThatNameAlreadyExists_Expect_AlreadyExists()
    {
        var newDistillery = DistilleryTestData.Generic with { Name = "New Distillery" };
        _mockRead.Setup(r => r.ExistsByNameAsync("New Distillery", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.CreateAsync(newDistillery);

        Assert.Multiple(
            () => Assert
                .Equal(CreateDistilleryOutcome.AlreadyExists, result.Outcome),
            () => _mockWrite
                .Verify(w => w.AddAsync(It.IsAny<Distillery>(), It.IsAny<CancellationToken>()), Times.Never));
    }

    [Fact]
    public async Task When_CreateAndDistilleryCreated_Expect_Created()
    {
        var newDistillery = DistilleryTestData.Generic with { Name = "New Distillery" };
        var savedDistillery = DistilleryTestData.Generic with { Id = Guid.NewGuid(), Name = "New Distillery" };

        _mockRead.Setup(r => r.ExistsByNameAsync(newDistillery.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockWrite.Setup(w => w.AddAsync(newDistillery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(savedDistillery);

        var result = await _service.CreateAsync(newDistillery);

        Assert.Multiple(
            () => Assert.Equal(CreateDistilleryOutcome.Created, result.Outcome),
            () => Assert.Equal(savedDistillery, result.Distillery));
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

        Assert.Multiple(
            () => Assert.Equal(CreateDistilleryOutcome.Error, result.Outcome),
            () => Assert.Equal("Exception", result.Error),
            () => Assert.Equal(LogLevel.Error, _fakeLogger.Collector.LatestRecord.Level),
            () => Assert.Equal(
                $"Error creating distillery with [Name: {newDistillery.Name}]",
                _fakeLogger.Collector.LatestRecord.Message));
    }

    [Fact]
    public async Task When_UpdateAndDistilleryNotFound_Expect_NotFound()
    {
        var id = Guid.NewGuid();
        var updatedDistillery = DistilleryTestData.Generic with { Id = id, Name = "Updated Distillery" };
        _mockRead.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Distillery?)null);

        var result = await _service.UpdateAsync(id, updatedDistillery);

        Assert.Equal(UpdateDistilleryOutcome.NotFound, result.Outcome);
    }

    [Fact]
    public async Task When_UpdateAndDistilleryNameChangedToExistingDistilleryName_Expect_NameConflict()
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
    }

    [Fact]
    public async Task When_UpdateAndDistilleryUpdated_Expect_Updated()
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

        Assert.Equal(UpdateDistilleryOutcome.Updated, result.Outcome);
    }

    [Fact]
    public async Task When_UpdateAndDistilleryDeletedAfterLookup_Expect_NotFound()
    {
        var id = Guid.NewGuid();
        var currentDistillery = DistilleryTestData.Generic with { Id = id, Name = "Current Distillery" };
        var updatedDistillery = DistilleryTestData.Generic with { Id = id, Name = "Updated Distillery" };
        _mockRead.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentDistillery);
        _mockWrite.Setup(w => w.UpdateAsync(id, updatedDistillery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.UpdateAsync(id, updatedDistillery);

        Assert.Equal(UpdateDistilleryOutcome.NotFound, result.Outcome);
    }

    [Fact]
    public async Task When_UpdateAndExceptionIsThrown_Expect_ErrorAndLogError()
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
            () => Assert.Equal(LogLevel.Error, _fakeLogger.Collector.LatestRecord.Level),
            () => Assert.Equal(
                $"Error updating distillery [Name: {updatedDistillery.Name}, Id: {id}]",
                _fakeLogger.Collector.LatestRecord.Message));
    }

    [Fact]
    public async Task When_DeleteAndDistilleryFound_Expect_Deleted()
    {
        var id = Guid.NewGuid();
        _mockWrite.Setup(w => w.DeleteAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _service.DeleteAsync(id);

        Assert.Equal(DeleteDistilleryOutcome.Deleted, result.Outcome);
    }

    [Fact]
    public async Task When_DeleteAndDistilleryNotFound_Expect_NotFound()
    {
        var id = Guid.NewGuid();
        _mockWrite.Setup(w => w.DeleteAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _service.DeleteAsync(id);

        Assert.Equal(DeleteDistilleryOutcome.NotFound, result.Outcome);
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
            () => Assert.Equal(LogLevel.Error, _fakeLogger.Collector.LatestRecord.Level),
            () => Assert.Equal($"Error deleting distillery [Id: {id}]", _fakeLogger.Collector.LatestRecord.Message));
    }
}