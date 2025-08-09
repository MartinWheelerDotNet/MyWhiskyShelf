using MyWhiskyShelf.Database.Mappers;
using MyWhiskyShelf.Database.Tests.Resources;
using MyWhiskyShelf.TestHelpers.Data;

namespace MyWhiskyShelf.Database.Tests.Mappers;

public class DistilleryRequestToEntityMapperTests
{
    [Fact]
    public void When_MappingToEntity_Expect_DistilleryRequestWithExpectedValues()
    {
        //the mapped entity has not been added to the database so will not yet have generated an entityId
        var expectedDistilleryEntity = DistilleryEntityTestData.Aberargie;
        expectedDistilleryEntity.Id = Guid.Empty;

        var distilleryMapper = new DistilleryRequestToEntityMapper();
        var mappedDistilleryEntity = distilleryMapper.Map(DistilleryRequestTestData.Aberargie);

        Assert.Equivalent(expectedDistilleryEntity, mappedDistilleryEntity);
    }
}