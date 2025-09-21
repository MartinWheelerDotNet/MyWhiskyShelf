using System.Net.Http.Json;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.IntegrationTests.Helpers;
using MyWhiskyShelf.IntegrationTests.TestData;
using MyWhiskyShelf.WebApi.Contracts.Distilleries;
using static MyWhiskyShelf.IntegrationTests.Fixtures.WorkingFixture;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection(nameof(WorkingFixture))]
public class WebApiDistilleriesTests(WorkingFixture fixture)
{
    [Fact]
    public async Task When_GettingAllDistilleries_Expect_AllDistilleriesReturned()
    {
        const string endpoint = "/distilleries";
        var expectedDistilleryResponses = fixture
            .GetSeededEntityDetailsByType(EntityType.Distillery)
            .Select((kvp, _) => DistilleryResponseTestData.GenericResponse(kvp.Id) with { Name = kvp.Name });

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var response = await httpClient.GetAsync(endpoint);
        var distilleries = await response.Content.ReadFromJsonAsync<List<DistilleryResponse>>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.All(expectedDistilleryResponses, distillery
                => Assert.Contains(distilleries!, actual => distillery == actual)),
            () => Assert.All(distilleries!, distillery
                => Assert.NotEqual(Guid.Empty, distillery.Id)));
    }

    [Fact]
    public async Task When_GettingDistilleryByIdAndDistilleryExists_Expect_CorrectDistilleryReturned()
    {
        await fixture.SeedDistilleriesAsync();
        var (name, id) = fixture.GetSeededEntityDetailByTypeAndMethod(HttpMethod.Post, EntityType.Distillery);

        var expectedResponse = DistilleryResponseTestData.GenericResponse(id) with { Name = name };

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var response = await httpClient.GetAsync($"/distilleries/{id}");
        var distilleryResponse = await response.Content.ReadFromJsonAsync<DistilleryResponse>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.Equal(expectedResponse, distilleryResponse!));
    }

    [Fact]
    public async Task When_GettingDistilleryByIdAndDistilleryDoesNotExist_Expect_NotFoundResponse()
    {
        var endpoint = $"/distilleries/{Guid.NewGuid()}";
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var response = await httpClient.GetAsync(endpoint);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task When_SearchingByNameAndNotFound_Expect_EmptyDistilleryNameResponse()
    {
        await fixture.SeedDistilleriesAsync(
            DistilleryRequestTestData.GenericCreate with { Name = "Any Distillery" });
        
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var response = await httpClient.GetAsync("/distilleries/search?pattern=Else");
        var distilleryNames = await response.Content.ReadFromJsonAsync<List<DistilleryNameResponse>>();

        Assert.Empty(distilleryNames!);
    }
    
    [Theory]
    [InlineData("CASE TEST")]
    [InlineData("CASE test")]
    [InlineData("CaSe TeSt")]
    [InlineData("case test")]
    public async Task When_SearchingByNameInsensitiveCase_Expect_DistilleryNameResponseWithJustThatDistillery(
        string pattern)
    {
        const string expectedName = "Case Test";
        var seededDistilleries = await fixture.SeedDistilleriesAsync(
            DistilleryRequestTestData.GenericCreate with { Name = expectedName });
            
        seededDistilleries.TryGetValue(expectedName, out var expectedId);
        var expectedResponse = new DistilleryNameResponse(expectedId, expectedName);

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var response = await httpClient.GetAsync($"/distilleries/search?pattern={pattern}");
        var distilleryNames = await response.Content.ReadFromJsonAsync<List<DistilleryNameResponse>>();

        Assert.Single(distilleryNames!, actual => expectedResponse == actual);
        
        var deleteRequest = IdempotencyHelpers
            .CreateNoBodyRequestWithIdempotencyKey(HttpMethod.Delete, $"/distilleries/{expectedId}");
        await httpClient.SendAsync(deleteRequest);
    }
    
    [Fact]
    public async Task When_SearchingByNameAndOneFound_Expect_DistilleryNameResponseWithJustThatDistillery()
    {
        const string expectedName = "One";
        var seededDistilleries = await fixture.SeedDistilleriesAsync(
            DistilleryRequestTestData.GenericCreate with { Name = expectedName },
            DistilleryRequestTestData.GenericCreate with { Name = "Another" });
        
        seededDistilleries.TryGetValue(expectedName, out var expectedId);

        var expectedResponse = new DistilleryNameResponse(expectedId, expectedName);

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var response = await httpClient.GetAsync($"/distilleries/search?pattern={expectedName}");
        var distilleryNames = await response.Content.ReadFromJsonAsync<List<DistilleryNameResponse>>();

        Assert.Single(distilleryNames!, actual => expectedResponse == actual);
    }
    
    [Fact]
    public async Task When_SearchingByNameAndMultipleFound_Expect_DistilleryNameResponseWithThoseDistilleries()
    {
        const string searchPattern = "Two";
        var seededDistilleries = await fixture.SeedDistilleriesAsync(
            DistilleryRequestTestData.GenericCreate with { Name = "Two" },
            DistilleryRequestTestData.GenericCreate with { Name = "Also Two" });

        var expectedDistilleryNames = seededDistilleries.Select(d => new DistilleryNameResponse(d.Value, d.Key));

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var response = await httpClient.GetAsync($"/distilleries/search?pattern={searchPattern}");
        var distilleryNames = await response.Content.ReadFromJsonAsync<List<DistilleryNameResponse>>();

        Assert.Equal(expectedDistilleryNames, distilleryNames);
    }
    
    [Fact]
    public async Task When_AddingDistilleryAndDistilleryDoesNotExist_Expect_CreatedWithLocationHeaderSet()
    {
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Post,
            "/distilleries",
            DistilleryRequestTestData.GenericCreate with { Name = "Distillery D" });
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var response = await httpClient.SendAsync(request);

        Assert.Multiple(
            () => Assert.NotNull(response.Headers.Location),
            () => Assert.Equal(HttpStatusCode.Created, response.StatusCode));
    }

    [Fact]
    public async Task When_AddingDistilleryAndDistilleryAlreadyExists_Expect_Conflict()
    {
        var (name, _) = fixture.GetSeededEntityDetailByTypeAndMethod(HttpMethod.Get, EntityType.Distillery);

        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Post,
            "/distilleries",
            DistilleryRequestTestData.GenericCreate with { Name = name });
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task When_DeletingDistilleryAndDistilleryExists_Expect_NoContent()
    {
        await fixture.SeedDistilleriesAsync();
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var (_, id) = fixture.GetSeededEntityDetailByTypeAndMethod(HttpMethod.Delete, EntityType.Distillery);
        var request = IdempotencyHelpers.CreateNoBodyRequestWithIdempotencyKey(
            HttpMethod.Delete,
            $"/distilleries/{id}");

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task When_DeletingDistilleryAndDistilleryDoesNotExist_Expect_NotFound()
    {
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var request = IdempotencyHelpers.CreateNoBodyRequestWithIdempotencyKey(
            HttpMethod.Delete,
            $"/distilleries/{Guid.NewGuid()}");

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task When_UpdatingDistilleryAndDistilleryExists_Expect_OkResponse()
    {
        await fixture.SeedDistilleriesAsync();
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var (name, id) = fixture.GetSeededEntityDetailByTypeAndMethod(HttpMethod.Put, EntityType.Distillery);
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Put,
            $"/distilleries/{id}",
            DistilleryRequestTestData.GenericUpdate with { Name = name });

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task When_UpdatingDistilleryAndDistilleryDoesNotExist_Expect_NotFoundResponse()
    {
        var distilleryId = Guid.NewGuid();
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Put,
            $"/distilleries/{distilleryId}",
            DistilleryRequestTestData.GenericUpdate with { Name = "Update Distillery" });

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}