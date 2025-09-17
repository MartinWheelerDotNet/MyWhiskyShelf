namespace MyWhiskyShelf.Core.Extensions;

public static class StringExtensions
{
    public static string SanitizeForLog(this string value) => value
        .ReplaceLineEndings()
        .Replace(Environment.NewLine, "");
}