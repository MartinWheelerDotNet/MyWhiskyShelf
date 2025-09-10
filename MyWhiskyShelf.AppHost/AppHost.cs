using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var database = postgres.AddDatabase("myWhiskyShelfDb");
var cache = builder.AddRedis("cache");

if (builder.Environment.IsDevelopment())
{
    if (builder.Configuration.GetValue("MYWHISKYSHELF_PG_WEB_ENABLED", false))
        postgres.WithPgWeb();

    if (builder.Configuration.GetValue("MYWHISKYSHELF_REDIS_INSIGHT_ENABLED", false))
        cache.WithRedisInsight();
}

var enableDataSeeding = builder.Configuration["MYWHISKYSHELF_DATA_SEEDING_ENABLED"];
var runMigrations = builder.Configuration.GetValue("MYWHISKYSHELF_RUN_MIGRATIONS", true);

if (runMigrations)
{
    var migrations = builder.AddProject<MyWhiskyShelf_MigrationService>("migrations")
        .WithReference(database)
        .WaitFor(database);
    
    builder.AddProject<MyWhiskyShelf_WebApi>("WebApi")
        .WithEnvironment("MYWHISKYSHELF_DATA_SEEDING_ENABLED", enableDataSeeding)
        .WithReference(database)
        .WaitFor(database)
        .WithReference(cache)
        .WaitFor(cache)
        .WithReference(migrations)
        .WaitForCompletion(migrations);
}
else
{
    builder.AddProject<MyWhiskyShelf_WebApi>("WebApi")
        .WithEnvironment("MYWHISKYSHELF_DATA_SEEDING_ENABLED", enableDataSeeding)
        .WithReference(database)
        .WaitFor(database)
        .WithReference(cache)
        .WaitFor(cache);
}

await builder.Build().RunAsync();