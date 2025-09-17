using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.WebApi.Filters;

namespace MyWhiskyShelf.WebApi.Tests.Filters;

public class ValidateNonEmptyQueryParameterFilterTests
{
    private static EndpointFilterDelegate NextReturnsResult(object? result)
    {
        return _ => ValueTask.FromResult(result);
    }

    [Fact]
    public async Task When_InvokeAsyncAndParameterIsPresentAndNotEmpty_Expect_ReturnsNextResult()
    {
        var filter = new ValidateNonEmptyQueryParameterFilter("pattern");
        var context = CreateContext("?pattern=validValue");

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
            title: "Missing or empty query parameters",
            type: "urn:mywhiskyshelf:validation-errors:query-parameter",
            errors: new Dictionary<string, string[]>
            {
                { "pattern", ["Query parameter 'pattern' is required and cannot be empty."] }
            });

        var filter = new ValidateNonEmptyQueryParameterFilter("pattern");
        var context = CreateContext($"?pattern={value}");

        var filterResult = await filter.InvokeAsync(context, null!);
        var result = Assert.IsType<IResult>(filterResult, false);

        Assert.Equivalent(expectedResult, result);
    }

    private static DefaultEndpointFilterInvocationContext CreateContext(string? queryValue = null)
    {
        var httpContext = new DefaultHttpContext();
        if (queryValue is not null) httpContext.Request.QueryString = new QueryString(queryValue);

        return new DefaultEndpointFilterInvocationContext(httpContext, null, Array.Empty<object>());
    }
}