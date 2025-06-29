using System.Text.RegularExpressions;

namespace MangaUpdater.Services.Database.Helpers;

public static partial class ChapterNumberParser
{
    public static (int Major, int Minor, string Suffix) Parse(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            return (0, 0, "");

        var regex = ChapterNumberParserRegex();
        var match = regex.Match(number.Trim());

        if (!match.Success)
            return (0, 0, number);

        var major = int.Parse(match.Groups["major"].Value);

        var minor = match.Groups["minor"].Success
            ? int.Parse(match.Groups["minor"].Value)
            : 0;

        var suffix = match.Groups["suffix"].Value;

        return (major, minor, suffix);
    }

    [GeneratedRegex(@"^(?<major>\d+)(?:\.(?<minor>\d+))?(?<suffix>[a-zA-Z]*)$", RegexOptions.Compiled)]
    private static partial Regex ChapterNumberParserRegex();
}
