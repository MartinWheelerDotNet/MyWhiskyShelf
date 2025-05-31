using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Extensions;
using MyWhiskyShelf.Database.Services;

namespace MyWhiskyShelf.WebApi;

[ExcludeFromCodeCoverage]
internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication
            .CreateBuilder(args)
            .ConfigureDefaultServices();
        builder.UsePostgresDatabase();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapDefaultEndpoints();
            app.Services.EnsureDatabaseCreated();
        }

        app.UseHttpsRedirection();

        app.MapGet("/distilleries/", async (DistilleryReadService distilleryReadService) =>
            await distilleryReadService.GetAllDistilleriesAsync());

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

    private static void EnsureDatabaseCreated(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MyWhiskyShelfDbContext>();
        dbContext.Database.EnsureCreated();
    }
    
}