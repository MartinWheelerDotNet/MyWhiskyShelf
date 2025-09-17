using MyWhiskyShelf.IntegrationTests.Fixtures;

namespace MyWhiskyShelf.IntegrationTests.Collections;

[CollectionDefinition("DataSeedingTestCollection", DisableParallelization = true)]
public class DataNotSeedingTestCollection : ICollectionFixture<MyWhiskyShelfDataSeedingFixtureFactory>;