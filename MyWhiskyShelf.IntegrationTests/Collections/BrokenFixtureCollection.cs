using MyWhiskyShelf.IntegrationTests.Fixtures;

namespace MyWhiskyShelf.IntegrationTests.Collections;

[CollectionDefinition(nameof(BrokenFixture), DisableParallelization = true)]
public class BrokenFixtureCollection : ICollectionFixture<BrokenFixture>;