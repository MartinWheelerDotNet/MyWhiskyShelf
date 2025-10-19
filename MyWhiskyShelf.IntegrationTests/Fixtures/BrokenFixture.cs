using Aspire.Hosting;
using JetBrains.Annotations;

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

    private async Task BreakDatabaseAsync()
    {
        var connectionString = await Application.GetConnectionStringAsync("myWhiskyShelfDb");
        await using var npgsqlConnection = new Npgsql.NpgsqlConnection(connectionString);
        await npgsqlConnection.OpenAsync();
        
        const string sql = "ALTER SCHEMA public RENAME TO broken;";

        await using var cmd = new Npgsql.NpgsqlCommand(sql, npgsqlConnection);
        await cmd.ExecuteNonQueryAsync();
    }
    
    public async Task DisposeAsync()
    {
        try
        {
            await Application.StopAsync();
            await Application.ResourceNotifications
                .WaitForResourceAsync("WebApi")
                .WaitAsync(TimeSpan.FromSeconds(20));
        }
        finally
        {
            await Application.DisposeAsync();
        }
    } 
}