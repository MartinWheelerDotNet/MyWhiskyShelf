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
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var dbContext = new MyWhiskyShelfDbContext(options);

        dbContext.Set<TEntity>().AddRange(distilleryEntities);
        await dbContext.SaveChangesAsync();

        return dbContext;
    }

    public static async Task<MyWhiskyShelfDbContext> CreateFailingDbContextAsync<TEntity>(
        Type exceptionType, 
        params TEntity[] whiskyBottleEntities) where TEntity : class
    {
        var options = new DbContextOptionsBuilder<MyWhiskyShelfDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var dbContext = new FailingSaveChangesDbContext(options, exceptionType);
        
        // This adds to the entity tracking but does not save to the database as this would cause an exception.
        // This is sufficient as entities are retrieved using .Find() which checks the entity tracking first.
        await dbContext.AddRangeAsync(whiskyBottleEntities.ToList());
        
        
        return dbContext;
    }
}