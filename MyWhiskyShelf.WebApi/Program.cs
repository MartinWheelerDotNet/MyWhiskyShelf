using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

namespace MyWhiskyShelf.WebApi;

[ExcludeFromCodeCoverage]
internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication
            .CreateBuilder(args)
            .ConfigureDefaultServices();
        builder.AddNpgsqlDataSource("postgresDb");

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapDefaultEndpoints();
        }

        app.UseHttpsRedirection();

        app.Run();
    }
    
    private static WebApplicationBuilder ConfigureDefaultServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
        builder.Services.AddOpenApi();
        builder.AddServiceDefaults();
        
        return builder;
    }
}