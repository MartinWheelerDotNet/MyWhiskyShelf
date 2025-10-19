using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.WebApi.Filters;

namespace MyWhiskyShelf.WebApi.Tests.Filters;

public class ValidatePagingQueryInRangeFilterTests
{
    private static EndpointFilterDelegate NextReturnsResult(object? result)
    {
        return _ => ValueTask.FromResult(result);
    }

    [Fact]
    public async Task When_InvokeAsyncAndPageAndAmountNotProvided_Expect_NextReturnsNextResult()
    {
        var expectedResult = new OkResult();
        var filter = new ValidatePagingQueryInRangeFilter();
        var context = CreateContext("");

        var result = await filter.InvokeAsync(context, NextReturnsResult(expectedResult));

        Assert.Same(expectedResult, result);
    }

    [Theory]
    [InlineData("?page=1")]
    [InlineData("?amount=10")]
    public async Task When_InvokeAsyncAndPageOrAmountNotProvided_Expect_NextReturnsValidationProblem(string queryValue)
    {
        var expectedResult = Results.ValidationProblem(
            title: "Paging parameters are out of range",
            type: "urn:mywhiskyshelf:validation-errors:paging",
            errors: new Dictionary<string, string[]>
            {
                { "paging", ["Either page and amount should be omitted, or both should be provided"] }
            });

        var filter = new ValidatePagingQueryInRangeFilter();
        var context = CreateContext(queryValue);

        var filterResult = await filter.InvokeAsync(context, null!);
        var result = Assert.IsType<ProblemHttpResult>(filterResult);

        Assert.Equivalent(expectedResult, result);
    }

    [Theory]
    [InlineData("?page=-10&amount=10")]
    [InlineData("?page=-1&amount=10")]
    [InlineData("?page=0&amount=10")]
    public async Task When_InvokeAsyncAndPageIsOutOfBounds_Expect_NextReturnsValidationProblem(string queryValue)
    {
        var expectedResult = Results.ValidationProblem(
            title: "Paging parameters are out of range",
            type: "urn:mywhiskyshelf:validation-errors:paging",
            errors: new Dictionary<string, string[]>
            {
                { "page", ["page must be greater than or equal to 1"] }
            });

        var filter = new ValidatePagingQueryInRangeFilter();
        var context = CreateContext(queryValue);

        var filterResult = await filter.InvokeAsync(context, null!);
        var result = Assert.IsType<ProblemHttpResult>(filterResult);

        Assert.Equivalent(expectedResult, result);
    }

    [Theory]
    [InlineData("?page=1&amount=-10")]
    [InlineData("?page=1&amount=-1")]
    [InlineData("?page=1&amount=0")]
    [InlineData("?page=1&amount=201")]
    [InlineData("?page=1&amount=500")]
    public async Task When_InvokeAsyncAndAmountIsOutOfBounds_Expect_NextReturnsValidationProblem(string queryValue)
    {
        var expectedResult = Results.ValidationProblem(
            title: "Paging parameters are out of range",
            type: "urn:mywhiskyshelf:validation-errors:paging",
            errors: new Dictionary<string, string[]>
            {
                { "amount", ["amount must be between 1 and 200"] }
            });

        var filter = new ValidatePagingQueryInRangeFilter();
        var context = CreateContext(queryValue);

        var filterResult = await filter.InvokeAsync(context, null!);
        var result = Assert.IsType<ProblemHttpResult>(filterResult);

        Assert.Equivalent(expectedResult, result);
    }

    [Fact]
    public async Task When_InvokeAsyncAndAmountAndPageAreOutOfBounds_Expect_NextReturnsValidationProblem()
    {
        var expectedResult = Results.ValidationProblem(
            title: "Paging parameters are out of range",
            type: "urn:mywhiskyshelf:validation-errors:paging",
            errors: new Dictionary<string, string[]>
            {
                { "page", ["page must be greater than or equal to 1"] },
                { "amount", ["amount must be between 1 and 200"] }
            });

        var filter = new ValidatePagingQueryInRangeFilter();
        var context = CreateContext("?page=-1&amount=201");

        var filterResult = await filter.InvokeAsync(context, null!);
        var result = Assert.IsType<ProblemHttpResult>(filterResult);

        Assert.Equivalent(expectedResult, result);
    }

    [Fact]
    public async Task When_InvokeAsyncAndOnlyPageProvidedAndIsOutOfBounds_Expect_NextReturnsValidationProblem()
    {
        var expectedResult = Results.ValidationProblem(
            title: "Paging parameters are out of range",
            type: "urn:mywhiskyshelf:validation-errors:paging",
            errors: new Dictionary<string, string[]>
            {
                { "paging", ["Either page and amount should be omitted, or both should be provided"] },
                { "page", ["page must be greater than or equal to 1"] }
            });

        var filter = new ValidatePagingQueryInRangeFilter();
        var context = CreateContext("?page=-1");

        var filterResult = await filter.InvokeAsync(context, null!);
        var result = Assert.IsType<ProblemHttpResult>(filterResult);

        Assert.Equivalent(expectedResult, result);
    }


    [Fact]
    public async Task When_InvokeAsyncAndOnlyAmountProvidedAndIsOutOfBounds_Expect_NextReturnsValidationProblem()
    {
        var expectedResult = Results.ValidationProblem(
            title: "Paging parameters are out of range",
            type: "urn:mywhiskyshelf:validation-errors:paging",
            errors: new Dictionary<string, string[]>
            {
                { "paging", ["Either page and amount should be omitted, or both should be provided"] },
                { "amount", ["amount must be between 1 and 200"] }
            });

        var filter = new ValidatePagingQueryInRangeFilter();
        var context = CreateContext("?amount=201");

        var filterResult = await filter.InvokeAsync(context, null!);
        var result = Assert.IsType<ProblemHttpResult>(filterResult);

        Assert.Equivalent(expectedResult, result);
    }

    private static DefaultEndpointFilterInvocationContext CreateContext(string? queryValue = null)
    {
        var httpContext = new DefaultHttpContext();
        if (queryValue is not null) httpContext.Request.QueryString = new QueryString(queryValue);

        return new DefaultEndpointFilterInvocationContext(httpContext, null, Array.Empty<object>());
    }
}