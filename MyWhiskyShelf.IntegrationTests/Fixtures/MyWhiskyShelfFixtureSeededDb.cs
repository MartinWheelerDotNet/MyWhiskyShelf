using JetBrains.Annotations;

namespace MyWhiskyShelf.IntegrationTests.Fixtures;

[UsedImplicitly]
public class MyWhiskyShelfBaseFixtureSeededDb : MyWhiskyShelfBaseFixture
{
    public MyWhiskyShelfBaseFixtureSeededDb() => UseDataSeeding = true;
}