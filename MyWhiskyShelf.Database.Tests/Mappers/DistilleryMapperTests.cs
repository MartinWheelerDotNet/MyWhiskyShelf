using MyWhiskyShelf.Database.Mappers;
using MyWhiskyShelf.Database.Tests.Resources;
using MyWhiskyShelf.TestHelpers.Data;

namespace MyWhiskyShelf.Database.Tests.Mappers;

public class DistilleryMapperTests
{
    [Fact]
    public void When_MappingToEntity_Expect_DistilleryIsMappedToDistilleryEntityWithExpectedValues()
    {
        var distilleryMapper = new DistilleryMapper();

        var mappedDistilleryEntity = distilleryMapper.MapToEntity(DistilleryTestData.Aberargie);
        
        //the mapped entity has not been added to the database so will not yet have generated an entityId
        Assert.Equal(DistilleryEntityTestData.Aberargie with { Id = Guid.Empty }, mappedDistilleryEntity); 
    }

    [Fact]
    public void When_MappingToDomain_Expect_DistilleryEntityIsMappedToDistilleryWithExpectedValues()
    {
        var distilleryMapper = new DistilleryMapper();

        var mappedDistillery = distilleryMapper.MapToDomain(DistilleryEntityTestData.Aberargie);
        
        Assert.Equal(DistilleryTestData.Aberargie, mappedDistillery);
    }

    [Fact]
    public void When_MappingToEntityAndBackToDomain_Expect_BothDistilleryModelsAreTheSame()
    {
        var distilleryMapper = new DistilleryMapper();

        var mappedDistilleryEntity = distilleryMapper.MapToEntity(DistilleryTestData.Aberargie);
        var mappedDistillery = distilleryMapper.MapToDomain(mappedDistilleryEntity);
        
        Assert.Equal(DistilleryTestData.Aberargie, mappedDistillery); 
    }
    
    [Fact]
    public void When_MappingToDomainAndBackToEntity_Expect_BothDistilleryEntityModelsAreTheSame()
    {
        var distilleryMapper = new DistilleryMapper();

        var mappedDistillery = distilleryMapper.MapToDomain(DistilleryEntityTestData.Aberargie);
        var mappedDistilleryEntity = distilleryMapper.MapToEntity(mappedDistillery);
        
        //the mapped entity has not been added to the database so will not yet have generated an entityId
        Assert.Equal(DistilleryEntityTestData.Aberargie with { Id = Guid.Empty }, mappedDistilleryEntity); 
    }
}