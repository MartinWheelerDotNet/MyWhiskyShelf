using JetBrains.Annotations;

namespace MyWhiskyShelf.IntegrationTests.Fixtures;

[UsedImplicitly]
public class MyWhiskyShelfBaseFixtureEmptyDb : MyWhiskyShelfBaseFixture
{
    public MyWhiskyShelfBaseFixtureEmptyDb()
    {
        UseDataSeeding = false;
    }
}