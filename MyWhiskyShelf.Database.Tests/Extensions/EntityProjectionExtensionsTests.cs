using MyWhiskyShelf.Database.Extensions;
using MyWhiskyShelf.Database.Models;
using MyWhiskyShelf.Models;

namespace MyWhiskyShelf.Database.Tests.Extensions;

public class EntityProjectionExtensionsTests
{
    [Fact]
    public void When_ProjectingDistilleryEntityToDistillery_Expect_AllValuesCorrectlyMapped()
    {
        var distillery = new Distillery
        {
            DistilleryName = "testName",
            Location = "testLocation",
            Region = "testRegion",
            Founded = 2025,
            Owner = "testOwner",
            DistilleryType = "testDistilleryType",
            Active = true
        };

        var expectedDistilleryEntity = new DistilleryEntity
        {
            Id = Guid.Empty,
            DistilleryName = "testName",
            Location = "testLocation",
            Region = "testRegion",
            Founded = 2025,
            Owner = "testOwner",
            DistilleryType = "testDistilleryType",
            Active = true
        };

        var distilleryEntity = distillery.ProjectToDistilleryEntity();
        
        Assert.Equal(expectedDistilleryEntity, distilleryEntity);
    }
    
    [Fact]
    public void When_ProjectingDistilleryToDistilleryEntity_Expect_AllValuesCorrectlyMapped()
    {
        var distilleryEntity = new DistilleryEntity
        {
            Id = Guid.NewGuid(),
            DistilleryName = "testName",
            Location = "testLocation",
            Region = "testRegion",
            Founded = 2025,
            Owner = "testOwner",
            DistilleryType = "testDistilleryType",
            Active = true
        };

        var expectedDistillery = new Distillery
        {
            DistilleryName = "testName",
            Location = "testLocation",
            Region = "testRegion",
            Founded = 2025,
            Owner = "testOwner",
            DistilleryType = "testDistilleryType",
            Active = true
        };

        var distillery = distilleryEntity.ProjectToDistillery();
        
        Assert.Equal(expectedDistillery, distillery);
    }
}