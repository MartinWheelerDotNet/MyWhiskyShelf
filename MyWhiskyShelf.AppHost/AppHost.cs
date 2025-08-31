using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres("postgres")
    .WithPgWeb();
var database = postgres
    .AddDatabase("myWhiskyShelfDb");

var cache = builder
    .AddRedis("cache")
    .WithRedisInsight();

var enableDataSeeding = builder.Configuration["MYWHISKYSHELF_DATA_SEEDING_ENABLED"];

builder.AddProject<MyWhiskyShelf_WebApi>("WebApi")
    .WithEnvironment("MYWHISKYSHELF_DATA_SEEDING_ENABLED", enableDataSeeding)
    .WithReference(database)
    .WithReference(cache)
    .WaitFor(database)
    .WaitFor(cache);

await builder.Build().RunAsync();