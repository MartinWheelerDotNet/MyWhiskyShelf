using System.Diagnostics.CodeAnalysis;
using MyWhiskyShelf.WebApi.ErrorResults;
using MyWhiskyShelf.WebApi.Interfaces;
using MyWhiskyShelf.WebApi.Models;

namespace MyWhiskyShelf.WebApi.Filters;

public class IdempotencyKeyFilter(IIdempotencyService idempotencyService) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        
        if (!TryParseIdempotencyKeyHeader(httpContext, out var key))
            return ValidationProblemResults.MissingOrInvalidIdempotencyKey();
        
        var cachedResult = await idempotencyService.TryGetCachedResultAsync(key.Value);
        if (cachedResult is not null)
        {
            await WriteResponse(httpContext, cachedResult);
            return null;
        }

        var result = await next(context);
        if (result is not IResult apiResult) return result;
        
        await WriteResultToCache(apiResult, httpContext, key.Value);

        return result;
    }

    private async ValueTask WriteResultToCache(IResult apiResult, HttpContext sourceContext, Guid idempotencyKey)
    {
        using var memoryStream = new MemoryStream();
        var responseContext = new DefaultHttpContext
        {
            Response = { Body = memoryStream },
            RequestServices = sourceContext.RequestServices
        };

        await apiResult.ExecuteAsync(responseContext);
        memoryStream.Position = 0;
        var statusCode = responseContext.Response.StatusCode;
        var content = await new StreamReader(memoryStream).ReadToEndAsync();
        var contentType = responseContext.Response.ContentType;
        var headers = responseContext.Response.Headers
            .ToDictionary(header => header.Key, header => header.Value.ToArray());

        await idempotencyService.AddToCacheAsync(idempotencyKey.ToString(), statusCode, content, contentType, headers);
    }

    private static async ValueTask WriteResponse(HttpContext httpContext, CachedResponse cachedResponse)
    {
        httpContext.Response.StatusCode = cachedResponse.StatusCode;
        foreach (var header in cachedResponse.Headers)
            httpContext.Response.Headers[header.Key] = header.Value;

        if (string.IsNullOrWhiteSpace(cachedResponse.Content))
        {
            httpContext.Response.ContentType = null;
            httpContext.Response.ContentLength = 0;
            await httpContext.Response.WriteAsync(string.Empty);
        }
        else
        {
            httpContext.Response.ContentType = cachedResponse.ContentType;
            httpContext.Response.ContentLength = cachedResponse.Content.Length;
            await httpContext.Response.WriteAsync(cachedResponse.Content);
        }
        
        
        
        
    }

    private static bool TryParseIdempotencyKeyHeader(
        HttpContext httpContext, 
        [NotNullWhen(true)] out Guid? idempotencyKey)
    {
        idempotencyKey = null;
        
        if (!httpContext.Request.Headers.TryGetValue("Idempotency-Key", out var value)) return false;
        if (!Guid.TryParse(value, out var parsedValue) || parsedValue == Guid.Empty) return false;
        
        idempotencyKey = parsedValue;
        return true;
    }
}
