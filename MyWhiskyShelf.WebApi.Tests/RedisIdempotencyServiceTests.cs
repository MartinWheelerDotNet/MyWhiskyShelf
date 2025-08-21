using System.Net;
using System.Text.Json;
using Moq;
using MyWhiskyShelf.WebApi.Models;
using MyWhiskyShelf.WebApi.Services;
using StackExchange.Redis;

namespace MyWhiskyShelf.WebApi.Tests;

public class RedisIdempotencyServiceTests
{
    private readonly Mock<IDatabase> _mockDatabase;
    private readonly RedisIdempotencyService _service;

    public RedisIdempotencyServiceTests()
    {
        var mockConnection = new Mock<IConnectionMultiplexer>();
        _mockDatabase = new Mock<IDatabase>();
        mockConnection
            .Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_mockDatabase.Object);

        _service = new RedisIdempotencyService(mockConnection.Object);
    }
    
    #region TryGetCachedResultAsync Tests
    
    [Fact]
    public async Task When_TryGetCachedResultAndKeyExistsAndIsValid_Expect_ResultReturned()
    {
        var idempotencyKey = Guid.NewGuid();
        var cachedResponse = new CachedResponse(
            (int) HttpStatusCode.OK,
            "This is some test content",
            "application/json",
            new Dictionary<string, string?[]>());
        
        _mockDatabase
            .Setup(db => db.StringGetAsync(idempotencyKey.ToString(), CommandFlags.None))
            .ReturnsAsync(JsonSerializer.SerializeToUtf8Bytes(cachedResponse))
            .Verifiable(Times.Once);
    
        var response = await _service.TryGetCachedResultAsync(idempotencyKey);
       
        Assert.Equivalent(cachedResponse, response);
        _mockDatabase.Verify();
    }
    
    [Fact]
    public async Task When_TryGetCachedResultAndKeyDoesNotExist_Expect_ReturnsNull()
    {
        var idempotencyKey = Guid.NewGuid();

        _mockDatabase
            .Setup(db => db.StringGetAsync(idempotencyKey.ToString(), CommandFlags.None))
            .ReturnsAsync(RedisValue.Null)
            .Verifiable(Times.Once);

        var result = await _service.TryGetCachedResultAsync(idempotencyKey);

        Assert.Multiple(
            () => Assert.Null(result),
            () => _mockDatabase.Verify());
    }

    [Fact]
    public async Task When_TryGetCachedResultAndCachedStringIsEmpty_ReturnsNull()
    {
        var idempotencyKey = Guid.NewGuid();
        
        _mockDatabase
            .Setup(db => db.StringGetAsync(idempotencyKey.ToString(), CommandFlags.None))
            .ReturnsAsync(string.Empty)
            .Verifiable(Times.Once);

        var result = await _service.TryGetCachedResultAsync(idempotencyKey);

        Assert.Null(result);
        
        Assert.Multiple(
            () => Assert.Null(result),
            () => _mockDatabase.Verify());
    }
    
    #endregion

    #region AddToCacheAsync Tests

    [Fact]
    public async Task When_AddToCache_Expect_DatabaseUpdated()
    {
        var idempotencyKey = Guid.NewGuid().ToString();
        var cachedResponse = new CachedResponse(200, "content", "application/json", new Dictionary<string, string?[]>());
        var serializedResponse = JsonSerializer.SerializeToUtf8Bytes(cachedResponse);
        _mockDatabase
            .Setup(db => db.StringSetAsync(idempotencyKey,
                serializedResponse,
                null,
                false,
                When.Always,
                CommandFlags.None))
            .ReturnsAsync(true)
            .Verifiable(Times.Once);
        
        await _service.AddToCacheAsync(idempotencyKey, 200, "content", "application/json", new Dictionary<string, string?[]>());
        
        _mockDatabase.Verify();
    }
    
    #endregion
}