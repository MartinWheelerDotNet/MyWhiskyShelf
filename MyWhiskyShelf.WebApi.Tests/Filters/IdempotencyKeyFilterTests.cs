using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyWhiskyShelf.WebApi.Filters;
using MyWhiskyShelf.WebApi.Interfaces;
using MyWhiskyShelf.WebApi.Models;

namespace MyWhiskyShelf.WebApi.Tests.Filters;

public class IdempotencyKeyFilterTests
{
    private readonly IdempotencyKeyFilter _idempotencyKeyFilter;
    private readonly Mock<IIdempotencyService> _idempotencyServiceMock;

    public IdempotencyKeyFilterTests()
    {
        _idempotencyServiceMock = new Mock<IIdempotencyService>();
        _idempotencyKeyFilter = new IdempotencyKeyFilter(_idempotencyServiceMock.Object);
    }

    private static DefaultEndpointFilterInvocationContext CreateContext(HttpContext httpContext)
    {
        return new DefaultEndpointFilterInvocationContext(httpContext);
    }

    private static ValidationProblemDetails CreateIdempotencyKeyValidationProblem()
    {
        return new ValidationProblemDetails
        {
            Type = "urn:mywhiskyshelf:validation-errors:idempotency-key",
            Title = "Missing or empty idempotency key",
            Status = StatusCodes.Status400BadRequest,
            Errors = new Dictionary<string, string[]>
            {
                { "idempotencyKey", ["Header value 'idempotency-key' is required and must be an non-empty UUID"] }
            }
        };
    }

    private static DefaultHttpContext CreateHttpContext(string? idempotencyKeyHeader = null)
    {
        var httpContext = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection().AddLogging().BuildServiceProvider()
        };

        if (!string.IsNullOrEmpty(idempotencyKeyHeader))
            httpContext.Request.Headers["Idempotency-Key"] = idempotencyKeyHeader;

        return httpContext;
    }

    [Fact]
    public async Task When_IdempotencyKeyHeaderIsMissing_Expect_ValidationProblemReturned()
    {
        var expectedResult = CreateIdempotencyKeyValidationProblem();
        var httpContext = CreateHttpContext();
        var context = CreateContext(httpContext);

        var result = await _idempotencyKeyFilter.InvokeAsync(context, null!);

        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equivalent(expectedResult, problem.ProblemDetails);
    }

    [Fact]
    public async Task When_IdempotencyKeyHeaderIsInvalid_Expect_ValidationProblemReturned()
    {
        var expectedResult = CreateIdempotencyKeyValidationProblem();
        var httpContext = CreateHttpContext("not-a-guid");
        var context = CreateContext(httpContext);

        var result = await _idempotencyKeyFilter.InvokeAsync(context, null!);

        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equivalent(expectedResult, problem.ProblemDetails);
    }

    [Fact]
    public async Task When_IdempotencyKeyHeaderIsEmptyGuid_Expect_ValidationProblemReturned()
    {
        var expectedResult = CreateIdempotencyKeyValidationProblem();
        var httpContext = CreateHttpContext(Guid.Empty.ToString());
        var context = CreateContext(httpContext);

        var result = await _idempotencyKeyFilter.InvokeAsync(context, null!);

        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equivalent(expectedResult, problem.ProblemDetails);
    }


    [Fact]
    public async Task When_ResultCached_Expect_IResultReturnedAndResponseWritten()
    {
        var idempotencyKey = Guid.NewGuid();
        var headers = new Dictionary<string, string?[]>
        {
            { "Header-Key", ["Header-Value"] }
        };
        var httpContext = CreateHttpContext(idempotencyKey.ToString());
        httpContext.Response.Body = new MemoryStream();
        var context = CreateContext(httpContext);

        var cachedResponse = new CachedResponse(200, "cached-content", "application/json", headers);
        _idempotencyServiceMock
            .Setup(s => s.TryGetCachedResultAsync(idempotencyKey))
            .ReturnsAsync(cachedResponse);

        var result = await _idempotencyKeyFilter.InvokeAsync(context, null!);

        var iResult = Assert.IsType<IResult>(result, false);
        await iResult.ExecuteAsync(httpContext);

        httpContext.Response.Body.Position = 0;
        var body = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();

        Assert.Equal("cached-content", body);
        Assert.Equal("application/json", httpContext.Response.ContentType);
        Assert.Equal(200, httpContext.Response.StatusCode);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("    ")]
    [InlineData("\t \n")]
    public async Task When_ResultCachedAndContentIsEmpty_Expect_IResultReturnedAndResponseWritten(string content)
    {
        var idempotencyKey = Guid.NewGuid();
        var headers = new Dictionary<string, string?[]>
        {
            { "Header-Key", ["Header-Value"] }
        };
        var httpContext = CreateHttpContext(idempotencyKey.ToString());
        httpContext.Response.Body = new MemoryStream();
        var context = CreateContext(httpContext);

        var cachedResponse = new CachedResponse(200, content, "application/json", headers);
        _idempotencyServiceMock
            .Setup(s => s.TryGetCachedResultAsync(idempotencyKey))
            .ReturnsAsync(cachedResponse);

        var result = await _idempotencyKeyFilter
            .InvokeAsync(context, null!);

        var iResult = Assert.IsType<IResult>(result, false);
        await iResult.ExecuteAsync(httpContext);

        httpContext.Response.Body.Position = 0;
        var body = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();

        Assert.Empty(body);
        Assert.Null(httpContext.Response.ContentType);
        Assert.Equal(200, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task When_ResultNotCachedAndReturnsNonIResult_Expect_PassthroughAndNoCache()
    {
        var idempotencyKey = Guid.NewGuid().ToString();
        var httpContext = CreateHttpContext(idempotencyKey);
        var context = CreateContext(httpContext);

        _idempotencyServiceMock
            .Setup(s => s.TryGetCachedResultAsync(Guid.Parse(idempotencyKey)))
            .ReturnsAsync((CachedResponse?)null);

        var result = await _idempotencyKeyFilter.InvokeAsync(context, _ => ValueTask.FromResult<object?>(123));

        Assert.Equal(123, result);
        _idempotencyServiceMock.Verify(
            s => s.AddToCacheAsync(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<Dictionary<string, string?[]>>()),
            Times.Never);
    }

    [Fact]
    public async Task When_ResultNotCached_And_ReturnsIResult_Expect_CachedAndReturned()
    {
        var idempotencyKey = Guid.NewGuid().ToString();
        var httpContext = CreateHttpContext(idempotencyKey);
        var context = CreateContext(httpContext);
        var okResult = Results.Ok("new-content");

        _idempotencyServiceMock
            .Setup(s => s.TryGetCachedResultAsync(Guid.Parse(idempotencyKey)))
            .ReturnsAsync((CachedResponse?)null);

        var result = await _idempotencyKeyFilter.InvokeAsync(context, _ => ValueTask.FromResult<object?>(okResult));

        Assert.Same(okResult, result);
        _idempotencyServiceMock.Verify(
            s => s.AddToCacheAsync(
                idempotencyKey,
                200,
                "\"new-content\"",
                It.IsAny<string?>(),
                It.IsAny<Dictionary<string, string?[]>>()),
            Times.Once);
    }
}