using MyWhiskyShelf.IntegrationTests.Fixtures;

namespace MyWhiskyShelf.IntegrationTests.Collections;

[CollectionDefinition("DataSeededTestCollection")]
public class DataSeededTestCollection : ICollectionFixture<MyWhiskyShelfDataSeededFixture>;