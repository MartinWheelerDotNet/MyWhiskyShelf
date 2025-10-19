using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.IntegrationTests.Helpers;
using static MyWhiskyShelf.WebApi.Constants.Authentication;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection(nameof(WorkingFixture))]
public class AuthorizationTests(WorkingFixture fixture)
{
    public static TheoryData<string, bool, string, string> AuthorizationData()
    {
        return new TheoryData<string, bool, string, string>
        {
            //distilleries
            { Roles.User, true, $"/distilleries/{Guid.NewGuid()}", HttpMethod.Get.Method },
            { Roles.Admin, true, $"/distilleries/{Guid.NewGuid()}", HttpMethod.Get.Method },
            { Roles.User, true, "/distilleries?amount=10", HttpMethod.Get.Method },
            { Roles.Admin, true, "/distilleries", HttpMethod.Get.Method },
            { Roles.User, false, $"/distilleries/{Guid.NewGuid()}", HttpMethod.Delete.Method },
            { Roles.Admin, true, $"/distilleries/{Guid.NewGuid()}", HttpMethod.Delete.Method },
            { Roles.User, false, "/distilleries", HttpMethod.Post.Method },
            { Roles.Admin, true, "/distilleries", HttpMethod.Post.Method },
            { Roles.User, false, $"/distilleries/{Guid.NewGuid()}", HttpMethod.Put.Method },
            { Roles.Admin, true, $"/distilleries/{Guid.NewGuid()}", HttpMethod.Put.Method },
            // whisky-bottles
            { Roles.User, true, $"/whisky-bottles/{Guid.NewGuid()}", HttpMethod.Get.Method },
            { Roles.Admin, true, $"/whisky-bottles/{Guid.NewGuid()}", HttpMethod.Get.Method },
            { Roles.User, true, $"/whisky-bottles/{Guid.NewGuid()}", HttpMethod.Delete.Method },
            { Roles.Admin, true, $"/whisky-bottles/{Guid.NewGuid()}", HttpMethod.Delete.Method },
            { Roles.User, true, "/whisky-bottles", HttpMethod.Post.Method },
            { Roles.Admin, true, "/whisky-bottles", HttpMethod.Post.Method },
            { Roles.User, true, $"/whisky-bottles/{Guid.NewGuid()}", HttpMethod.Put.Method },
            { Roles.Admin, true, $"/whisky-bottles/{Guid.NewGuid()}", HttpMethod.Put.Method }
        };
    }

    public static TheoryData<string, string> EndpointData()
    {
        var data = new TheoryData<string, string>();
        var pairs = AuthorizationData()
            .Select(row => (Url: row[2].ToString(), Method: row[3].ToString()))
            .Distinct();

        foreach (var (url, method) in pairs)
            data.Add(url!, method!);

        return data;
    }

    [Theory]
    [MemberData(nameof(AuthorizationData), DisableDiscoveryEnumeration = true)]
    public async Task When_AuthenticateAsRole_Expect_ShouldAuthenticateResult(
        string role,
        bool shouldAuthenticate,
        string endpoint,
        string method
    )
    {
        var request = IdempotencyHelpers.CreateNoBodyRequestWithIdempotencyKey(HttpMethod.Parse(method), endpoint);
        using var httpClient = await fixture.Application.CreateHttpsClientWithRole(role);

        var response = await httpClient.SendAsync(request);

        if (shouldAuthenticate)
            Assert.False(response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden);
        else
            Assert.True(response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden);
    }

    [Theory]
    [MemberData(nameof(EndpointData), DisableDiscoveryEnumeration = true)]
    public async Task When_CreateWhiskyBottleWithoutBearerToken_Expect_Unauthorized(string endpoint, string method)
    {
        var request = IdempotencyHelpers.CreateNoBodyRequestWithIdempotencyKey(HttpMethod.Parse(method), endpoint);
        var httpClient = fixture.Application.CreateHttpClient("WebApi");
        httpClient.BaseAddress = fixture.Application.GetEndpoint("WebApi", "https");

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}