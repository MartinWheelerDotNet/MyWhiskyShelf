using Aspire.Hosting;
using JetBrains.Annotations;

namespace MyWhiskyShelf.IntegrationTests.Fixtures;

[UsedImplicitly]
public class BrokenFixture : IAsyncLifetime
{
    public DistributedApplication Application { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        string[] args = 
        [
            "MYWHISKYSHELF_RUN_MIGRATIONS=true",
            "MYWHISKYSHELF_DATA_SEEDING_ENABLED=false",
            "MYWHISKYSHELF_PG_WEB_ENABLED=false",
            "MYWHISKYSHELF_REDIS_INSIGHT_ENABLED=false"
        ];
        Application = await FixtureFactory.StartAsync(args);

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
        await Application.DisposeAsync();  
    } 
}