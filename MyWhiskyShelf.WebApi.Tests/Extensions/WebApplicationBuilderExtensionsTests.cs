using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MyWhiskyShelf.WebApi.Extensions;

namespace MyWhiskyShelf.WebApi.Tests.Extensions;

public class WebApplicationBuilderExtensionsTests
{
    [Fact]
    public void When_SetupAuthorizationInDevelopment_Expect_ConfiguresJwtBearerAndPoliciesAndDisablesHttpsMetadata()
    {
        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions { EnvironmentName = Environments.Development });

        builder.SetupAuthorization();
        using var sp = builder.Services.BuildServiceProvider();
        var jwt = sp
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        Assert.Multiple(
            () => Assert.Equal("mywhiskyshelf-api", jwt.Audience),
            () => Assert.False(jwt.RequireHttpsMetadata),
            () => Assert.Equal(ClaimTypes.Role, jwt.TokenValidationParameters.RoleClaimType));
    }

    [Fact]
    public void When_SetupAuthorizationInProductionsWithoutAuthoritySet_Expect_ThrowsException()
    {
        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions { EnvironmentName = Environments.Production });
        builder.SetupAuthorization();
        using var sp = builder.Services.BuildServiceProvider();

        var exception = Assert
            .Throws<InvalidOperationException>(
                () => sp.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
                    .Get(JwtBearerDefaults.AuthenticationScheme));

        Assert.Equal("Authentication:Authority must be configured in Production.", exception.Message);
    }

    [Fact]
    public void When_SetupAuthorizationInProductionWithNonHttpsAuthority_Expect_ThrowsException()
    {
        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions { EnvironmentName = Environments.Production });
        builder.Configuration.AddInMemoryCollection(
        [
            new KeyValuePair<string,string?>(
                "Authentication:Authority", 
                "http://keycloak.example.com/realms/mywhiskyshelf")
        ]);

        builder.SetupAuthorization();
        using var sp = builder.Services.BuildServiceProvider();

        var ex = Assert.Throws<InvalidOperationException>(
            () => sp.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
                .Get(JwtBearerDefaults.AuthenticationScheme));

        Assert.Equal("Authentication:Authority must be HTTPS in Production.", ex.Message);
    }

    [Fact]
    public void When_SetupAuthorizationInProduction_Expect_ConfiguresJwtBearerAndPolicies()
    {
        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions { EnvironmentName = Environments.Production });
        const string authority = "https://keycloak.example.com/realms/mywhiskyshelf";
        builder.Configuration.AddInMemoryCollection([
            new KeyValuePair<string,string?>("Authentication:Authority", authority)
        ]);

        builder.SetupAuthorization();
        using var sp = builder.Services.BuildServiceProvider();
        var jwt = sp
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);
        
        var authScheme = sp.GetRequiredService<IOptions<AuthenticationOptions>>().Value;

        Assert.Multiple(
            () => Assert.Equal("mywhiskyshelf-api", jwt.Audience),
            () => Assert.True(jwt.RequireHttpsMetadata),
            () => Assert.Equal(ClaimTypes.Role, jwt.TokenValidationParameters.RoleClaimType),
            () => Assert.Equal(authority, jwt.Authority),
            () => Assert.Equal(JwtBearerDefaults.AuthenticationScheme, authScheme.DefaultAuthenticateScheme),
            () => Assert.Equal(JwtBearerDefaults.AuthenticationScheme, authScheme.DefaultChallengeScheme));
    }
}