using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using static MyWhiskyShelf.WebApi.Constants.Authentication;

namespace MyWhiskyShelf.WebApi.ExtensionMethods;

public static class WebApplicationBuilderExtensions
{
    public static void SetupAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(Policies.ReadWhiskyBottles,  policy => policy.RequireRole(Roles.User, Roles.Admin))
            .AddPolicy(Policies.WriteWhiskyBottles, policy => policy.RequireRole(Roles.User, Roles.Admin));

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                        options.Authority = "<LOAD THIS FROM SECRETS>";
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