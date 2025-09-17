using MyWhiskyShelf.IntegrationTests.Fixtures;

namespace MyWhiskyShelf.IntegrationTests.Collections;

[CollectionDefinition("AspireTests", DisableParallelization = true)]
public class AspireTestCollection : ICollectionFixture<MyWhiskyShelfFixture>;