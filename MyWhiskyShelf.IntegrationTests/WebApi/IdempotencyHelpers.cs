using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

public static class IdempotencyHelpers
{
    
    public static HttpRequestMessage CreateRequestWithIdempotencyKey<TRequest>(
        HttpMethod method, 
        string endpoint,
        TRequest? distilleryRequest,
        string? idempotencyKey = null) 
        where TRequest : class
    {
        var request = new HttpRequestMessage(method, endpoint);
        request.Content = JsonContent.Create(distilleryRequest);
        
        request.Headers.Add("Idempotency-Key", idempotencyKey ?? Guid.NewGuid().ToString());
        return request;
    }
    
    public static HttpRequestMessage CreateNoBodyRequestWithIdempotencyKey(
        HttpMethod method, 
        string endpoint,
        string? idempotencyKey = null) 
    {
        var request = new HttpRequestMessage(method, endpoint);
        
        request.Headers.Add("Idempotency-Key", idempotencyKey ?? Guid.NewGuid().ToString());
        return request;
    }
    
    public static ValidationProblemDetails CreateIdempotencyKeyValidationProblem() =>
        new()
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