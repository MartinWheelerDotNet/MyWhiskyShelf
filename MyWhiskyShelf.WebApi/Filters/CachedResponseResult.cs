using System.Text;
using MyWhiskyShelf.WebApi.Models;

namespace MyWhiskyShelf.WebApi.Filters;

public sealed class CachedResponseResult(CachedResponse cached) : IResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = cached.StatusCode;
        foreach (var header in cached.Headers)
            httpContext.Response.Headers[header.Key] = header.Value;
        
        if (string.IsNullOrWhiteSpace(cached.Content)) return;
        
        httpContext.Response.ContentType = cached.ContentType;
        await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(cached.Content));
    }
}