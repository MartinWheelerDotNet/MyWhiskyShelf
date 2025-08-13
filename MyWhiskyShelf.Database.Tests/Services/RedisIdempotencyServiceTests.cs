
using Moq;
using MyWhiskyShelf.Database.Services;
using StackExchange.Redis;

namespace MyWhiskyShelf.Database.Tests.Services;

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

    [Fact]
    public async Task When_TryGetCachedResultAndKeyExistsAndIsValid_Expect_ReturnGuid()
    {
        var idempotencyKey = Guid.NewGuid();
        var expectedGuid = Guid.NewGuid();
        
        _mockDatabase
            .Setup(db => db.StringGetAsync(idempotencyKey.ToString(), CommandFlags.None))
            .ReturnsAsync(expectedGuid.ToString())
            .Verifiable(Times.Once);

        var result = await _service.TryGetCachedResult(idempotencyKey);
    
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(expectedGuid, result!.Value),
            () => _mockDatabase.Verify());
    }
    
    [Fact]
    public async Task When_TryGetCachedResultAndKeyExistsButIsNotGuid_Expect_ReturnsNull()
    {
        var idempotencyKey = Guid.NewGuid();
        const string invalidGuidString = "not-a-guid";
        
        _mockDatabase
            .Setup(db => db.StringGetAsync(idempotencyKey.ToString(), CommandFlags.None))
            .ReturnsAsync(invalidGuidString)
            .Verifiable(Times.Once);
        
        var result = await _service.TryGetCachedResult(idempotencyKey);

        Assert.Multiple(
            () => Assert.Null(result),
            () => _mockDatabase.Verify());
    }

    [Fact]
    public async Task When_TryGetCachedResultAndKeyDoesNotExist_Expect_ReturnsNull()
    {
        var idempotencyKey = Guid.NewGuid();

        _mockDatabase
            .Setup(db => db.StringGetAsync(idempotencyKey.ToString(), CommandFlags.None))
            .ReturnsAsync(RedisValue.Null)
            .Verifiable(Times.Once);

        var result = await _service.TryGetCachedResult(idempotencyKey);

        Assert.Multiple(
            () => Assert.Null(result),
            () => _mockDatabase.Verify());
    }

    [Fact]
    public async Task TryGetCachedResult_ShouldReturnNull_WhenCachedStringIsEmpty()
    {
        var idempotencyKey = Guid.NewGuid();
        
        _mockDatabase
            .Setup(db => db.StringGetAsync(idempotencyKey.ToString(), CommandFlags.None))
            .ReturnsAsync(string.Empty)
            .Verifiable(Times.Once);

        var result = await _service.TryGetCachedResult(idempotencyKey);

        Assert.Null(result);
        
        Assert.Multiple(
            () => Assert.Null(result),
            () => _mockDatabase.Verify());
    }
}