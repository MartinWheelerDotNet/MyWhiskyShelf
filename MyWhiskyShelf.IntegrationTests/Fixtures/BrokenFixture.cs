using Aspire.Hosting;
using JetBrains.Annotations;
using Npgsql;

namespace MyWhiskyShelf.IntegrationTests.Fixtures;

[UsedImplicitly]
public class BrokenFixture : IAsyncLifetime
{
    public DistributedApplication Application { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        Application = await FixtureFactory.StartAsync(FixtureFactory.DefaultTestingArguments);
        await BreakDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        try
        {
            await Application.StopAsync();
        }
        finally
        {
            await Application.DisposeAsync();
        }
    }

    private async Task BreakDatabaseAsync()
    {
        var connectionString = await Application.GetConnectionStringAsync("myWhiskyShelfDb");
        await using var npgsqlConnection = new NpgsqlConnection(connectionString);
        await npgsqlConnection.OpenAsync();

        const string sql = "ALTER SCHEMA public RENAME TO broken;";

        await using var cmd = new NpgsqlCommand(sql, npgsqlConnection);
        await cmd.ExecuteNonQueryAsync();
    }
}