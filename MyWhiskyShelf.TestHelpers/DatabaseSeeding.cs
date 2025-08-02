using System.Net.Http.Json;
using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.TestHelpers;

public static class DatabaseSeeding
{
    public static async Task<List<Guid>> AddDistilleries(HttpClient httpClient,
        params DistilleryRequest[] distilleryRequests)
    {
        const string addEndpoint = "/distilleries";
        var createdIds = new List<Guid>();
        foreach (var distilleryRequest in distilleryRequests)
        {
            var response = await httpClient.PostAsJsonAsync(addEndpoint, distilleryRequest);
            var identifierString = response.Headers.Location!.OriginalString.Split('/').Last();
            createdIds.Add(Guid.Parse(identifierString));
        }

        return createdIds;
    }

    public static async Task RemoveDistilleries(HttpClient httpClient, List<Guid> ids)
    {
        foreach (var id in ids)
            await httpClient.DeleteAsync($"/distilleries/{id}");
    }
}