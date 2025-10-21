using MyWhiskyShelf.Application.Codecs;
using MyWhiskyShelf.Application.Cursors;

namespace MyWhiskyShelf.Application.Tests.Cursors;

public class Base64JsonCursorCodecTests
{
    private readonly Base64JsonCursorCodec _codec = new();

    [Fact]
    public void When_EncodeAndDecodeRoundTrip_Expect_SuccessAndSamePayload()
    {
        var payload = new DistilleryQueryCursor("Name", "Pattern", Guid.NewGuid(), Guid.NewGuid());

        var cursor = _codec.Encode(payload);
        var result = _codec.TryDecode<DistilleryQueryCursor>(cursor, out var decoded);

        Assert.NotNull(decoded);
        Assert.Multiple(
            () => Assert.True(result),
            () => Assert.Equal(payload.AfterName, decoded.AfterName),
            () => Assert.Equal(payload.NameSearchPattern, decoded.NameSearchPattern),
            () => Assert.Equal(payload.CountryId, decoded.CountryId),
            () => Assert.Equal(payload.RegionId, decoded.RegionId));
    }

    [Fact]
    public void When_TryDecodeWithNullCursor_Expect_TrueAndNullPayload()
    {
        var result = _codec.TryDecode<DistilleryQueryCursor>(null, out var decoded);

        Assert.Multiple(
            () => Assert.True(result),
            () => Assert.Null(decoded));
    }

    [Fact]
    public void When_TryDecodeWithEmptyCursor_Expect_TrueAndNullPayload()
    {
        var result = _codec.TryDecode<DistilleryQueryCursor>(string.Empty, out var decoded);

        Assert.Multiple(
            () => Assert.True(result),
            () => Assert.Null(decoded));
    }

    [Fact]
    public void When_TryDecodeWithInvalidBase64_Expect_FalseAndNullPayload()
    {
        var result = _codec.TryDecode<DistilleryQueryCursor>("not-base64!!", out var decoded);

        Assert.Multiple(
            () => Assert.False(result),
            () => Assert.Null(decoded));
    }

    [Fact]
    public void When_TryDecodeWithMalformedJson_Expect_FalseAndNullPayload()
    {
        var blob = Convert.ToBase64String("{thisIs: notJson"u8.ToArray());
        var result = _codec.TryDecode<DistilleryQueryCursor>(blob, out var decoded);

        Assert.Multiple(
            () => Assert.False(result),
            () => Assert.Null(decoded));
    }

    [Fact]
    public void When_TryDecodeWithDifferentShape_Expect_TrueWithDefaultInstance()
    {
        var cursor = _codec.Encode(new { Number = 42 });

        var result = _codec.TryDecode<DistilleryQueryCursor>(cursor, out var decoded);

        Assert.NotNull(decoded);
        Assert.Multiple(
            () => Assert.True(result),
            () => Assert.Null(decoded.AfterName),
            () => Assert.Null(decoded.NameSearchPattern),
            () => Assert.Null(decoded.CountryId),
            () => Assert.Null(decoded.RegionId));
    }
}