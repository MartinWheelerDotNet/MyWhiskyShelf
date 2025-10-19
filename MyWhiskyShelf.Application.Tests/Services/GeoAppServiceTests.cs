using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Application.Results.Geo;
using MyWhiskyShelf.Application.Services;
using MyWhiskyShelf.Application.Tests.TestData;
using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Tests.Services;

public class GeoAppServiceTests
{
    private readonly FakeLogger<GeoAppService> _fakeLogger = new();
    private readonly Mock<IGeoReadRepository> _readMock = new();
    private readonly Mock<IGeoWriteRepository> _writeMock = new();
    private readonly GeoAppService _service;
    public GeoAppServiceTests()
    {
        _service = new GeoAppService(_readMock.Object, _writeMock.Object, _fakeLogger);
    }

    [Fact]
    public async Task When_GetAllAndCountriesFound_Expect_SuccessOutcomeWithCountries()
    {
        List<Country> expectedCountries = [CountryTestData.Generic()];
        _readMock.Setup(read => read.GetAllGeoInformationAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCountries);

        var result = await _service.GetAllAsync();
        
        Assert.Multiple(
            () => Assert.Equal(GetCountryGeoOutcome.Success,result.Outcome),
            () => Assert.Equal(expectedCountries, result.Countries));
    }
    
    [Fact]
    public async Task When_GetAllAndAnErrorOccurs_Expect_ErrorOutcomeWithMessage()
    {
        _readMock.Setup(read => read.GetAllGeoInformationAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Exception"));

        var result = await _service.GetAllAsync();
        
        Assert.Multiple(
            () => Assert.Equal(GetCountryGeoOutcome.Error,result.Outcome),
            () => Assert.Equal("Exception", result.Error));
    }
    
    [Fact]
    public async Task When_CreateCountryAndNameAlreadyExists_Expect_NameConflictOutcome()
    {
        _readMock.Setup(read => read.CountryExistsByNameAsync("name", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.CreateCountryAsync(CountryTestData.Generic() with { Name = "name" });

        Assert.Equal(CreateCountryOutcome.NameConflict, result.Outcome);
    }
    
    [Fact]
    public async Task When_CreateCountryAndSlugAlreadyExists_Expect_EnrichedSlugCreated()
    {
        var country = CountryTestData.Generic() with
        {
            Name = "name",
            Slug = "slug"
        };
        _readMock.Setup(read => read.CountryExistsBySlugAsync("slug", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _service.CreateCountryAsync(country);

        _writeMock.Verify(
            write => write.AddCountryAsync(
                It.Is<Country>(c => c.Slug != country.Slug),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task When_CreateCountryAndCountryIsCreated_Expect_CreatedOutcomeWithCountry()
    {
        var country = CountryTestData.Generic();
        var expectedCountry = CountryTestData.Generic() with { Id = Guid.NewGuid() };
        _writeMock.Setup(write => write.AddCountryAsync(country, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCountry);

        var result = await _service.CreateCountryAsync(country);

        Assert.Multiple(
            () => Assert.Equal(CreateCountryOutcome.Created, result.Outcome),
            () => Assert.Equal(expectedCountry, result.Country));
    }
    
    [Fact]
    public async Task When_CreateCountryAndAnErrorOccurs_Expect_ErrorOutcomeWithMessage()
    {
        const string name = "New Country";
        var country = CountryTestData.Generic() with { Name = name };
        _writeMock.Setup(write => write.AddCountryAsync(country, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Exception"));

        var result = await _service.CreateCountryAsync(country);
        
        Assert.Multiple(
            () => Assert.Equal(CreateCountryOutcome.Error, result.Outcome),
            () => Assert.Equal("Exception", result.Error),
            () => Assert.Equal(LogLevel.Error, _fakeLogger.Collector.LatestRecord.Level),
            () => Assert.Equal(
                $"Error creating country with [Name: {name}]",
                _fakeLogger.Collector.LatestRecord.Message));
    }
    
    [Fact]
    public async Task When_UpdateCountryAndCountryDoesNotExist_ExpectNotFoundOutcome()
    {
        var id = Guid.NewGuid();
        var updatedCountry = CountryTestData.Generic() with { Id = id };
        _readMock.Setup(read => read.GetCountryByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Country?) null);
        
        var result = await _service.UpdateCountryAsync(id, updatedCountry);
        
        Assert.Equal(UpdateCountryOutcome.NotFound, result.Outcome);
    }

    [Fact]
    public async Task When_UpdateCountryAndNameHasChangedAndAlreadyExists_Expect_NameConflictOutcome()
    {
        
        var id = Guid.NewGuid();
        var existingCountry = CountryTestData.Generic() with { Id = id, Name = "Original Name" };
        var updatedCountry =  CountryTestData.Generic() with { Id = id, Name = "New Name" };
        _readMock.Setup(read => read.GetCountryByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCountry);
        _readMock.Setup(read => read.CountryExistsByNameAsync("New Name", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var result = await _service.UpdateCountryAsync(id, updatedCountry);
        
        Assert.Equal(UpdateCountryOutcome.NameConflict, result.Outcome);
    }
    
    [Fact]
    public async Task When_UpdateCountryAndSlugHasChangedAndAlreadyExists_Expect_SlugEnriched()
    {
        var id = Guid.NewGuid();
        var existingCountry = CountryTestData.Generic() with { Id = id, Slug = "original-slug" };
        var updatedCountry =  CountryTestData.Generic() with { Id = id, Slug = "updated-slug" };
        _readMock.Setup(read => read.GetCountryByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCountry);
        _readMock.Setup(read => read.CountryExistsBySlugAsync("slug", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        await _service.UpdateCountryAsync(id, updatedCountry);
        
        _writeMock.Verify(
            write => write.UpdateCountryAsync(
                id, 
                It.Is<Country>(c => c.Slug != existingCountry.Slug),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task When_UpdateCountryAndCountryNotFoundOnUpdate_Expect_NotFoundOutcome()
    {
        var id = Guid.NewGuid();
        var existingCountry = CountryTestData.Generic() with { Id = id, Name = "Original Name" };
        var updatedCountry =  CountryTestData.Generic() with { Id = id, Name = "New Name" };
        _readMock.Setup(read => read.GetCountryByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCountry);
        _writeMock.Setup(write => write.UpdateCountryAsync(id, updatedCountry, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        var result = await _service.UpdateCountryAsync(id, updatedCountry);

        Assert.Equal(UpdateCountryOutcome.NotFound, result.Outcome);
    }
    
    [Fact]
    public async Task When_UpdateCountryAndCountryCreated_Expect_SuccessOutcome_WithCountry()
    {
        var id = Guid.NewGuid();
        var existingCountry = CountryTestData.Generic() with { Id = id, Name = "Original Name" };
        var updatedCountry =  CountryTestData.Generic() with { Id = id, Name = "New Name" };
        _readMock.Setup(read => read.GetCountryByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCountry);
        _writeMock.Setup(write => write.UpdateCountryAsync(id, updatedCountry, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var result = await _service.UpdateCountryAsync(id, updatedCountry);

        Assert.Multiple(
            () => Assert.Equal(UpdateCountryOutcome.Updated, result.Outcome),
            () => Assert.Equal(updatedCountry, result.Country));
    }
    
    [Fact]
    public async Task When_UpdateCountryAndAnErrorOccurs_Expect_ErrorOutcomeWithMessage()
    {
        var id = Guid.NewGuid();
        _readMock.Setup(read => read.GetCountryByIdAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Exception"));

        var result = await _service.UpdateCountryAsync(id, CountryTestData.Generic(id));

        Assert.Multiple(
            () => Assert.Equal(UpdateCountryOutcome.Error, result.Outcome),
            () => Assert.Equal("Exception", result.Error),
            () => Assert.Equal(LogLevel.Error, _fakeLogger.Collector.LatestRecord.Level),
            () => Assert.Equal(
                $"Error updating Country with [Id: {id}]",
                _fakeLogger.Collector.LatestRecord.Message));
    }
    
    [Fact]
    public async Task When_SetCountryActiveAndCountryNotFound_Expect_NotFoundOutcome()
    {
        var id = Guid.NewGuid();
        _writeMock.Setup(write => write.SetCountryActiveAsync(id, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.SetCountryActiveAsync(id, true);

        Assert.Equal(SetCountryActiveOutcome.NotFound, result.Outcome);
    }
    
    [Fact]
    public async Task When_SetCountryActiveAndCountryActiveIsSet_Expect_UpdatedOutcome()
    {
        var id = Guid.NewGuid();
        _writeMock.Setup(write => write.SetCountryActiveAsync(id, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.SetCountryActiveAsync(id, false);

        Assert.Equal(SetCountryActiveOutcome.Updated, result.Outcome);
    }
    
    [Fact]
    public async Task When_SetCountryActiveAndErrorOccurs_Expect_ErrorOutcomeWithMessage()
    {
        var id = Guid.NewGuid();
        _writeMock.Setup(write => write.SetCountryActiveAsync(id, false, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Exception"));

        var result = await _service.SetCountryActiveAsync(id, false);

        Assert.Multiple(
            () => Assert.Equal(SetCountryActiveOutcome.Error, result.Outcome),
            () => Assert.Equal("Exception", result.Error),
            () => Assert.Equal(LogLevel.Error, _fakeLogger.Collector.LatestRecord.Level),
            () => Assert.Equal(
                $"Error setting country active flag for [Id: {id}]",
                _fakeLogger.Collector.LatestRecord.Message));
    }
    
    [Fact]
    public async Task When_CreateRegionAndCountryNotFound_Expect_CountryNotFoundOutcome()
    {
        var countryId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var region = RegionTestData.ActiveRegion(countryId, id);
        _readMock.Setup(read => read.CountryExistsByIdAsync(countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        var result = await _service.CreateRegionAsync(countryId, region);
        
        Assert.Equal(CreateRegionOutcome.CountryNotFound, result.Outcome);
    }

    [Fact]
    public async Task When_CreateRegionAndNameAlreadyExistsInCountry_Expect_NameConflictOutcome()
    {
        const string name = "New Region";
        var countryId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var region = RegionTestData.ActiveRegion(countryId, id) with { Name = "New Region" };
        _readMock.Setup(read => read.CountryExistsByIdAsync(countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _readMock.Setup(read =>
                read.RegionExistsByNameAndCountryIdAsync(name, countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var result = await _service.CreateRegionAsync(countryId, region);
        
        Assert.Equal(CreateRegionOutcome.NameConflict, result.Outcome);
    }

    [Fact]
    public async Task When_CreateRegionAndSlugAlreadyExists_Expect_SlugEnriched()
    {
        const string slug = "new-region-slug";
        var countryId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var region = RegionTestData.ActiveRegion(countryId, id) with { Slug = slug };
        _readMock.Setup(read => read.CountryExistsByIdAsync(countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _readMock.Setup(read =>
                read.RegionExistsBySlugAndCountryIdAsync(slug, countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        await _service.CreateRegionAsync(countryId, region);
        
        _writeMock.Verify(
            write => write.AddRegionAsync(
                countryId,
                It.Is<Region>(r => r.Slug != slug),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task When_CreateRegionAndCountryDoesNotExistsOnCreate_Expect_CountryNotFoundOutcome()
    {
        var countryId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var region = RegionTestData.ActiveRegion(countryId, id);
        _readMock.Setup(read => read.CountryExistsByIdAsync(countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _writeMock.Setup(write => write.AddRegionAsync(countryId, region, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Region?) null);
        
        var result = await _service.CreateRegionAsync(countryId, region);
        
        Assert.Equal(CreateRegionOutcome.CountryNotFound, result.Outcome);
    }

    [Fact]
    public async Task When_CreateRegionAndRegionCreated_Expect_CreatedOutcomeWithRegion()
    {
        var countryId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var region = RegionTestData.ActiveRegion(countryId, id);
        _readMock.Setup(read => read.CountryExistsByIdAsync(countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _writeMock.Setup(write => write.AddRegionAsync(countryId, region, It.IsAny<CancellationToken>()))
            .ReturnsAsync(region);
        
        var result = await _service.CreateRegionAsync(countryId, region);
        
        Assert.Multiple(
            () => Assert.Equal(CreateRegionOutcome.Created, result.Outcome),
            () => Assert.Same(region, result.Region));
    }
    
    [Fact]
    public async Task When_CreateRegionAndErrorOccurs_Expect_ErrorOutcomeWithMessage()
    {
        var countryId = Guid.NewGuid();
        var id = Guid.NewGuid();
        const string name = "New Region";
        var region = RegionTestData.ActiveRegion(countryId, id) with { Name = name };
        _readMock.Setup(read => read.CountryExistsByIdAsync(countryId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Exception"));
        
        var result = await _service.CreateRegionAsync(countryId, region);
        
        Assert.Multiple(
            () => Assert.Equal(CreateRegionOutcome.Error, result.Outcome),
            () => Assert.Equal("Exception", result.Error),
            () => Assert.Equal(LogLevel.Error, _fakeLogger.Collector.LatestRecord.Level),
            () => Assert.Equal(
                $"Error creating region [CountryId: {countryId}, Name: {name}]",
                _fakeLogger.Collector.LatestRecord.Message));
    }
    
    [Fact]
    public async Task When_UpdateRegionAndRegionDoesNotExist_Expect_NotFoundOutcome()
    {
        var countryId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var updatedRegion = RegionTestData.ActiveRegion(countryId, id);
        _readMock.Setup(read => read.GetRegionByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Region?) null);
        
        var result = await _service.UpdateRegionAsync(id, updatedRegion);
        
        Assert.Equal(UpdateRegionOutcome.NotFound, result.Outcome);
    }
    
    [Fact]
    public async Task When_UpdateRegionAndCountryHasChanged_Expect_CountryChangeAttemptedOutcome()
    {
        var countryId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var region = RegionTestData.ActiveRegion(countryId, id) ;
        var updatedRegion = RegionTestData.ActiveRegion(countryId, id) with { CountryId = Guid.NewGuid() };
        _readMock.Setup(read => read.GetRegionByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(region);
        
        var result = await _service.UpdateRegionAsync(id, updatedRegion);
        
        Assert.Equal(UpdateRegionOutcome.CountryChangeAttempted, result.Outcome);
    }
    
    [Fact]
    public async Task When_UpdateRegionAndNameAlreadyExistsInCountry_Expect_NameConflictOutcome()
    {
        const string name = "Updated Region";
        var countryId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var region = RegionTestData.ActiveRegion(countryId, id) ;
        var updatedRegion = RegionTestData.ActiveRegion(countryId, id) with { Name = name };
        _readMock.Setup(read => read.GetRegionByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(region);
        _readMock.Setup(read =>
                read.RegionExistsByNameAndCountryIdAsync(name, countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var result = await _service.UpdateRegionAsync(id, updatedRegion);
        
        Assert.Equal(UpdateRegionOutcome.NameConflict, result.Outcome);
    }
    
    [Fact]
    public async Task When_UpdateRegionAndSlugAlreadyExists_Expect_SlugEnriched()
    {
        const string slug = "updated-region-slug";
        var countryId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var region = RegionTestData.ActiveRegion(countryId, id) ;
        var updatedRegion = RegionTestData.ActiveRegion(countryId, id) with { Slug = slug };
        _readMock.Setup(read => read.GetRegionByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(region);
        _readMock.Setup(read =>
                read.RegionExistsBySlugAndCountryIdAsync(slug, countryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        await _service.UpdateRegionAsync(id, updatedRegion);
        
        _writeMock.Verify(
            write => write.UpdateRegionAsync(
                id,
                It.Is<Region>(r => r.Slug != slug),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task When_UpdateRegionAndRegionDoesNotExistsOnCreate_Expect_NotFoundOutcome()
    {
        var countryId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var region = RegionTestData.ActiveRegion(countryId, id);
        var updatedRegion = RegionTestData.ActiveRegion(countryId, id) with { IsActive = false };
        _readMock.Setup(read => read.GetRegionByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(region);
        _writeMock.Setup(write => write.UpdateRegionAsync(id, region, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        var result = await _service.UpdateRegionAsync(id, updatedRegion);
        
        Assert.Equal(UpdateRegionOutcome.NotFound, result.Outcome);
    }
    
    [Fact]
    public async Task When_UpdateRegionAndRegionCreated_Expect_UpdatedOutcomeWithRegion()
    {
        var countryId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var region = RegionTestData.ActiveRegion(countryId, id);
        var updatedRegion = RegionTestData.ActiveRegion(countryId, id) with { IsActive = false };
        _readMock.Setup(read => read.GetRegionByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(region);
        _writeMock.Setup(write => write.UpdateRegionAsync(id, updatedRegion, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var result = await _service.UpdateRegionAsync(id, updatedRegion);
        
        Assert.Multiple(
            () => Assert.Equal(UpdateRegionOutcome.Updated, result.Outcome),
            () => Assert.Same(updatedRegion, result.Region));
    }
    
    [Fact]
    public async Task When_UpdateRegionAndErrorOccurs_Expect_ErrorOutcomeWithMessage()
    {
        var countryId = Guid.NewGuid();
        var id = Guid.NewGuid();
        const string name = "New Region";
        var updatedRegion = RegionTestData.ActiveRegion(countryId, id) with { Name = name };
        _readMock.Setup(read => read.GetRegionByIdAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Exception"));
        
        var result = await _service.UpdateRegionAsync(id, updatedRegion);
        
        Assert.Multiple(
            () => Assert.Equal(UpdateRegionOutcome.Error, result.Outcome),
            () => Assert.Equal("Exception", result.Error),
            () => Assert.Equal(LogLevel.Error, _fakeLogger.Collector.LatestRecord.Level),
            () => Assert.Equal(
                $"Error updating Region with [Id: {id}, Name: {name}]",
                _fakeLogger.Collector.LatestRecord.Message));
    }
    
    [Fact]
    public async Task When_SetRegionActiveAndRegionNotFound_Expect_NotFoundOutcome()
    {
        var id = Guid.NewGuid();
        _writeMock.Setup(write => write.SetRegionActiveAsync(id, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.SetRegionActiveAsync(id, true);

        Assert.Equal(SetRegionActiveOutcome.NotFound, result.Outcome);
    }
    
    [Fact]
    public async Task When_SetRegionActiveAndRegionActiveIsSet_Expect_UpdatedOutcome()
    {
        var id = Guid.NewGuid();
        _writeMock.Setup(write => write.SetRegionActiveAsync(id, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.SetRegionActiveAsync(id, true);

        Assert.Equal(SetRegionActiveOutcome.Updated, result.Outcome);
    }
    
    [Fact]
    public async Task When_SetRegionActiveAndErrorOccurs_Expect_ErrorOutcomeWithMessage()
    {
        var id = Guid.NewGuid();
        _writeMock.Setup(write => write.SetRegionActiveAsync(id, true, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Exception"));

        var result = await _service.SetRegionActiveAsync(id, true);
        Assert.Multiple(
            () => Assert.Equal(SetRegionActiveOutcome.Error, result.Outcome),
            () => Assert.Equal("Exception", result.Error),
            () => Assert.Equal(LogLevel.Error, _fakeLogger.Collector.LatestRecord.Level),
            () => Assert.Equal(
                $"Error setting region active flag for [Id: {id}]",
                _fakeLogger.Collector.LatestRecord.Message));
    }
    
}