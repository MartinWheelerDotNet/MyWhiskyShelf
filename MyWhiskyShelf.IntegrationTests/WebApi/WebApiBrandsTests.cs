using System.Net.Http.Json;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.IntegrationTests.Helpers;
using MyWhiskyShelf.WebApi.Contracts.Brands;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection(nameof(WorkingFixture))]
public class WebApiBrandsTests(WorkingFixture fixture) : IAsyncLifetime
{
    public async Task InitializeAsync() => await Task.CompletedTask;
    public async Task DisposeAsync() => await fixture.ClearDatabaseAsync();

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(99)]
    [InlineData(123)]
    public async Task When_GetAllAndBrandsExists_Expect_OkWithListOfBrandsOrderedByName(int amount)
    {
        var expectedResponses = await fixture.SeedBrandsAsync(amount);
        
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.GetAsync("/brands");
        var brandResponses = await result.Content.ReadFromJsonAsync<List<BrandResponse>>();
        
        Assert.NotNull(brandResponses);
        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, result.StatusCode),
            () => Assert.Equal(amount, brandResponses.Count),
            () => Assert.Equal(expectedResponses, brandResponses));
    }
}
