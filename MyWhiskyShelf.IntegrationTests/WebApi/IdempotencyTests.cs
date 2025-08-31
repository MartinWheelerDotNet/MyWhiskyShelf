using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.IntegrationTests.Comparers;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.IntegrationTests.TestData;
using static MyWhiskyShelf.IntegrationTests.Fixtures.MyWhiskyShelfFixture;
using static MyWhiskyShelf.IntegrationTests.WebApi.IdempotencyHelpers;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection("AspireTests")]
public class IdempotencyTests(MyWhiskyShelfFixture fixture) : IClassFixture<MyWhiskyShelfFixture>
{
    private const string WebApiResourceName = "WebApi";

    private static readonly List<(EntityType EntityType, string Method, string Path, object? Body)> EndpointData =
    [
        (EntityType.Distillery, HttpMethod.Post.Method, "/distilleries", DistilleryRequestTestData.GenericCreate),
        (EntityType.Distillery, HttpMethod.Put.Method, "/distilleries/{Id}",
            DistilleryRequestTestData.GenericUpdate with
            {
                Name = "Update"
            }),
        (EntityType.Distillery, HttpMethod.Delete.Method, "/distilleries/{Id}", null),

        (EntityType.WhiskyBottle, HttpMethod.Post.Method, "/whisky-bottles", WhiskyBottleRequestTestData.GenericCreate),
        (EntityType.WhiskyBottle, HttpMethod.Put.Method, "/whisky-bottles/{Id}",
            WhiskyBottleRequestTestData.GenericUpdate with 
            {
                Name = "Update" 
            }),
        (EntityType.WhiskyBottle, HttpMethod.Delete.Method, "/whisky-bottles/{Id}", null)
    ];

    private static readonly List<string> InvalidKeys =
    [
        " ",
        "   ",
        "    ",
        "\t \t",
        "00000000-0000-0000-0000-000000000000"
    ];

    public static TheoryData<EntityType, string, string, RequestBodyWrapper> IdempotentEndpointsData()
    {
        var data = new TheoryData<EntityType, string, string, RequestBodyWrapper>();
        foreach (var (entityType, method, path, body) in EndpointData)
            data.Add(entityType, method, path, new RequestBodyWrapper(body));
    
        return data;
    }

    public static TheoryData<EntityType, string, string, RequestBodyWrapper, string> InvalidKeysData()
    {
        var data = new TheoryData<EntityType, string, string, RequestBodyWrapper, string>();
        foreach (var (entityType, method, path, body) in EndpointData)
            foreach (var key in InvalidKeys)
                data.Add(entityType, method, path, new RequestBodyWrapper(body), key);
        
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

        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.SendAsync(request);
        var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedValidationProblem, validationProblem);
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
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

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
        var httpMethod =  new HttpMethod(method);
        await fixture.SeedDatabase();

        var (_, id) = fixture.GetSeededEntityDetailByTypeAndMethod(httpMethod, entityType);
        path = path.Replace("{Id}", id.ToString());

        var key = Guid.NewGuid().ToString();
        var initialRequest = CreateRequestWithIdempotencyKey(httpMethod, path, body.Value, key);
        var resendRequest = CreateRequestWithIdempotencyKey(httpMethod, path, body.Value, key);
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var initialResponse = await httpClient.SendAsync(initialRequest);
        var resendResponse = await httpClient.SendAsync(resendRequest);

        Assert.True(initialResponse.IsSuccessStatusCode);
        Assert.Equal(initialResponse.StatusCode, resendResponse.StatusCode);
        Assert.True(new HttpResponseMessageComparer().Equals(initialResponse, resendResponse));
    }
}