using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MyWhiskyShelf.WebApi.Extensions;
using static MyWhiskyShelf.WebApi.Constants.Authentication;

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

        var authOptions = sp.GetRequiredService<IOptions<AuthorizationOptions>>().Value;

        Assert.Multiple(
            () => AssertRoles(authOptions.GetPolicy(Policies.ReadWhiskyBottles), Roles.User, Roles.Admin),
            () => AssertRoles(authOptions.GetPolicy(Policies.WriteWhiskyBottles), Roles.User, Roles.Admin),
            () => AssertRoles(authOptions.GetPolicy(Policies.ReadDistilleries), Roles.User, Roles.Admin),
            () => AssertRoles(authOptions.GetPolicy(Policies.WriteDistilleries), Roles.Admin),
            () => Assert.Equal("mywhiskyshelf-api", jwt.Audience),
            () => Assert.False(jwt.RequireHttpsMetadata),
            () => Assert.Equal(ClaimTypes.Role, jwt.TokenValidationParameters.RoleClaimType),
            () => Assert.Equal(ClaimTypes.Name, jwt.TokenValidationParameters.NameClaimType));
    }

    [Fact]
    public void When_SetupAuthorizationInProductionsWithoutAuthoritySet_Expect_ThrowsException()
    {
        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions { EnvironmentName = Environments.Production });
        builder.SetupAuthorization();
        using var sp = builder.Services.BuildServiceProvider();

        var exception = Assert
            .Throws<InvalidOperationException>(() => sp.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
                .Get(JwtBearerDefaults.AuthenticationScheme));

        Assert.Equal("Authentication:Authority must be configured.", exception.Message);
    }

    [Fact]
    public void When_SetupAuthorizationInProductionWithNonHttpsAuthority_Expect_ThrowsException()
    {
        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions { EnvironmentName = Environments.Production });
        builder.Configuration.AddInMemoryCollection(
        [
            new KeyValuePair<string, string?>(
                "Authentication:Authority",
                "http://keycloak.example.com/realms/mywhiskyshelf")
        ]);

        builder.SetupAuthorization();
        using var sp = builder.Services.BuildServiceProvider();

        var ex = Assert.Throws<InvalidOperationException>(() => sp
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme));

        Assert.Equal("Authentication:Authority must be HTTPS.", ex.Message);
    }

    [Fact]
    public void When_SetupAuthorizationInProduction_Expect_ConfiguresJwtBearerAndPolicies()
    {
        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions { EnvironmentName = Environments.Production });
        const string authority = "https://keycloak.example.com/realms/mywhiskyshelf";
        builder.Configuration.AddInMemoryCollection([
            new KeyValuePair<string, string?>("Authentication:Authority", authority)
        ]);

        builder.SetupAuthorization();
        using var sp = builder.Services.BuildServiceProvider();
        var jwt = sp
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        var authScheme = sp.GetRequiredService<IOptions<AuthenticationOptions>>().Value;
        var authOptions = sp.GetRequiredService<IOptions<AuthorizationOptions>>().Value;

        Assert.Multiple(
            () => AssertRoles(authOptions.GetPolicy(Policies.ReadWhiskyBottles), Roles.User, Roles.Admin),
            () => AssertRoles(authOptions.GetPolicy(Policies.WriteWhiskyBottles), Roles.User, Roles.Admin),
            () => AssertRoles(authOptions.GetPolicy(Policies.ReadDistilleries), Roles.User, Roles.Admin),
            () => AssertRoles(authOptions.GetPolicy(Policies.WriteDistilleries), Roles.Admin),
            () => Assert.Equal("mywhiskyshelf-api", jwt.Audience),
            () => Assert.True(jwt.RequireHttpsMetadata),
            () => Assert.Equal(authority, jwt.Authority),
            () => Assert.Equal(JwtBearerDefaults.AuthenticationScheme, authScheme.DefaultAuthenticateScheme),
            () => Assert.Equal(JwtBearerDefaults.AuthenticationScheme, authScheme.DefaultChallengeScheme),
            () => Assert.Equal(ClaimTypes.Role, jwt.TokenValidationParameters.RoleClaimType),
            () => Assert.Equal(ClaimTypes.Name, jwt.TokenValidationParameters.NameClaimType));
    }

    [Fact]
    public async Task When_TokenValidated_WithRealmAccessRoles_Expect_RolesAddedToIdentity()
    {
        // Arrange builder + options (Dev is fine for these event tests)
        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions { EnvironmentName = Environments.Development });
        builder.SetupAuthorization();
        await using var sp = builder.Services.BuildServiceProvider();
        var jwt = sp.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        var identity = new ClaimsIdentity("test");
        var principal = new ClaimsPrincipal(identity);

        const string realmAccessJson = $"{{\"roles\":[\"{Roles.User}\",\"{Roles.Admin}\",\"\", \"   \"]}}";
        identity.AddClaim(new Claim("realm_access", realmAccessJson));

        // Act
        var ctx = BuildTokenValidatedContext(principal);
        await jwt.Events.OnTokenValidated(ctx);

        // Assert
        var roles = identity.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();
        Assert.Contains(Roles.User, roles);
        Assert.Contains(Roles.Admin, roles);
        Assert.DoesNotContain(string.Empty, roles);
        Assert.DoesNotContain("   ", roles);
    }

    [Fact]
    public async Task When_TokenValidated_WithNoRealmAccess_Expect_NoRolesAdded()
    {
        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions { EnvironmentName = Environments.Development });
        builder.SetupAuthorization();
        await using var sp = builder.Services.BuildServiceProvider();
        var jwt = sp.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        var identity = new ClaimsIdentity("test");
        var principal = new ClaimsPrincipal(identity);

        var ctx = BuildTokenValidatedContext(principal);
        await jwt.Events.OnTokenValidated(ctx);

        Assert.Empty(identity.FindAll(ClaimTypes.Role));
    }

    [Fact]
    public async Task When_TokenValidated_WithMalformedRealmAccess_Expect_NoThrowAndNoRolesAdded()
    {
        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions { EnvironmentName = Environments.Development });
        builder.SetupAuthorization();
        await using var sp = builder.Services.BuildServiceProvider();
        var jwt = sp.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        var identity = new ClaimsIdentity("test");
        identity.AddClaim(new Claim("realm_access", "this-is-not-json"));
        var principal = new ClaimsPrincipal(identity);

        var ctx = BuildTokenValidatedContext(principal);
        await jwt.Events.OnTokenValidated(ctx);

        Assert.Empty(identity.FindAll(ClaimTypes.Role));
    }

    [Fact]
    public async Task When_TokenValidated_WithNullPrincipal_Expect_NoThrow()
    {
        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions { EnvironmentName = Environments.Development });
        builder.SetupAuthorization();
        await using var sp = builder.Services.BuildServiceProvider();
        var jwt = sp.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        var ctx = BuildTokenValidatedContext(null);
        await jwt.Events.OnTokenValidated(ctx);

        Assert.Null(ctx.Principal);
    }

    private static TokenValidatedContext BuildTokenValidatedContext(ClaimsPrincipal? principal)
    {
        var httpContext = new DefaultHttpContext();
        var scheme = new AuthenticationScheme(
            JwtBearerDefaults.AuthenticationScheme,
            JwtBearerDefaults.AuthenticationScheme,
            typeof(JwtBearerHandler));
        var options = new JwtBearerOptions();
        var ctx = new TokenValidatedContext(httpContext, scheme, options)
        {
            Principal = principal
        };
        return ctx;
    }

    private static void AssertRoles(AuthorizationPolicy? policy, params string[] expected)
    {
        var req = policy!.Requirements.OfType<RolesAuthorizationRequirement>().Single();
        foreach (var role in expected)
            Assert.Contains(role, req.AllowedRoles);
    }
}