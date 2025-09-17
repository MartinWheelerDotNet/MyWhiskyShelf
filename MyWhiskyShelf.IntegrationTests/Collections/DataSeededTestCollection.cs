using MyWhiskyShelf.IntegrationTests.Fixtures;

namespace MyWhiskyShelf.IntegrationTests.Collections;

[CollectionDefinition("DataSeededTestCollection", DisableParallelization = true)]
public class DataSeededTestCollection : ICollectionFixture<MyWhiskyShelfDataSeededFixture>;