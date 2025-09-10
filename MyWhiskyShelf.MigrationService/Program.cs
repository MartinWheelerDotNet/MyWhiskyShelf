using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;
using MyWhiskyShelf.MigrationService;
using MyWhiskyShelf.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

var connStr = builder.Configuration.GetConnectionString("myWhiskyShelfDb")
              ?? throw new InvalidOperationException("Connection string not found");

builder.Services.AddDbContext<MyWhiskyShelfDbContext>(options =>
    options.UseNpgsql(connStr, npgsql =>
        npgsql.MigrationsAssembly("MyWhiskyShelf.Migrations")));

var host = builder.Build();
await host.RunAsync();