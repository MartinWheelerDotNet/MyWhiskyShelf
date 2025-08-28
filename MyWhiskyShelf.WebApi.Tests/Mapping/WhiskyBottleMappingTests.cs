using MyWhiskyShelf.Core.Enums;
using MyWhiskyShelf.WebApi.Mapping;
using MyWhiskyShelf.WebApi.Tests.TestData;

namespace MyWhiskyShelf.WebApi.Tests.Mapping;

public class WhiskyBottleMappingTests
{
    [Fact]
    public void When_MappingWhiskyBottleToResponseWithAllFields_Expect_WhiskyBottleResponseMatches()
    {
        var id = Guid.NewGuid();
        var domain = WhiskyBottleTestData.Generic with { Id = id };

        var response = domain.ToResponse();

        Assert.Multiple(
            () => Assert.Equal(domain.Id, response.Id),
            () => Assert.Equal(domain.Name, response.Name),
            () => Assert.Equal(domain.DistilleryName, response.DistilleryName),
            () => Assert.Equal(domain.DistilleryId, response.DistilleryId),
            () => Assert.Equal(domain.Status.ToString(), response.Status),
            () => Assert.Equal(domain.Bottler, response.Bottler),
            () => Assert.Equal(domain.YearBottled, response.YearBottled),
            () => Assert.Equal(domain.BatchNumber, response.BatchNumber),
            () => Assert.Equal(domain.CaskNumber, response.CaskNumber),
            () => Assert.Equal(domain.AbvPercentage, response.AbvPercentage),
            () => Assert.Equal(domain.VolumeCl, response.VolumeCl),
            () => Assert.Equal(domain.VolumeRemainingCl, response.VolumeRemainingCl),
            () => Assert.Equal(domain.AddedColouring, response.AddedColouring),
            () => Assert.Equal(domain.ChillFiltered, response.ChillFiltered),
            () => Assert.Equal(domain.FlavourProfile, response.FlavourProfile));
    }

    [Fact]
    public void When_MappingWhiskyBottleCreateRequestToDomainWithAllFields_Expect_WhiskyBottleMatchesAndStatusParsed()
    {
        var createWhiskyBottleRequest = WhiskyBottleRequestTestData.GenericCreateRequest;

        var whiskyBottle = createWhiskyBottleRequest.ToDomain();

        Assert.Multiple(
            () => Assert.Equal(createWhiskyBottleRequest.Name, whiskyBottle.Name),
            () => Assert.Equal(createWhiskyBottleRequest.DistilleryName, whiskyBottle.DistilleryName),
            () => Assert.Equal(createWhiskyBottleRequest.DistilleryId, whiskyBottle.DistilleryId),
            () => Assert.Equal(Enum.Parse<BottleStatus>(createWhiskyBottleRequest.Status!), whiskyBottle.Status),
            () => Assert.Equal(createWhiskyBottleRequest.Bottler, whiskyBottle.Bottler),
            () => Assert.Equal(createWhiskyBottleRequest.YearBottled, whiskyBottle.YearBottled),
            () => Assert.Equal(createWhiskyBottleRequest.BatchNumber, whiskyBottle.BatchNumber),
            () => Assert.Equal(createWhiskyBottleRequest.CaskNumber, whiskyBottle.CaskNumber),
            () => Assert.Equal(createWhiskyBottleRequest.AbvPercentage, whiskyBottle.AbvPercentage),
            () => Assert.Equal(createWhiskyBottleRequest.VolumeCl, whiskyBottle.VolumeCl),
            () => Assert.Equal(createWhiskyBottleRequest.VolumeRemainingCl, whiskyBottle.VolumeRemainingCl),
            () => Assert.Equal(createWhiskyBottleRequest.AddedColouring, whiskyBottle.AddedColouring),
            () => Assert.Equal(createWhiskyBottleRequest.ChillFiltered, whiskyBottle.ChillFiltered),
            () => Assert.Equal(createWhiskyBottleRequest.FlavourProfile, whiskyBottle.FlavourProfile));
    }

    [Fact]
    public void When_MappingWhiskyBottleCreateRequestToDomainWithUnknownStatus_Expect_StatusUnknown()
    {
        var whiskyBottleCreateRequest = WhiskyBottleRequestTestData.GenericCreateRequest with
        {
            Status = "Not A Real Status"
        };

        var whiskyBottle = whiskyBottleCreateRequest.ToDomain();

        Assert.Equal(BottleStatus.Unknown, whiskyBottle.Status);
    }

    [Fact]
    public void When_MappingWhiskyBottleCreateRequestToDomainWithoutRemainingVolume_Expect_RemainingEqualsVolume()
    {
        var whiskyBottleCreateRequest = WhiskyBottleRequestTestData.GenericCreateRequest with
        {
            VolumeCl = 50, 
            VolumeRemainingCl = null
        };

        var whiskyBottle = whiskyBottleCreateRequest.ToDomain();

        Assert.Multiple(
            () => Assert.Equal(50, whiskyBottle.VolumeCl),
            () => Assert.Equal(50, whiskyBottle.VolumeRemainingCl));
    }

    [Fact]
    public void When_MappingWhiskyBottleUpdateRequestToDomainWithAllFields_Expect_WhiskyBottleMatchesAndStatusParsed()
    {
        var whiskyBottleUpdateRequest = WhiskyBottleRequestTestData.GenericUpdateRequest;

        var whiskyBottle = whiskyBottleUpdateRequest.ToDomain();

        Assert.Multiple(
            () => Assert.Equal(whiskyBottleUpdateRequest.Name, whiskyBottle.Name),
            () => Assert.Equal(whiskyBottleUpdateRequest.DistilleryName, whiskyBottle.DistilleryName),
            () => Assert.Equal(whiskyBottleUpdateRequest.DistilleryId, whiskyBottle.DistilleryId),
            () => Assert.Equal(Enum.Parse<BottleStatus>(whiskyBottleUpdateRequest.Status!), whiskyBottle.Status),
            () => Assert.Equal(whiskyBottleUpdateRequest.Bottler, whiskyBottle.Bottler),
            () => Assert.Equal(whiskyBottleUpdateRequest.YearBottled, whiskyBottle.YearBottled),
            () => Assert.Equal(whiskyBottleUpdateRequest.BatchNumber, whiskyBottle.BatchNumber),
            () => Assert.Equal(whiskyBottleUpdateRequest.CaskNumber, whiskyBottle.CaskNumber),
            () => Assert.Equal(whiskyBottleUpdateRequest.AbvPercentage, whiskyBottle.AbvPercentage),
            () => Assert.Equal(whiskyBottleUpdateRequest.VolumeCl, whiskyBottle.VolumeCl),
            () => Assert.Equal(whiskyBottleUpdateRequest.VolumeRemainingCl, whiskyBottle.VolumeRemainingCl),
            () => Assert.Equal(whiskyBottleUpdateRequest.AddedColouring, whiskyBottle.AddedColouring),
            () => Assert.Equal(whiskyBottleUpdateRequest.ChillFiltered, whiskyBottle.ChillFiltered),
            () => Assert.Equal(whiskyBottleUpdateRequest.FlavourProfile, whiskyBottle.FlavourProfile));
    }

    [Fact]
    public void When_MappingWhiskyBottleUpdateRequestToDomainWithUnknownStatus_Expect_StatusUnknown()
    {
        var whiskyBottleUpdateRequest = WhiskyBottleRequestTestData.GenericUpdateRequest with
        {
            Status = "Not A Real Status"
        };

        var domain = whiskyBottleUpdateRequest.ToDomain();

        Assert.Equal(BottleStatus.Unknown, domain.Status);
    }

    [Fact]
    public void When_MappingWhiskyBottleUpdateRequestToDomainWithoutRemainingVolume_Expect_RemainingEqualsVolume()
    {
        var req = WhiskyBottleRequestTestData.GenericUpdateRequest with
        {
            VolumeCl = 50,
            VolumeRemainingCl = null
        };

        var domain = req.ToDomain();

        Assert.Multiple(
            () => Assert.Equal(50, domain.VolumeCl),
            () => Assert.Equal(50, domain.VolumeRemainingCl)
        );
    }
}