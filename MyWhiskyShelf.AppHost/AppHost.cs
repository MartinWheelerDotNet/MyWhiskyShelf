var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres("postgres")
    .WithPgWeb();

var database = postgres.AddDatabase("postgresDb");

builder.AddProject<Projects.MyWhiskyShelf_WebApi>("WebApi")
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();