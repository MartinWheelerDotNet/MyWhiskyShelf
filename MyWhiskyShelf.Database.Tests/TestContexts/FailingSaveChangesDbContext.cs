using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Database.Contexts;

namespace MyWhiskyShelf.Database.Tests.TestContexts;

public class FailingSaveChangesDbContext(DbContextOptions<MyWhiskyShelfDbContext> options, Type exceptionType)
    : MyWhiskyShelfDbContext(options)
{
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var exception = Activator.CreateInstance(exceptionType, exceptionType.Name) as Exception
                        ?? throw new InvalidOperationException("Could not create exception of the specified type");
        throw exception;
    }
}