using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.TestHelpers.Data;

namespace MyWhiskyShelf.TestHelpers;

[ExcludeFromCodeCoverage]
public static class DatabaseSeeding
{
    private static readonly List<DistilleryRequest> DistilleriesToSeed =
    [
        DistilleryRequestTestData.Aberargie,
        DistilleryRequestTestData.Aberfeldy,
        DistilleryRequestTestData.Aberlour
    ];

    public static async Task SeedDatabase(HttpClient httpClient)
    {
        foreach (var distilleryRequest in DistilleriesToSeed) 
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/distilleries");
            request.Content = JsonContent.Create(distilleryRequest);
            request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
            await httpClient.SendAsync(request);
        }
    }

    public static async Task ClearDatabase(HttpClient httpClient)
    {
        var distilleryEntities = await httpClient.GetFromJsonAsync<List<DistilleryNameDetails>>("/distilleries");
        if (distilleryEntities == null || distilleryEntities.Count == 0) return;
        
        foreach (var request in distilleryEntities.Select(entity => entity.Id).Select(id => new HttpRequestMessage(HttpMethod.Delete, $"/distilleries/{id}")))
        {
            request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
            await httpClient.SendAsync(request);
        }
        
        
    }
    
}