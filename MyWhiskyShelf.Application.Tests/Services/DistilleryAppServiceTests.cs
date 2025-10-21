using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using MyWhiskyShelf.Application.Abstractions.Cursor;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Application.Cursors;
using MyWhiskyShelf.Application.Results.Distillery;
using MyWhiskyShelf.Application.Services;
using MyWhiskyShelf.Application.Tests.TestData;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Application.Tests.Services;

public class DistilleryAppServiceTests
{
    private readonly FakeLogger<DistilleryAppService> _fakeLogger = new();
    private readonly Mock<ICursorCodec> _mockCursorCodec = new();
    private readonly Mock<IDistilleryReadRepository> _mockRead = new();
    private readonly Mock<IDistilleryWriteRepository> _mockWrite = new();
    private readonly Mock<IGeoReadRepository> _mockGeoRead = new();
    private readonly DistilleryAppService _service;

    public DistilleryAppServiceTests()
    {
        _service = new DistilleryAppService(
            _mockRead.Object,
            _mockWrite.Object,
            _mockGeoRead.Object,
            _mockCursorCodec.Object,
            _fakeLogger);
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
            () => Assert.Equal(DistilleryTestData.Generic with { Id = id }, result.Distillery));
    }

    [Fact]
    public async Task When_GetByIdAndErrorOccurs_Expect_ErrorWithExceptionMessage()
    {
        var id = Guid.NewGuid();
        _mockRead.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Exception"));

        var result = await _service.GetByIdAsync(id);

        Assert.Multiple(
            () => Assert.Equal(GetDistilleryByIdOutcome.Error, result.Outcome),
            () => Assert.Equal("Exception", result.Error),
            () => Assert.Equal(LogLevel.Error, _fakeLogger.Collector.LatestRecord.Level),
            () => Assert.Equal($"Error retrieving distillery with [Id: {id}]", _fakeLogger.Collector.LatestRecord.Message));
    }

    [Fact]
    public async Task When_GetAllAndTwoDistilleriesExist_Expect_SuccessWithListOfTwoDistilleries()
    {
        const int amount = 10;
        var afterCursor = string.Empty;
        List<Distillery> expectedDistilleries =
        [
            DistilleryTestData.Generic with { Name = "First Distillery" },
            DistilleryTestData.Generic with { Name = "Second Distillery" }
        ];
        var emptyDistilleryFilterOptions = new DistilleryFilterOptions();
        _mockRead.Setup(r => r.SearchByFilter(emptyDistilleryFilterOptions, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDistilleries);

        var result = await _service.GetAllAsync(amount, afterCursor);

        Assert.Multiple(
            () => Assert.Equal(GetAllDistilleriesOutcome.Success, result.Outcome),
            () => Assert.Equal(expectedDistilleries, result.Distilleries),
            () => Assert.Null(result.NextCursor),
            () => Assert.Equal(amount, result.Amount)
        );

        _mockCursorCodec
            .Verify(c => c.TryDecode(It.IsAny<string>(), out It.Ref<DistilleryQueryCursor?>.IsAny), Times.Never);
        _mockCursorCodec
            .Verify(c => c.Encode(It.IsAny<DistilleryQueryCursor>()), Times.Never);
    }

    [Fact]
    public async Task When_GetAllAndNoDistilleriesExist_Expect_SuccessWithEmptyList()
    {
        const int amount = 10;
        _mockRead.Setup(r => r.SearchByFilter(It.IsAny<DistilleryFilterOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _service.GetAllAsync(amount, string.Empty);

        Assert.Multiple(
            () => Assert.Equal(GetAllDistilleriesOutcome.Success, result.Outcome),
            () => Assert.Empty(result.Distilleries!),
            () => Assert.Null(result.NextCursor),
            () => Assert.Equal(amount, result.Amount)
        );
    }

    [Fact]
    public async Task When_GetAllAndErrorOccurs_Expect_ErrorWithMessage()
    {
        const int amount = 10;
        _mockRead.Setup(r => r.SearchByFilter(It.IsAny<DistilleryFilterOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Exception"));

        var result = await _service.GetAllAsync(amount, afterCursor: null!);

        Assert.Multiple(
            () => Assert.Equal(GetAllDistilleriesOutcome.Error, result.Outcome),
            () => Assert.Equal("Exception", result.Error),
            () => Assert.Equal(LogLevel.Error, _fakeLogger.Collector.LatestRecord.Level),
            () => Assert.Equal("An error occurred whilst retrieving all distilleries", _fakeLogger.Collector.LatestRecord.Message));
    }

    [Fact]
    public async Task When_GetAllAndAmountIsZero_Expect_SuccessWithEmptyListAndNoRepositoryCall()
    {
        const int amount = 0;

        var result = await _service.GetAllAsync(amount, afterCursor: null!);

        Assert.Multiple(
            () => Assert.Equal(GetAllDistilleriesOutcome.Success, result.Outcome),
            () => Assert.Empty(result.Distilleries!),
            () => Assert.Null(result.NextCursor),
            () => Assert.Equal(0, result.Amount)
        );

        _mockRead.Verify(
            r => r.SearchByFilter(It.IsAny<DistilleryFilterOptions>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _mockCursorCodec
            .Verify(c => c.TryDecode(It.IsAny<string>(), out It.Ref<DistilleryQueryCursor?>.IsAny), Times.Never);
    }

    [Fact]
    public async Task When_GetAllWithInvalidCursor_Expect_InvalidCursorWithMessageAndNoRepositoryCall()
    {
        const int amount = 10;
        const string afterCursor = "badCursor";
        DistilleryQueryCursor? distilleryQueryCursor;
        _mockCursorCodec.Setup(c => c.TryDecode(afterCursor, out distilleryQueryCursor))
            .Returns(false);

        var result = await _service.GetAllAsync(amount, afterCursor);

        Assert.Multiple(
            () => Assert.Equal(GetAllDistilleriesOutcome.InvalidCursor, result.Outcome),
            () => Assert.Equal("Invalid cursor provided", result.Error));

        _mockRead.Verify(
            r => r.SearchByFilter(It.IsAny<DistilleryFilterOptions>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task When_GetAllWithFullPage_Expect_NextCursorReturned()
    {
        const int amount = 2;
        var firstDistillery = DistilleryTestData.Generic with { Name = "First Distillery", Id = Guid.NewGuid() };
        var secondDistillery = DistilleryTestData.Generic with { Name = "Second Distillery", Id = Guid.NewGuid() };
        var distilleryFilterOptionsWithAmountOnly = new DistilleryFilterOptions { Amount = amount };
        var expectedDistilleryQueryCursor = new DistilleryQueryCursor(secondDistillery.Name, null, null, null);
        List<Distillery> expectedDistilleries = [firstDistillery, secondDistillery];
        _mockRead.Setup(r => r.SearchByFilter(distilleryFilterOptionsWithAmountOnly, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDistilleries);
        _mockCursorCodec.Setup(c => c.Encode(It.IsAny<DistilleryQueryCursor>()))
            .Returns("NextCursor");

        var result = await _service.GetAllAsync(amount, afterCursor: null!);

        Assert.Multiple(
            () => Assert.Equal(GetAllDistilleriesOutcome.Success, result.Outcome),
            () => Assert.Equal(expectedDistilleries, result.Distilleries),
            () => Assert.Equal("NextCursor", result.NextCursor),
            () => Assert.Equal(amount, result.Amount)
        );

        _mockCursorCodec.Verify(c => c.Encode(expectedDistilleryQueryCursor), Times.Once);
    }

    [Fact]
    public async Task When_GetAllWithValidCursor_Expect_PassedToRepository()
    {
        const int amount = 5;
        const string afterCursor = "afterCursor";
        var expectedDistilleryFilterOptions = new DistilleryFilterOptions { AfterName = "Distillery", Amount = amount };
        var distilleryQueryCursor = new DistilleryQueryCursor("Distillery", null, null, null);
        _mockCursorCodec
            .Setup(c => c.TryDecode(afterCursor, out distilleryQueryCursor))
            .Returns(true);
        _mockRead
            .Setup(r => r.SearchByFilter(expectedDistilleryFilterOptions, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _service.GetAllAsync(amount, afterCursor);

        Assert.Multiple(
            () => Assert.Equal(GetAllDistilleriesOutcome.Success, result.Outcome),
            () => Assert.Empty(result.Distilleries!),
            () => Assert.Null(result.NextCursor),
            () => Assert.Equal(amount, result.Amount)
        );

        _mockRead.Verify(
            r => r.SearchByFilter(expectedDistilleryFilterOptions, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task When_CreateAndDistilleryWithThatNameAlreadyExists_Expect_AlreadyExists()
    {
        var distillery = DistilleryTestData.Generic with { Name = "New Distillery" };

        _mockRead.Setup(r => r.ExistsByNameAsync("New Distillery", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.CreateAsync(distillery);

        Assert.Equal(CreateDistilleryOutcome.AlreadyExists, result.Outcome);
        
        _mockWrite
            .Verify(w => w.AddAsync(It.IsAny<Distillery>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockGeoRead
            .Verify(g => g.CountryExistsByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task When_CreateAndCountryDoesNotExist_Expect_CountryDoesNotExist()
    {
        var distillery = DistilleryTestData.Generic with
        {
            Name = "New Distillery",
            CountryId = Guid.NewGuid(),
            RegionId = null
        };
        _mockRead.Setup(r => r.ExistsByNameAsync(distillery.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockGeoRead.Setup(g => g.CountryExistsByIdAsync(distillery.CountryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.CreateAsync(distillery);

        Assert.Equal(CreateDistilleryOutcome.CountryDoesNotExist, result.Outcome);

        _mockWrite.Verify(w => w.AddAsync(It.IsAny<Distillery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task When_CreateAndRegionNotInCountry_Expect_RegionDoesNotExistInCountry()
    {
        var countryId = Guid.NewGuid();
        var regionId = Guid.NewGuid();
        var distillery = DistilleryTestData.Generic with
        {
            Name = "New Distillery",
            CountryId = countryId,
            RegionId = regionId
        };
        _mockRead.Setup(r => r.ExistsByNameAsync(distillery.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockGeoRead.Setup(g => g.CountryExistsByIdAsync(countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockGeoRead.Setup(g => g.GetRegionByIdAsync(regionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Region
            {
                Id = regionId,
                CountryId = Guid.NewGuid(),
                Name = "Other Country Region",
                Slug = "other-country-region",
                IsActive = false
            });

        var result = await _service.CreateAsync(distillery);
        
        Assert.Equal(CreateDistilleryOutcome.RegionDoesNotExistInCountry, result.Outcome);
        _mockWrite.Verify(w => w.AddAsync(It.IsAny<Distillery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task When_CreateAndDistilleryCreated_Expect_Created()
    {
        var countryId = Guid.NewGuid();
        var distillery = DistilleryTestData.Generic with
        {
            Name = "New Distillery",
            CountryId = countryId,
            RegionId = null
        };
        var savedDistillery = distillery with { Id = Guid.NewGuid() };
        _mockRead.Setup(r => r.ExistsByNameAsync(distillery.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockGeoRead.Setup(g => g.CountryExistsByIdAsync(countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockWrite.Setup(w => w.AddAsync(distillery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(savedDistillery);

        var result = await _service.CreateAsync(distillery);

        Assert.Multiple(
            () => Assert.Equal(CreateDistilleryOutcome.Created, result.Outcome),
            () => Assert.Equal(savedDistillery, result.Distillery));
    }

    [Fact]
    public async Task When_CreateAndExceptionIsThrown_Expect_ErrorAndLogError()
    {
        var countryId = Guid.NewGuid();
        var newDistillery = DistilleryTestData.Generic with
        {
            Name = "New Distillery",
            CountryId = countryId,
            RegionId = null
        };
        _mockRead.Setup(r => r.ExistsByNameAsync(newDistillery.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockGeoRead.Setup(g => g.CountryExistsByIdAsync(countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
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
    public async Task When_UpdateAndCountryDoesNotExist_Expect_CountryDoesNotExist()
    {
        var id = Guid.NewGuid();
        var current = DistilleryTestData.Generic with { Id = id, Name = "Current" };
        var updated = DistilleryTestData.Generic with
        {
            Id = id,
            Name = "Updated",
            CountryId = Guid.NewGuid(),
            RegionId = null
        };
        _mockRead.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(current);
        _mockRead.Setup(r => r.ExistsByNameAsync(updated.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockGeoRead.Setup(g => g.CountryExistsByIdAsync(updated.CountryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.UpdateAsync(id, updated);

        Assert.Equal(UpdateDistilleryOutcome.CountryDoesNotExist, result.Outcome);
    }

    [Fact]
    public async Task When_UpdateAndRegionNotInCountry_Expect_RegionDoesNotExistInCountry()
    {
        var id = Guid.NewGuid();
        var current = DistilleryTestData.Generic with { Id = id, Name = "Current" };
        var countryId = Guid.NewGuid();
        var regionId = Guid.NewGuid();
        var updated = DistilleryTestData.Generic with
        {
            Id = id,
            Name = "Updated",
            CountryId = countryId,
            RegionId = regionId
        };
        _mockRead.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(current);
        _mockRead.Setup(r => r.ExistsByNameAsync(updated.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockGeoRead.Setup(g => g.CountryExistsByIdAsync(countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockGeoRead.Setup(g => g.GetRegionByIdAsync(regionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Region
            {
                Id = regionId,
                CountryId = Guid.NewGuid(),
                Name = "Region",
                Slug = "region",
                IsActive = true
            });

        var result = await _service.UpdateAsync(id, updated);

        Assert.Equal(UpdateDistilleryOutcome.RegionDoesNotExistInCountry, result.Outcome);
    }

    [Fact]
    public async Task When_UpdateAndDistilleryUpdated_Expect_Updated()
    {
        var id = Guid.NewGuid();
        var currentDistillery = DistilleryTestData.Generic with { Id = id, Name = "Current Distillery" };
        var updatedDistillery = DistilleryTestData.Generic with
        {
            Id = id,
            Name = "Updated Distillery",
            CountryId = Guid.NewGuid(),
            RegionId = null
        };
        _mockRead.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentDistillery);
        _mockRead.Setup(r => r.ExistsByNameAsync(updatedDistillery.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockGeoRead.Setup(g => g.CountryExistsByIdAsync(updatedDistillery.CountryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
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
        var updatedDistillery = DistilleryTestData.Generic with
        {
            Id = id,
            Name = "Updated Distillery",
            CountryId = Guid.NewGuid(),
            RegionId = null
        };

        _mockRead.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentDistillery);
        _mockRead.Setup(r => r.ExistsByNameAsync(updatedDistillery.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockGeoRead.Setup(g => g.CountryExistsByIdAsync(updatedDistillery.CountryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
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
        var updatedDistillery = DistilleryTestData.Generic with
        {
            Id = id,
            Name = "Updated Distillery",
            CountryId = Guid.NewGuid(),
            RegionId = null
        };
        _mockRead.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentDistillery);
        _mockRead.Setup(r => r.ExistsByNameAsync(updatedDistillery.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockGeoRead.Setup(g => g.CountryExistsByIdAsync(updatedDistillery.CountryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
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
