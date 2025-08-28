using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using MyWhiskyShelf.WebApi.Filters;

namespace MyWhiskyShelf.WebApi.Tests.Filters;

public class ValidateNonEmptyRouteParameterFilterTests
{
    private static EndpointFilterDelegate NextReturnsResult(object? result) => _ => ValueTask.FromResult(result);

    [Fact]
    public async Task When_InvokeAsyncAndParameterIsPresentAndNotEmpty_Expect_ReturnsNextResult() 
    {
        var filter = new ValidateNonEmptyRouteParameterFilter("test");
        var context = CreateContext("validValue");

        var expectedResult = new OkResult();

        var result = await filter.InvokeAsync(context, NextReturnsResult(expectedResult));

        Assert.Same(expectedResult, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("    ")]
    [InlineData("\t \n")]
    public async Task When_InvokeAsyncAndParameterIsNullEmptyOrWhitespace_Expect_ValidationProblem(string? value)
    {
        var expectedResult = Results.ValidationProblem(
            title: "Missing or empty route parameters",
            type: "urn:mywhiskyshelf:validation-errors:route-parameter",
            errors: new Dictionary<string, string[]>
            {
                { "test", ["Route parameter 'test' is required and cannot be empty."] }
            });
            
        var filter = new ValidateNonEmptyRouteParameterFilter("test");
        var context = CreateContext(value);
            
        var filterResult = await filter.InvokeAsync(context, null!);
        var result = Assert.IsAssignableFrom<IResult>(filterResult);
            
        Assert.Equivalent(expectedResult, result);
    }
        
    private static DefaultEndpointFilterInvocationContext CreateContext(string? routeValue = null)
    {
        var httpContext = new DefaultHttpContext();
        if (routeValue is not null) httpContext.Request.RouteValues["test"] = routeValue;
            
        return new DefaultEndpointFilterInvocationContext(httpContext, null, Array.Empty<object>());
    }
}