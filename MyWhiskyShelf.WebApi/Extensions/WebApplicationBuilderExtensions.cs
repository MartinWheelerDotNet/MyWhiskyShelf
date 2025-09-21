using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using static MyWhiskyShelf.WebApi.Constants.Authentication;

namespace MyWhiskyShelf.WebApi.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void SetupAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(Policies.ReadDistilleries,  policy => policy.RequireRole(Roles.User, Roles.Admin))
            .AddPolicy(Policies.WriteDistilleries, policy => policy.RequireRole(Roles.Admin))
            .AddPolicy(Policies.ReadWhiskyBottles,  policy => policy.RequireRole(Roles.User, Roles.Admin))
            .AddPolicy(Policies.WriteWhiskyBottles, policy => policy.RequireRole(Roles.User, Roles.Admin));

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddKeycloakJwtBearer(
                serviceName: "keycloak",
                realm: "mywhiskyshelf",
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
                        RoleClaimType = ClaimTypes.Role
                    };
                });
        
        builder.Logging.AddFilter("Microsoft.IdentityModel", LogLevel.Debug);
        builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Debug);
    }
}