using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres", port: builder.Environment.IsDevelopment() ? 55432 : null);
var database = postgres.AddDatabase("myWhiskyShelfDb");

var migrations = builder.AddProject<MyWhiskyShelf_MigrationService>("migrations")
    .WithReference(postgres)
    .WaitFor(postgres);

var cache = builder.AddRedis("cache");

if (builder.Environment.IsDevelopment())
{
    if (builder.Configuration.GetValue("MYWHISKYSHELF_PG_WEB_ENABLED", false))
        postgres.WithPgWeb();

    if (builder.Configuration.GetValue("MYWHISKYSHELF_REDIS_INSIGHT_ENABLED", false))
        cache.WithRedisInsight();
}

var enableDataSeeding = builder.Configuration["MYWHISKYSHELF_DATA_SEEDING_ENABLED"];

builder.AddProject<MyWhiskyShelf_WebApi>("WebApi")
    .WithEnvironment("MYWHISKYSHELF_DATA_SEEDING_ENABLED", enableDataSeeding)
    .WithReference(database)
    .WithReference(cache)
    .WithReference(migrations)
    .WaitFor(database)
    .WaitFor(cache)
    .WaitForCompletion(migrations);

await builder.Build().RunAsync();