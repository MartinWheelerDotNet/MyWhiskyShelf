namespace MyWhiskyShelf.Application.Extensions;

public static class StringExtensions
{
    public static string SanitizeForLog(this string value)
    {
        return value
            .ReplaceLineEndings()
            .Replace(Environment.NewLine, "")
            .Replace("\0", "")
            .Replace("\t", " ")
            .Trim();
    }
}