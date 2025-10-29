using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using static MyWhiskyShelf.WebApi.Constants.Authentication;

namespace MyWhiskyShelf.WebApi.Extensions;

public static class WebApplicationBuilderExtensions
{
    private static JwtBearerEvents JwtEvents =>
        new()
        {
            OnTokenValidated = ctx =>
            {
                if (ctx.Principal?.Identity is not ClaimsIdentity identity)
                    return Task.CompletedTask;


                var realmAccessJson = ctx.Principal!.FindFirst("realm_access")?.Value;
                if (string.IsNullOrWhiteSpace(realmAccessJson))
                    return Task.CompletedTask;

                try
                {
                    using var doc = JsonDocument.Parse(realmAccessJson);
                    if (doc.RootElement.TryGetProperty("roles", out var rolesEl) &&
                        rolesEl.ValueKind == JsonValueKind.Array)
                    {
                        var roles = rolesEl.EnumerateArray()
                            .Where(x => x.ValueKind == JsonValueKind.String)
                            .Select(x => x.GetString()!)
                            .ToArray();
                        AddRoles(identity, roles);
                    }
                }
                catch
                {
                    // ignore malformed JSON
                }

                return Task.CompletedTask;
            }
        };

    public static void SetupAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(Policies.ReadDistilleries, policy => policy.RequireRole(Roles.User, Roles.Admin))
            .AddPolicy(Policies.WriteDistilleries, policy => policy.RequireRole(Roles.Admin))
            .AddPolicy(Policies.ReadWhiskyBottles, policy => policy.RequireRole(Roles.User, Roles.Admin))
            .AddPolicy(Policies.WriteWhiskyBottles, policy => policy.RequireRole(Roles.User, Roles.Admin))
            .AddPolicy(Policies.ReadGeoData, policy => policy.RequireRole(Roles.User, Roles.Admin))
            .AddPolicy(Policies.WriteGeoData, policy => policy.RequireRole(Roles.Admin))
            .AddFallbackPolicy("default", policy => policy.RequireAuthenticatedUser());

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddKeycloakJwtBearer(
                "keycloak",
                "mywhiskyshelf",
                options =>
                {
                    options.Audience = "mywhiskyshelf-api";

                    /*
                     * When using RequireHttpsMetadata = true (the default), the JWT Bearer authentication requires
                     * the Authority URL to use HTTPS. However, .NET Aspire service discovery uses the
                     * https+http:// scheme, which doesn't satisfy this requirement. For production scenarios where
                     * HTTPS metadata validation is required, you need to explicitly configure the Authority URL.
                     */
                    if (builder.Environment.IsDevelopment())
                    {
                        options.RequireHttpsMetadata = false;
                    }
                    else
                    {
                        var authority = builder.Configuration["Authentication:Authority"];
                        if (string.IsNullOrWhiteSpace(authority))
                            throw new InvalidOperationException(
                                "Authentication:Authority must be configured in Production.");
                        if (!authority.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                            throw new InvalidOperationException(
                                "Authentication:Authority must be HTTPS in Production.");
                        options.Authority = authority;
                    }

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.Name
                    };

                    options.Events = JwtEvents;
                });
    }

    private static void AddRoles(ClaimsIdentity id, IEnumerable<string>? roles)
    {
        if (roles == null) return;

        foreach (var role in roles.Where(role => !string.IsNullOrWhiteSpace(role)))
            id.AddClaim(new Claim(ClaimTypes.Role, role));
    }
}