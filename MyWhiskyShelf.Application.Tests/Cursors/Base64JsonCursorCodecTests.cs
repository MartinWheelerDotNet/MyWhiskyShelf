using MyWhiskyShelf.Application.Codecs;
using MyWhiskyShelf.Application.Cursors;

namespace MyWhiskyShelf.Application.Tests.Cursors;

public class Base64JsonCursorCodecTests
{
    private readonly Base64JsonCursorCodec _codec = new();

    [Fact]
    public void When_EncodeAndDecodeRoundTrip_Expect_SuccessAndSamePayload()
    {
        var payload = new NameIdCursor("Name", Guid.NewGuid());

        var cursor = _codec.Encode(payload);
        var result = _codec.TryDecode<NameIdCursor>(cursor, out var decoded);

        Assert.Multiple(
            () => Assert.True(result),
            () => Assert.NotNull(decoded),
            () => Assert.Equal(payload.Name, decoded!.Name),
            () => Assert.Equal(payload.Id, decoded!.Id));
    }

    [Fact]
    public void When_TryDecodeWithNullCursor_Expect_TrueAndNullPayload()
    {
        var result = _codec.TryDecode<NameIdCursor>(null, out var decoded);

        Assert.Multiple(
            () => Assert.True(result),
            () => Assert.Null(decoded));
    }

    [Fact]
    public void When_TryDecodeWithEmptyCursor_Expect_TrueAndNullPayload()
    {
        var result = _codec.TryDecode<NameIdCursor>(string.Empty, out var decoded);

        Assert.Multiple(
            () => Assert.True(result),
            () => Assert.Null(decoded));
    }

    [Fact]
    public void When_TryDecodeWithInvalidBase64_Expect_FalseAndNullPayload()
    {
        var result = _codec.TryDecode<NameIdCursor>("not-base64!!", out var decoded);

        Assert.Multiple(
            () => Assert.False(result),
            () => Assert.Null(decoded));
    }

    [Fact]
    public void When_TryDecodeWithMalformedJson_Expect_FalseAndNullPayload()
    {
        var blob = Convert.ToBase64String("{thisIs: notJson"u8.ToArray());
        var result = _codec.TryDecode<NameIdCursor>(blob, out var decoded);

        Assert.Multiple(
            () => Assert.False(result),
            () => Assert.Null(decoded));
    }

    [Fact]
    public void When_TryDecodeWithDifferentShape_Expect_TrueWithDefaultInstance()
    {
        var cursor = _codec.Encode(new { Number = 42 });

        var result = _codec.TryDecode<NameIdCursor>(cursor, out var decoded);

        Assert.Multiple(
            () => Assert.True(result),
            () => Assert.NotNull(decoded),
            () => Assert.Null(decoded!.Name),
            () => Assert.Equal(Guid.Empty, decoded!.Id));
    }
}