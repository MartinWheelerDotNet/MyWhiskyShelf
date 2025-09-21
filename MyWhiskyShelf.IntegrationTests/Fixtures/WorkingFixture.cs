using System.Net.Http.Json;
using Aspire.Hosting;
using JetBrains.Annotations;
using MyWhiskyShelf.IntegrationTests.Helpers;
using MyWhiskyShelf.IntegrationTests.TestData;
using MyWhiskyShelf.WebApi.Contracts.Distilleries;
using MyWhiskyShelf.WebApi.Contracts.WhiskyBottles;

namespace MyWhiskyShelf.IntegrationTests.Fixtures;

[UsedImplicitly]
public class WorkingFixture : IAsyncLifetime
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
        _seededEntityDetails.Clear();
        string[] args =
        [
            "MYWHISKYSHELF_RUN_MIGRATIONS=true",
            "MYWHISKYSHELF_DATA_SEEDING_ENABLED=false",
            "MYWHISKYSHELF_PG_WEB_ENABLED=false",
            "MYWHISKYSHELF_REDIS_INSIGHT_ENABLED=false"
        ];
        Application = await FixtureFactory.StartAsync(args);
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

    public async Task SeedDatabase()
    {
        await SeedDistilleriesAsync();
        await SeedWhiskyBottlesAsync();
    }

    public async Task SeedDistilleriesAsync()
    {
        using var httpClient = await Application.CreateAdminHttpsClientAsync();

        foreach (var method in _methods)
        {
            var name = $"Distillery {method.Method}";
            var distilleryCreateRequest = DistilleryRequestTestData.GenericCreate with { Name = name };
            var request = IdempotencyHelpers
                .CreateRequestWithIdempotencyKey(HttpMethod.Post, "/distilleries", distilleryCreateRequest);
            var response = await httpClient
                .SendAsync(request);

            if (!response.IsSuccessStatusCode) continue;

            var distillery = await response.Content.ReadFromJsonAsync<DistilleryResponse>();
            _seededEntityDetails[(method, EntityType.Distillery)] = (name, distillery!.Id);
        }
    }

    public async Task<Dictionary<string, Guid>> SeedDistilleriesAsync(
        params DistilleryCreateRequest[] createRequests)
    {
        Dictionary<string, Guid> seededDistilleries = [];
        using var httpClient = Application.CreateHttpClient("WebApi");
        
        foreach (var createRequest in createRequests)
        {
            var request = IdempotencyHelpers
                .CreateRequestWithIdempotencyKey(HttpMethod.Post, "/distilleries", createRequest);
            var response = await httpClient
                .SendAsync(request);

            var distillery = await response.Content.ReadFromJsonAsync<DistilleryResponse>();
            
            seededDistilleries.Add(distillery!.Name, distillery.Id);
        }
        
        return seededDistilleries;
    }

    public async Task SeedWhiskyBottlesAsync()
    {
        using var httpClient = await Application.CreateAdminHttpsClientAsync();

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
    
    public virtual async Task DisposeAsync()
    {
        await Application.DisposeAsync();
    }

}