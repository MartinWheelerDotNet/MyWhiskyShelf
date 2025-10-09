using System.Net.Http.Json;
using Aspire.Hosting;
using JetBrains.Annotations;
using MyWhiskyShelf.IntegrationTests.Helpers;
using MyWhiskyShelf.IntegrationTests.TestData;
using MyWhiskyShelf.WebApi.Contracts.Common;
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
        Application = await FixtureFactory.StartAsync(FixtureFactory.DefaultTestingArguments);
    }

    public (string Name, Guid Id) GetSeededEntityDetailByTypeAndMethod(HttpMethod method, EntityType entity)
    {
        var entityDetails = _seededEntityDetails[(method, entity)];
        return entityDetails;
    }

public async Task SeedDatabaseWithMethodTestData()
    {
        await SeedDistilleriesAsync();
        await SeedWhiskyBottlesAsync();
    }

    public async Task ClearDistilleriesAsync()
    {
        using var httpClient = await Application.CreateAdminHttpsClientAsync();
        var distilleries = await httpClient.GetFromJsonAsync<PagedResponse<DistilleryResponse>>(
            "/distilleries?page=1&amount=200");
        await DeleteDistilleriesAsync(distilleries!.Items.Select(x => x.Id));
    }

    private async Task SeedDistilleriesAsync()
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
        using var httpClient = await Application.CreateAdminHttpsClientAsync();
        
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

    public async Task<List<DistilleryResponse>> SeedDistilleriesAsync(int count)
    {
        var createRequests = Enumerable.Range(0, count)
            .Select(i => DistilleryRequestTestData.GenericCreate with { Name = $"Distillery Number {i}" })
            .ToArray();
        
        var seededDistilleryDetails = await SeedDistilleriesAsync(createRequests);
        
        return seededDistilleryDetails
            .Select(details => DistilleryResponseTestData.GenericResponse(details.Value) with { Name = details.Key })
            .OrderBy(response => response.Name)
            .ThenBy(response => response.Id)
            .ToList();
    }

    private async Task DeleteDistilleriesAsync(IEnumerable<Guid> ids)
    {
        using var httpClient = await Application.CreateAdminHttpsClientAsync();
        
        foreach (var id in ids)
        {
            var deleteRequest = IdempotencyHelpers.CreateNoBodyRequestWithIdempotencyKey(
                HttpMethod.Delete,
                $"/distilleries/{id}");
            
            await httpClient.SendAsync(deleteRequest);
        }
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