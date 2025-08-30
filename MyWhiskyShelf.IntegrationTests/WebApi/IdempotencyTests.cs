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

    private static readonly List<(EntityType EntityType, HttpMethod Method, string Path, object? Body)> EndpointData =
    [
        (EntityType.Distillery, HttpMethod.Post, "/distilleries", DistilleryRequestTestData.GenericCreate),
        (EntityType.Distillery, HttpMethod.Put, "/distilleries/{Id}", DistilleryRequestTestData.GenericUpdate with
        {
            Name = "Update"
        }),
        (EntityType.Distillery, HttpMethod.Delete, "/distilleries/{Id}", null),

        (EntityType.WhiskyBottle, HttpMethod.Post, "/whisky-bottles", WhiskyBottleRequestTestData.GenericCreate),
        (EntityType.WhiskyBottle, HttpMethod.Put, "/whisky-bottles/{Id}", WhiskyBottleRequestTestData.GenericUpdate with
        {
            Name = "Update"
        }),
        (EntityType.WhiskyBottle, HttpMethod.Delete, "/whisky-bottles/{Id}", null)
    ];

    private static readonly List<string> InvalidKeys =
    [
        " ",
        "   ",
        "    ",
        "\t \t",
        "00000000-0000-0000-0000-000000000000"
    ];

    public static IEnumerable<object?[]> IdempotentEndpointsData
        => from data in EndpointData
            select new[] { data.EntityType, data.Method, data.Path, data.Body };

    public static IEnumerable<object?[]> InvalidKeysData
        => from data in IdempotentEndpointsData
            from key in InvalidKeys
            select data.Append(key).ToArray();

    [Theory]
    [MemberData(nameof(IdempotentEndpointsData))]
    public async Task When_RequestWithMissingIdempotencyKey_Expect_ValidationProblem(
        EntityType _,
        HttpMethod method,
        string path,
        object? body)
    {
        var expectedValidationProblem = CreateIdempotencyKeyValidationProblem();
        var request = new HttpRequestMessage(method, path.Replace("{Id}", Guid.NewGuid().ToString()));
        if (body is not null) request.Content = JsonContent.Create(body);

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
        HttpMethod httpMethod,
        string path,
        object? body,
        string key)
    {
        var expectedValidationProblem = CreateIdempotencyKeyValidationProblem();
        var request = CreateRequestWithIdempotencyKey(
            httpMethod,
            path.Replace("{Id}", Guid.NewGuid().ToString()),
            body,
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
        HttpMethod method,
        string path,
        object? body)
    {
        await fixture.SeedDatabase();

        var (_, id) = fixture.GetSeededEntityDetailByTypeAndMethod(method, entityType);
        path = path.Replace("{Id}", id.ToString());

        var key = Guid.NewGuid().ToString();
        var initialRequest = CreateRequestWithIdempotencyKey(method, path, body, key);
        var resendRequest = CreateRequestWithIdempotencyKey(method, path, body, key);
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var initialResponse = await httpClient.SendAsync(initialRequest);
        var resendResponse = await httpClient.SendAsync(resendRequest);

        Assert.True(initialResponse.IsSuccessStatusCode);
        Assert.Equal(initialResponse.StatusCode, resendResponse.StatusCode);
        Assert.True(new HttpResponseMessageComparer().Equals(initialResponse, resendResponse));
    }
}