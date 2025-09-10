using System.Net.Http.Json;
using Aspire.Hosting;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;
using MyWhiskyShelf.IntegrationTests.TestData;
using MyWhiskyShelf.IntegrationTests.WebApi;
using MyWhiskyShelf.Migrations.Migrations;
using MyWhiskyShelf.WebApi.Contracts.Distilleries;
using MyWhiskyShelf.WebApi.Contracts.WhiskyBottles;
using Projects;

namespace MyWhiskyShelf.IntegrationTests.Fixtures;

[UsedImplicitly]
public class MyWhiskyShelfFixture : IAsyncLifetime
{
    public enum EntityType
    {
        Distillery,
        WhiskyBottle
    }

    private readonly List<HttpMethod> _methods = [HttpMethod.Get, HttpMethod.Post, HttpMethod.Put, HttpMethod.Delete];

    private readonly Dictionary<(HttpMethod Method, EntityType Entity), (string Name, Guid Id)> _seededEntityDetails
        = new();

    public DistributedApplication Application { get; private set; } = null!;
    
    public virtual async Task InitializeAsync()
    {
        var appHost = await CreateDefaultAppHost();
        Application = await appHost.BuildAsync();
        await Application.StartAsync();

        await WaitForRunningState(Application, "WebApi");
        
        var connectionString = await Application.GetConnectionStringAsync("myWhiskyShelfDb");
        await ApplyMigrationsAsync(connectionString!);

    }

    private static async Task ApplyMigrationsAsync(string connectionString)
    {
        var services = new ServiceCollection();
        services.AddDbContext<MyWhiskyShelfDbContext>(opts =>
            opts.UseNpgsql(connectionString,
                b => b.MigrationsAssembly(typeof(InitialMigration).Assembly.GetName().Name)));

        await using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MyWhiskyShelfDbContext>();
        await db.Database.MigrateAsync();
    }

    public virtual async Task DisposeAsync()
    {
        await Application.DisposeAsync();
    }

    public (string Name, Guid Id) GetSeededEntityDetailByTypeAndMethod(HttpMethod method, EntityType entity)
    {
        var entityDetails = _seededEntityDetails[(method, entity)];
        return entityDetails;
    }


    public List<(HttpMethod Method, string Name, Guid Id)> GetSeededEntityDetailsByType(EntityType entityType)
    {
        return _seededEntityDetails
            .Where((kvp, _) => kvp.Key.Entity == entityType)
            .Select((kvp, _) => (kvp.Key.Method, kvp.Value.Name, kvp.Value.Id))
            .ToList();
    }

    protected virtual async Task<IDistributedApplicationTestingBuilder> CreateDefaultAppHost()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<MyWhiskyShelf_AppHost>(
        [
            "MYWHISKYSHELF_DATA_SEEDING_ENABLED=false",
            "MYWHISKYSHELF_PG_WEB_ENABLED=false",
            "MYWHISKYSHELF_REDIS_INSIGHT_ENABLED=false",
            "MYWHISKYSHELF_RUN_MIGRATIONS=false"
        ]);

        appHost.Services
            .ConfigureHttpClientDefaults(clientBuilder => clientBuilder.AddStandardResilienceHandler());

        return appHost;
    }

    private static async Task WaitForRunningState(
        DistributedApplication application,
        string serviceName,
        TimeSpan? timeout = null)
    {
        await application.Services.GetRequiredService<ResourceNotificationService>()
            .WaitForResourceAsync(serviceName, KnownResourceStates.Running)
            .WaitAsync(timeout ?? TimeSpan.FromSeconds(30));
    }

    public async Task SeedDatabase()
    {
        await SeedDistilleriesAsync();
        await SeedWhiskyBottlesAsync();
    }

    public async Task SeedDistilleriesAsync()
    {
        using var httpClient = Application.CreateHttpClient("WebApi");

        foreach (var method in _methods)
        {
            var name = $"Distillery {method.Method}";
            var distilleryCreateRequest = DistilleryRequestTestData.GenericCreate with { Name = name };
            var request = IdempotencyHelpers
                .CreateRequestWithIdempotencyKey(HttpMethod.Post, "/distilleries", distilleryCreateRequest);
            var response = await httpClient
                .SendAsync(request);

            if (!response.IsSuccessStatusCode) continue;

            var entity = await response.Content.ReadFromJsonAsync<DistilleryResponse>();
            _seededEntityDetails[(method, EntityType.Distillery)] = (name, entity!.Id);
        }
    }

    public async Task SeedWhiskyBottlesAsync()
    {
        using var httpClient = Application.CreateHttpClient("WebApi");

        foreach (var method in _methods)
        {
            var name = $"Whisky Bottle {method.Method}";
            var distilleryCreateRequest = WhiskyBottleRequestTestData.GenericCreate with { Name = name };
            var request = IdempotencyHelpers
                .CreateRequestWithIdempotencyKey(HttpMethod.Post, "/whisky-bottles", distilleryCreateRequest);
            var response = await httpClient
                .SendAsync(request);

            if (!response.IsSuccessStatusCode) continue;

            var entityResponse = await response.Content.ReadFromJsonAsync<WhiskyBottleResponse>();
            _seededEntityDetails[(method, EntityType.WhiskyBottle)] = (name, entityResponse!.Id);
        }
    }
}