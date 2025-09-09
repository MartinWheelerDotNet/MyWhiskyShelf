using MyWhiskyShelf.IntegrationTests.Fixtures;

namespace MyWhiskyShelf.IntegrationTests.Collections;

[CollectionDefinition("DataNotSeededTestCollection")]
public class DataNotSeededTestCollection : ICollectionFixture<MyWhiskyShelfDataNotSeededFixture>;