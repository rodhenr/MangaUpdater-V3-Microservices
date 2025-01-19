using System.Globalization;
using System.Text.RegularExpressions;

namespace MangaUpdater.Shared.Extensions;

public static partial class StringExtensions
{
    public static decimal GetNumericPart(this string str)
    {
        var match = ChapterRegex().Match(str);
        if (match.Success)
        {
            return decimal.Parse(match.Value, CultureInfo.InvariantCulture);
        }

        return -1;
    }

    [GeneratedRegex(@"^\d+(\.\d+)?")]
    private static partial Regex ChapterRegex();
}