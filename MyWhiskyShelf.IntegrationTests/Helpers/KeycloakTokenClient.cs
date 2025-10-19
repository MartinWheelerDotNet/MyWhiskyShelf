using System.Net.Http.Json;

namespace MyWhiskyShelf.IntegrationTests.Helpers;

public static class KeycloakTokenClient
{
    public static async Task<string> GetAccessTokenAsync(
        Uri keycloakBaseUri,
        string realm,
        string clientId,
        string clientSecret)
    {
        using var http = new HttpClient();
        http.BaseAddress = keycloakBaseUri;

        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret
        });
        var resp = await http.PostAsync($"/realms/{realm}/protocol/openid-connect/token", form);
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        return json!["access_token"].ToString()!;
    }
}