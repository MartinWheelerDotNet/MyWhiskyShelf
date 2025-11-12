using System.Net.Http.Headers;
using Aspire.Hosting;
using static MyWhiskyShelf.WebApi.Constants.Authentication;

namespace MyWhiskyShelf.IntegrationTests.Helpers;

public static class AuthorizationHelpers
{
    extension(DistributedApplication application)
    {
        public async Task<HttpClient> CreateAdminHttpsClientAsync()
        {
            return await application.CreateHttpsClientWithRole(Roles.Admin);
        }

        public async Task<HttpClient> CreateHttpsClientWithRole(string role)
        {
            var keycloakUri = application.GetEndpoint("keycloak", "http");
            var userJwt = await KeycloakTokenClient.GetAccessTokenAsync(
                keycloakUri,
                "mywhiskyshelf",
                $"mywhiskyshelf-{role}-client",
                $"{role}-secret");

            var client = application.CreateHttpClient("WebApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userJwt);
            client.BaseAddress = application.GetEndpoint("WebApi", "https");

            return client;
        }
    }
}