using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.WebApi.Mapping;

namespace MyWhiskyShelf.WebApi.Tests.Mapping;

public class DistilleryNameMappingTests
{
    [Fact]
    public void When_MappingDistilleryNameToResponse_Expect_DistilleryNameResponseMatches()
    {
        var distilleryName = new DistilleryName(Guid.NewGuid(), "Name");
        var response = distilleryName.ToResponse();

        Assert.Multiple(
            () => Assert.Equal(distilleryName.Id, response.Id),
            () => Assert.Equal(distilleryName.Name, response.Name));
    }
}