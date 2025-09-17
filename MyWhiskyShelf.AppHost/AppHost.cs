using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var database = postgres.AddDatabase("myWhiskyShelfDb");
var cache = builder.AddRedis("cache");

if (builder.Environment.IsDevelopment())
{
    if (builder.Configuration.GetValue("MYWHISKYSHELF_PG_WEB_ENABLED", true))
        postgres.WithPgWeb();

    if (builder.Configuration.GetValue("MYWHISKYSHELF_REDIS_INSIGHT_ENABLED", true))
        cache.WithRedisInsight();
}

var enableDataSeeding = builder.Configuration.GetValue("MYWHISKYSHELF_DATA_SEEDING_ENABLED", true);
var runMigrations = builder.Configuration.GetValue("MYWHISKYSHELF_RUN_MIGRATIONS", true);

var webApiProjectBuilder = builder.AddProject<MyWhiskyShelf_WebApi>("WebApi");

webApiProjectBuilder
    .WithEnvironment("MYWHISKYSHELF_DATA_SEEDING_ENABLED", enableDataSeeding.ToString)
    .WithReference(database)
    .WaitFor(database)
    .WithReference(cache)
    .WaitFor(cache);

if (runMigrations)
{
    var migrations = builder.AddProject<MyWhiskyShelf_MigrationService>("migrations")
        .WithReference(database)
        .WaitFor(database);
    
    webApiProjectBuilder
        .WithReference(migrations)
        .WaitForCompletion(migrations);
}

await builder.Build().RunAsync();