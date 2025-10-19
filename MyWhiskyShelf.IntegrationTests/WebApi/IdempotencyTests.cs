using System.ComponentModel;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.IntegrationTests.Comparers;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.IntegrationTests.Helpers;
using MyWhiskyShelf.IntegrationTests.TestData;
using static MyWhiskyShelf.IntegrationTests.Fixtures.WorkingFixture;
using static MyWhiskyShelf.IntegrationTests.Helpers.IdempotencyHelpers;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection(nameof(WorkingFixture))]
public class IdempotencyTests(WorkingFixture fixture)
{
    private static readonly List<(EntityType EntityType, string Method, string Path, object? Body)> EndpointData =
    [
        (EntityType.Distillery, HttpMethod.Post.Method, "/distilleries",
            DistilleryRequestTestData.GenericCreate),
        (EntityType.Distillery, HttpMethod.Put.Method, "/distilleries/{Id}",
            DistilleryRequestTestData.GenericUpdate with { Name = "Update" }),
        (EntityType.Distillery, HttpMethod.Delete.Method, "/distilleries/{Id}", 
            null),
        (EntityType.WhiskyBottle, HttpMethod.Post.Method, "/whisky-bottles", 
            WhiskyBottleRequestTestData.GenericCreate),
        (EntityType.WhiskyBottle, HttpMethod.Put.Method, "/whisky-bottles/{Id}",
            WhiskyBottleRequestTestData.GenericUpdate with { Name = "Update" }),
        (EntityType.WhiskyBottle, HttpMethod.Delete.Method, "/whisky-bottles/{Id}",
            null),
        (EntityType.Country, HttpMethod.Post.Method, "/geo/countries",
            CountryRequestTestData.GenericCreate),
        (EntityType.Region, HttpMethod.Post.Method, "/geo/regions",
            RegionRequestTestData.GenericCreate)
    ];

    private static readonly List<string> InvalidKeys =
    [
        " ",
        "   ",
        "    ",
        "\t \t",
        "00000000-0000-0000-0000-000000000000"
    ];
    
    public static TheoryData<EntityType, string, string, RequestBodyWrapper, string> InvalidKeysData()
    {
        var data = new TheoryData<EntityType, string, string, RequestBodyWrapper, string>();
        foreach (var (entityType, method, path, body) in EndpointData)
        foreach (var key in InvalidKeys)
            data.Add(entityType, method, path, new RequestBodyWrapper(body), key);

        return data;
    }

    [Theory]
    [MemberData(nameof(InvalidKeysData))]
    public async Task When_RequestWithInvalidIdempotencyKey_Expect_ValidationProblem(
        EntityType _,
        string method,
        string path,
        RequestBodyWrapper body,
        string key)
    {
        var expectedValidationProblem = CreateIdempotencyKeyValidationProblem();
        var request = CreateRequestWithIdempotencyKey(
            new HttpMethod(method),
            path.Replace("{Id}", Guid.NewGuid().ToString()),
            body.Value,
            key);
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var response = await httpClient.SendAsync(request);
        var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedValidationProblem, validationProblem);
    }


    public static TheoryData<EntityType, string, string, RequestBodyWrapper> IdempotentEndpointsData()
    {
        var data = new TheoryData<EntityType, string, string, RequestBodyWrapper>();
        foreach (var (entityType, method, path, body) in EndpointData)
            data.Add(entityType, method, path, new RequestBodyWrapper(body));

        return data;
    }

    [Theory]
    [MemberData(nameof(IdempotentEndpointsData))]
    public async Task When_RequestWithMissingIdempotencyKey_Expect_ValidationProblem(
        EntityType _,
        string method,
        string path,
        RequestBodyWrapper body)
    {
        var expectedValidationProblem = CreateIdempotencyKeyValidationProblem();
        var request = new HttpRequestMessage(new HttpMethod(method), path.Replace("{Id}", Guid.NewGuid().ToString()));
        if (body.Value is not null) request.Content = JsonContent.Create(body.Value);

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var response = await httpClient.SendAsync(request);
        var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedValidationProblem, validationProblem);
    }
    
    [Theory]
    [MemberData(nameof(IdempotentEndpointsData))]
    public async Task When_RequestTwiceWithSameIdempotencyKey_Expect_TheSameResultReturned(
        EntityType entityType,
        string method,
        string path,
        RequestBodyWrapper body)
    {
        var httpMethod = new HttpMethod(method);

        var id = await SeedIdempotencyTestData(entityType, method);
        path = path.Replace("{Id}", id.ToString());    
        

        var idempotencyKey = Guid.NewGuid().ToString();
        var initialRequest = CreateRequestWithIdempotencyKey(httpMethod, path, body.Value, idempotencyKey);
        var resendRequest = CreateRequestWithIdempotencyKey(httpMethod, path, body.Value, idempotencyKey);
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var initialResponse = await httpClient.SendAsync(initialRequest);
        var resendResponse = await httpClient.SendAsync(resendRequest);

        await fixture.ClearDatabaseAsync();

        Assert.True(initialResponse.IsSuccessStatusCode);
        Assert.Equal(initialResponse.StatusCode, resendResponse.StatusCode);
        Assert.True(new HttpResponseMessageComparer().Equals(initialResponse, resendResponse));
    }
    
    private async Task<Guid> SeedIdempotencyTestData(EntityType entityType, string method)
    {
        return entityType switch
        {
            EntityType.Distillery => await SeedDistillery(method),
            EntityType.WhiskyBottle => await SeedWhiskyBottle(method),
            EntityType.Country => await SeedCountry(method),
            EntityType.Region => await SeedRegion(method),
            _ => throw new InvalidEnumArgumentException()
        };
    }
    
    private async Task<Guid> SeedDistillery(string method)
    {
        var entity = DistilleryEntityTestData.Generic($"Distillery {method}");
        var responses = await fixture.SeedDistilleriesAsync([entity]);
        return responses.Single().Id;
    }

    private async Task<Guid> SeedWhiskyBottle(string method)
    {
        var entity = WhiskyBottleEntityTestData.Generic($"WhiskyBottle {method}");
        var responses = await fixture.SeedWhiskyBottlesAsync([entity]);
        return responses.Single().Id;
    }
    
    private async Task<Guid> SeedCountry(string method)
    {
        var entity = CountryEntityTestData.Generic($"Country {method}", $"country-{method.ToLower()}");
        var responses = await fixture.SeedCountriesAsync([entity]);
        return responses.Single().Id;
    }

    private async Task<Guid> SeedRegion(string method)
    {
        var country = CountryEntityTestData.Generic(
            $"Region Country {method}",
            $"region-country-{method.ToLower()}");
        country.Id = Guid.Parse("3b3830b8-081c-4503-8ec4-a623e4cc28bc");
        await fixture.SeedCountriesAsync([country]);
                
        var region = RegionEntityTestData.Generic($"Region {method}", $"region-{method.ToLower()}", country.Id);
        var responses = await fixture.SeedRegionsAsync([region]);
        return responses.Single().Id;
    }
}