using MyWhiskyShelf.Database.Mappers;
using MyWhiskyShelf.Database.Tests.Resources;
using MyWhiskyShelf.TestHelpers.Data;

namespace MyWhiskyShelf.Database.Tests.Mappers;

public class DistilleryEntityToResponseMapperTests
{
    [Fact]
    public void When_MappingToEntity_Expect_DistilleryIsMappedToDistilleryEntityWithExpectedValues()
    {
        var distilleryMapper = new DistilleryEntityToResponseMapper();

        var mappedDistilleryResponse = distilleryMapper.Map(DistilleryEntityTestData.Aberargie);

        Assert.Equal(DistilleryResponseTestData.Aberargie, mappedDistilleryResponse);
    }
}