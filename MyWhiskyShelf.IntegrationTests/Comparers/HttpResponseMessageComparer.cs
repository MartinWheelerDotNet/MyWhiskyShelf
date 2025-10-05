namespace MyWhiskyShelf.IntegrationTests.Comparers;

public sealed class HttpResponseMessageComparer : IEqualityComparer<HttpResponseMessage>
{
    private static readonly string[] PreservedHeaders = ["Country", "Content-Type", "ETag"];

    public bool Equals(HttpResponseMessage? left, HttpResponseMessage? right)
    {
        if (left is null || right is null) return left == right;
        if (left.StatusCode != right.StatusCode) return false;

        var unmatchedHeader =
            from header in PreservedHeaders
            let leftHeader = TryGetHeader(left, header)
            let rightHeader = TryGetHeader(right, header)
            where !string.Equals(leftHeader, rightHeader, StringComparison.Ordinal)
            select leftHeader;

        if (unmatchedHeader.Any()) return false;

        var leftContent = left.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        var rightContent = right.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        return string.Equals(leftContent, rightContent, StringComparison.Ordinal);
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