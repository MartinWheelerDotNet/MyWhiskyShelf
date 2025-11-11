using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Application.Results.Brands;
using MyWhiskyShelf.Application.Services;
using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Tests.Services;

public class BrandAppServiceTests
{
    private readonly FakeLogger<BrandAppService> _fakeLogger = new();
    private readonly Mock<IBrandReadRepository> _readMock = new();
    private readonly BrandAppService _service;
    public BrandAppServiceTests()
    {
        _service = new BrandAppService(_readMock.Object, _fakeLogger);
    }
    
    [Fact]
    public async Task When_GetBrandsAndBrandsFound_Expect_SuccessOutcomeWithListOfBrands()
    {
        List<Brand> expectedBrands =
        [
            new()
            {
                Id = Guid.NewGuid(), 
                Name = "First Name",
                Description = "First Description",
                CountryId = Guid.NewGuid(),CountryName = "CountryName"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Second Name",
                Description = "Second Description",
                CountryId = Guid.NewGuid(),
                CountryName = "CountryName"
            }
        ];
        _readMock.Setup(read => read.GetBrands(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBrands);

        var result = await _service.GetBrandsAsync();

        Assert.Multiple(
            () => Assert.Equal(GetBrandsOutcome.Success, result.Outcome),
            () => Assert.Equal(expectedBrands, result.Brands));
    }
    
        
    [Fact]
    public async Task When_GetBrandsAndNoBrandsFound_Expect_SuccessOutcomeWithEmptyList()
    {
        _readMock.Setup(read => read.GetBrands(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        
        var result = await _service.GetBrandsAsync();

        Assert.Multiple(
            () => Assert.Equal(GetBrandsOutcome.Success, result.Outcome),
            () => Assert.Equal([], result.Brands));
    }
    
        
    [Fact]
    public async Task When_GetBrandsAndAnErrorOccurs_Expect_ErrorOutcomeWithErrorLogged()
    {
        _readMock.Setup(read => read.GetBrands(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Exception"));

        var result = await _service.GetBrandsAsync();

        Assert.Multiple(
            () => Assert.Equal(GetBrandsOutcome.Error, result.Outcome),
            () => Assert.Equal("Exception", result.Error),
            () => Assert.Equal(LogLevel.Error, _fakeLogger.Collector.LatestRecord.Level),
            () => Assert.Equal("An error occured when retrieving brands", _fakeLogger.Collector.LatestRecord.Message));
    }
}