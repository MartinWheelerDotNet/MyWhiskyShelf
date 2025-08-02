using JetBrains.Annotations;

namespace MyWhiskyShelf.IntegrationTests.Fixtures;

[UsedImplicitly]
public class MyWhiskyShelfBaseFixtureSeededDb : MyWhiskyShelfBaseFixture
{
    protected override bool UseDataSeeding => true;
}