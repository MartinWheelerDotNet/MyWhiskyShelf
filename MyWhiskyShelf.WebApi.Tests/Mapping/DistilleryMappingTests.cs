using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.WebApi.Mapping;
using MyWhiskyShelf.WebApi.Tests.TestData;

namespace MyWhiskyShelf.WebApi.Tests.Mapping;

public class DistilleryMappingTests
{
    [Fact]
    public void When_MappingDistilleryToResponseWithAllFields_Expect_DistilleryResponseMatches()
    {
        var id = Guid.NewGuid();
        var distillery = DistilleryTestData.Generic with { Id = id };

        var response = distillery.ToResponse();

        Assert.Multiple(
            () => Assert.Equal(distillery.Id, response.Id),
            () => Assert.Equal(distillery.Name, response.Name),
            () => Assert.Equal(distillery.Country, response.Country),
            () => Assert.Equal(distillery.Region, response.Region),
            () => Assert.Equal(distillery.Founded, response.Founded),
            () => Assert.Equal(distillery.Owner, response.Owner),
            () => Assert.Equal(distillery.Type, response.Type),
            () => Assert.Equal(distillery.FlavourProfile, response.FlavourProfile),
            () => Assert.Equal(distillery.Active, response.Active));
    }

    [Fact]
    public void When_MappingDistilleryCreateRequestToDomainWithAllFields_Expect_DistilleryMatchesAndIdDefault()
    {
        var distilleryRequest = DistilleryRequestTestData.GenericCreateRequest;

        var distillery = distilleryRequest.ToDomain();

        Assert.Multiple(
            () => Assert.Equal(Guid.Empty, distillery.Id),
            () => Assert.Equal(distilleryRequest.Name, distillery.Name),
            () => Assert.Equal(distilleryRequest.Country, distillery.Country),
            () => Assert.Equal(distilleryRequest.Region, distillery.Region),
            () => Assert.Equal(distilleryRequest.Founded, distillery.Founded),
            () => Assert.Equal(distilleryRequest.Owner, distillery.Owner),
            () => Assert.Equal(distilleryRequest.Type, distillery.Type),
            () => Assert.Equal(distilleryRequest.FlavourProfile, distillery.FlavourProfile),
            () => Assert.Equal(distilleryRequest.Active, distillery.Active));
    }

    [Fact]
    public void When_MappingDistilleryCreateRequestToDomainWithoutFlavourProfile_Expect_EmptyFlavourProfile()
    {
        var distilleryRequest = DistilleryRequestTestData.GenericCreateRequest with { FlavourProfile = null };

        var distillery = distilleryRequest.ToDomain();

        Assert.Multiple(
            () => Assert.NotNull(distillery.FlavourProfile),
            () => Assert.Equal(new FlavourProfile(), distillery.FlavourProfile));
    }

    [Fact]
    public void When_DistilleryUpdateRequestToDomainWithAllFieldsAndRouteId_Expect_DistilleryMatchesAndIdSetFromRoute()
    {
        var id = Guid.NewGuid();
        var distilleryUpdateRequest = DistilleryRequestTestData.GenericUpdateRequest;

        var distillery = distilleryUpdateRequest.ToDomain(id);

        Assert.Multiple(
            () => Assert.Equal(id, distillery.Id),
            () => Assert.Equal(distilleryUpdateRequest.Name, distillery.Name),
            () => Assert.Equal(distilleryUpdateRequest.Country, distillery.Country),
            () => Assert.Equal(distilleryUpdateRequest.Region, distillery.Region),
            () => Assert.Equal(distilleryUpdateRequest.Founded, distillery.Founded),
            () => Assert.Equal(distilleryUpdateRequest.Owner, distillery.Owner),
            () => Assert.Equal(distilleryUpdateRequest.Type, distillery.Type),
            () => Assert.Equal(distilleryUpdateRequest.FlavourProfile, distillery.FlavourProfile),
            () => Assert.Equal(distilleryUpdateRequest.Active, distillery.Active));
    }

    [Fact]
    public void When_MappingDistilleryUpdateRequestToDomainWithoutFlavourProfile_Expect_EmptyFlavourProfile()
    {
        var id = Guid.NewGuid();
        var distilleryUpdateRequest = DistilleryRequestTestData.GenericUpdateRequest with { FlavourProfile = null };

        var distillery = distilleryUpdateRequest.ToDomain(id);

        Assert.Multiple(
            () => Assert.NotNull(distillery.FlavourProfile),
            () => Assert.Equal(new FlavourProfile(), distillery.FlavourProfile));
    }
}