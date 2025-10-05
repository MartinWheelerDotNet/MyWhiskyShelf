using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;

namespace MyWhiskyShelf.Migrations;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MyWhiskyShelfDbContext>
{
    public MyWhiskyShelfDbContext CreateDbContext(string[] args)
    {
        // Aspire sets this at runtime; for design-time you can provide it via user-secrets or env var
        var connStr = Environment.GetEnvironmentVariable("ConnectionStrings__MyWhiskyShelf");

        if (string.IsNullOrWhiteSpace(connStr))
            throw new InvalidOperationException(
                "No connection string found. Set 'ConnectionStrings__MyWhiskyShelf' as a user-secret.");

        var options = new DbContextOptionsBuilder<MyWhiskyShelfDbContext>()
            .UseNpgsql(connStr, npgsql => npgsql.UseVector())
            .Options;

        return new MyWhiskyShelfDbContext(options);
    }
}