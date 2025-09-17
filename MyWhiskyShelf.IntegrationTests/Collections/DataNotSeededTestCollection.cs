using MyWhiskyShelf.IntegrationTests.Fixtures;

namespace MyWhiskyShelf.IntegrationTests.Collections;

[CollectionDefinition("DataNotSeededTestCollection", DisableParallelization = true)]
public class DataNotSeededTestCollection : ICollectionFixture<MyWhiskyShelfDataNotSeededFixture>;