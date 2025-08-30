namespace MyWhiskyShelf.IntegrationTests.Comparers;

public sealed class HttpResponseMessageComparer : IEqualityComparer<HttpResponseMessage>
{
    private static readonly string[] InterestingHeaders = ["Location", "Content-Type", "ETag"];

    public bool Equals(HttpResponseMessage? x, HttpResponseMessage? y)
    {
        if (x is null || y is null) return x == y;
        if (x.StatusCode != y.StatusCode) return false;

        foreach (var h in InterestingHeaders)
        {
            var hx = TryGetHeader(x, h);
            var hy = TryGetHeader(y, h);
            if (!string.Equals(hx, hy, StringComparison.Ordinal)) return false;
        }

        var sx = x.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        var sy = y.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        return string.Equals(sx, sy, StringComparison.Ordinal);
    }

    public int GetHashCode(HttpResponseMessage obj)
    {
        return (int)obj.StatusCode;
    }

    private static string? TryGetHeader(HttpResponseMessage msg, string name)
    {
        return msg.Headers.TryGetValues(name, out var value) || msg.Content.Headers.TryGetValues(name, out value)
            ? string.Join(",", value)
            : null;
    }
}