using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres("postgres")
    .WithImage("pgvector/pgvector:pg18-trixie");

var database = postgres.AddDatabase("myWhiskyShelfDb");

var cache = builder
    .AddRedis("cache")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithContainerName("mws-redis");

var keycloak = builder
    .AddKeycloak("keycloak", 8080)
    .WithRealmImport("./Realms")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithContainerName("mws-keycloak");

if (builder.Environment.IsDevelopment())
{
    if (builder.Configuration.GetValue("MYWHISKYSHELF_PG_WEB_ENABLED", true))
        postgres.WithPgWeb();

    if (builder.Configuration.GetValue("MYWHISKYSHELF_REDIS_INSIGHT_ENABLED", true))
        cache.WithRedisInsight();
}

var enableDataSeeding = builder.Configuration.GetValue("MYWHISKYSHELF_DATA_SEEDING_ENABLED", true);
var runMigrations = builder.Configuration.GetValue("MYWHISKYSHELF_RUN_MIGRATIONS", true);

var webApi = builder.AddProject<MyWhiskyShelf_WebApi>("WebApi")
    .WithReference(keycloak)
    .WaitFor(keycloak)
    .WithEnvironment("MYWHISKYSHELF_DATA_SEEDING_ENABLED", enableDataSeeding.ToString())
    .WithReference(database)
    .WaitFor(database)
    .WithReference(cache)
    .WaitFor(cache);

if (builder.Configuration.GetValue("MYWHISKYSHELF_UI_ENABLED", true))
{
    // Stryker disable all: Frontend environment wiring isn’t covered by mutation tests
    const int vitePort = 5173; 
    builder
        .AddNpmApp("UI", "../MyWhiskyShelf.Frontend")
        .WithEnvironment("BROWSER", "none")
        .WithEnvironment("VITE_WEBAPI_URL", webApi.GetEndpoint("https"))
        .WithEnvironment("VITE_KEYCLOAK_URL", keycloak.GetEndpoint("http"))
        .WithEnvironment("VITE_KEYCLOAK_REALM", "mywhiskyshelf")
        .WithEnvironment("VITE_KEYCLOAK_CLIENT_ID", "mywhiskyshelf-frontend")
        .WithEnvironment("VITE_PORT", vitePort.ToString())
        .WithHttpEndpoint(port: vitePort, env: "VITE_PORT")
        .WithReference(webApi)
        .WaitFor(webApi)
        .WithReference(keycloak)
        .WaitFor(keycloak);
    // Stryker restore all
}


if (runMigrations)
{
    var migrations = builder.AddProject<MyWhiskyShelf_MigrationService>("migrations")
        .WithReference(database)
        .WaitFor(database);
    
    webApi
        .WithReference(migrations)
        .WaitForCompletion(migrations);
}

await builder.Build().RunAsync();