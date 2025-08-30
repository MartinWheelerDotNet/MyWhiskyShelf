using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyWhiskyShelf.Infrastructure.Persistence.Contexts;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MyWhiskyShelfDbContext>
{
    public MyWhiskyShelfDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<MyWhiskyShelfDbContext>();

        // For migrations creation only; not used at runtime.
        builder.UseNpgsql("Host=localhost;Port=5432;Database=myWhiskyShelfDb;Username=postgres;Password=postgres");

        return new MyWhiskyShelfDbContext(builder.Options);
    }
}