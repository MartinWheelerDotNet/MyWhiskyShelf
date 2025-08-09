using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres("postgres")
    .WithPgWeb();

var database = postgres.AddDatabase("postgresDb");

var enableDataSeeding = builder.Configuration["MYWHISKYSHELF_DATA_SEEDING_ENABLED"] ?? "true";

builder.AddProject<MyWhiskyShelf_WebApi>("WebApi")
    .WithReference(database)
    .WithEnvironment("MYWHISKYSHELF_DATA_SEEDING_ENABLED", enableDataSeeding)
    .WaitFor(database);

await builder.Build().RunAsync();