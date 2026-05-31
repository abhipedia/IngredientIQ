using System.Text.RegularExpressions;

namespace IngredientIQ.Services;

public sealed partial class IngredientTextParser
{
    private static readonly Dictionary<string, string> Abbreviations = new(StringComparer.OrdinalIgnoreCase)
    {
        ["vit e"] = "vitamin e",
        ["vit c"] = "vitamin c",
        ["vit a"] = "vitamin a",
        ["vit b3"] = "vitamin b3",
        ["vit b5"] = "vitamin b5",
        ["vit d"] = "vitamin d",
        ["vit k"] = "vitamin k",
    };

    private static readonly char[] Separators = [',', ';', '•', '·', '|', '\n', '\r'];

    public List<string> Parse(string rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
            return [];

        // Detect the "INGREDIENTS:" section if present
        var text = ExtractIngredientsSection(rawText);

        // Split on separators
        var parts = text.Split(Separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var results = new List<string>(parts.Length);

        foreach (var part in parts)
        {
            var normalized = Normalize(part);
            if (!string.IsNullOrWhiteSpace(normalized))
                results.Add(normalized);
        }

        return results;
    }

    private static string ExtractIngredientsSection(string text)
    {
        // Look for "INGREDIENTS:" or "INGREDIENTS :" header and take everything after it
        var match = IngredientsHeaderRegex().Match(text);
        return match.Success ? text[match.Index..][match.Length..] : text;
    }

    private static string Normalize(string ingredient)
    {
        // Lowercase
        var result = ingredient.ToLowerInvariant().Trim();

        // Strip leading bullet numbers like "1." or "2)"
        result = LeadingNumberRegex().Replace(result, "").Trim();

        // Strip trailing periods
        result = result.TrimEnd('.');

        // Expand abbreviations
        foreach (var (abbr, full) in Abbreviations)
        {
            if (result.Equals(abbr, StringComparison.OrdinalIgnoreCase))
            {
                result = full;
                break;
            }
        }

        // Collapse whitespace
        result = MultipleSpacesRegex().Replace(result, " ");

        return result;
    }

    [GeneratedRegex(@"ingredients\s*:", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex IngredientsHeaderRegex();

    [GeneratedRegex(@"^\d+[\.\)]\s*", RegexOptions.Compiled)]
    private static partial Regex LeadingNumberRegex();

    [GeneratedRegex(@"\s{2,}", RegexOptions.Compiled)]
    private static partial Regex MultipleSpacesRegex();
}
