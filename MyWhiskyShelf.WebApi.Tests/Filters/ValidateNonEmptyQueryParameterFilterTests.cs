using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using MyWhiskyShelf.WebApi.Filters;

namespace MyWhiskyShelf.WebApi.Tests.Filters;

public class ValidateNonEmptyQueryParameterFilterTests
{
    [Fact]
    public async Task When_InvokeAsyncAndParameterIsPresentAndNotEmpty_Expect_ReturnsNextResult() 
    {
        var filter = new ValidateNonEmptyQueryParameterFilter("test");
        var context = CreateContext("validValue");

        var expectedResult = new OkResult();

        var result = await filter.InvokeAsync(context, Next);

        Assert.Same(expectedResult, result);
        return;

        ValueTask<object?> Next(EndpointFilterInvocationContext _) => ValueTask.FromResult<object?>(expectedResult);
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
            title: "Missing or empty query parameters",
            type: "urn:mywhiskyshelf:validation-errors:query-parameter",
            errors: new Dictionary<string, string[]>
            {
                { "test", ["Query parameter 'test' is required and cannot be empty."] }
            });
            
        var filter = new ValidateNonEmptyQueryParameterFilter("test");
        var context = CreateContext(value);
            
        var filterResult = await filter.InvokeAsync(context, NextThrowsException);
        var result = Assert.IsAssignableFrom<IResult>(filterResult);
            
        Assert.Equivalent(expectedResult, result);
    }
        
    private static ValueTask<object?> NextThrowsException(EndpointFilterInvocationContext _) 
        => throw new InvalidOperationException("Should not be called");
        
    private static DefaultEndpointFilterInvocationContext CreateContext(string? queryValue = null)
    {
        var httpContext = new DefaultHttpContext();
        if (queryValue is not null)
        {
            httpContext.Request.QueryString = new QueryString($"?test={queryValue}");
        }

        var routeEndpoint = new RouteEndpoint(
            _ => Task.CompletedTask, RoutePatternFactory.Parse("/"), 0, new EndpointMetadataCollection(), "test");

        return new DefaultEndpointFilterInvocationContext(httpContext, routeEndpoint, Array.Empty<object>());
    }
}