using MyWhiskyShelf.Database.Mappers;
using MyWhiskyShelf.Database.Tests.Resources;
using MyWhiskyShelf.TestHelpers.Data;

namespace MyWhiskyShelf.Database.Tests.Mappers;

public class DistilleryRequestToEntityMapperTests
{
    [Fact]
    public void When_MappingToEntity_Expect_DistilleryRequestWithExpectedValues()
    {
        var distilleryMapper = new DistilleryRequestToEntityMapper();

        var mappedDistilleryEntity = distilleryMapper.Map(DistilleryRequestTestData.Aberargie);

        //the mapped entity has not been added to the database so will not yet have generated an entityId
        Assert.Equal(DistilleryEntityTestData.Aberargie with { Id = Guid.Empty }, mappedDistilleryEntity);
    }
}