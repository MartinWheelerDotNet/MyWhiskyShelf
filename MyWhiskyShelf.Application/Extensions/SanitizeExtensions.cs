namespace MyWhiskyShelf.Application.Extensions;

public static class SanitizeExtensions
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

    public static string SanitizeForLog(this Guid value) => value.ToString().SanitizeForLog();
}