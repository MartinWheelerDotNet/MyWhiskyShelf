using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Tests.TestContexts;

namespace MyWhiskyShelf.Database.Tests.TestHelpers;

[ExcludeFromCodeCoverage]
public static class MyWhiskyShelfContextBuilder
{
    public static async Task<MyWhiskyShelfDbContext> CreateDbContextAsync<TEntity>(params TEntity[] distilleryEntities) 
        where TEntity : class
    {
        var options = new DbContextOptionsBuilder<MyWhiskyShelfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        var dbContext = new MyWhiskyShelfDbContext(options);
        
        dbContext.Set<TEntity>().AddRange(distilleryEntities);
        await dbContext.SaveChangesAsync();
        
        return dbContext;
    }
    
    public static MyWhiskyShelfDbContext CreateFailingDbContextAsync(Type exceptionType) 
    {
        var options = new DbContextOptionsBuilder<MyWhiskyShelfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new FailingSaveChangesDbContext(options, exceptionType);
    }
}