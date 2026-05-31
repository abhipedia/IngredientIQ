using System.Text;
using IngredientIQ.Domain.Enums;
using IngredientIQ.Services.Interfaces;
using IngredientIQ.Services.Models;

namespace IngredientIQ.Services;

public sealed class ScoringService : IScoringService
{
    private const int AllergenPenalty = 20;
    private const int NeutralRating = 5;

    private readonly IConfiguration _config;

    public ScoringService(IConfiguration config)
    {
        _config = config;
    }

    public ScoringResult Calculate(
        List<MatchResult> matches, string? productCategory, List<string>? userAllergens)
    {
        if (matches.Count == 0)
        {
            return new ScoringResult
            {
                Score = 0,
                Category = ScoreCategory.Red,
                Explanation = "No ingredients provided."
            };
        }

        var categoryFactor = GetCategoryFactor(productCategory);
        var allergenSet = new HashSet<string>(
            userAllergens ?? [], StringComparer.OrdinalIgnoreCase);

        var details = new List<IngredientScoreDetail>(matches.Count);
        var totalWeighted = 0m;
        var allergenCount = 0;
        var explanation = new StringBuilder();

        explanation.AppendLine($"Product category: {productCategory ?? "Food"} (factor: {categoryFactor:F1}x)");
        explanation.AppendLine($"Total ingredients: {matches.Count}");
        explanation.AppendLine();

        for (var i = 0; i < matches.Count; i++)
        {
            var match = matches[i];
            var position = i + 1;
            var posWeight = CalculatePositionWeight(position, matches.Count);
            var rating = match.Ingredient?.SafetyRating ?? NeutralRating;
            var riskTags = match.Ingredient?.RiskTags ?? [];

            // Check if ingredient is a user allergen
            var isAllergen = false;
            if (match.Ingredient is not null)
            {
                isAllergen = allergenSet.Contains(match.Ingredient.Name) ||
                             match.Ingredient.Synonyms.Any(s => allergenSet.Contains(s)) ||
                             riskTags.Contains(RiskTag.Allergen) &&
                             allergenSet.Any(a => match.RawName.Contains(a, StringComparison.OrdinalIgnoreCase));
            }

            var userRiskMultiplier = isAllergen ? 0.5m : 1.0m;
            if (isAllergen) allergenCount++;

            var weighted = rating * posWeight * categoryFactor * userRiskMultiplier;
            totalWeighted += weighted;

            var isRisk = rating <= 3;

            details.Add(new IngredientScoreDetail
            {
                RawName = match.RawName,
                MatchedName = match.Ingredient?.Name,
                SafetyRating = rating,
                PositionWeight = posWeight,
                WeightedScore = Math.Round(weighted, 2),
                RiskTags = riskTags,
                Confidence = match.Confidence,
                IsRisk = isRisk,
                IsUnrecognized = match.IsUnrecognized
            });

            explanation.AppendLine(
                $"  #{position} {match.RawName} → rating={rating}, posWeight={posWeight:F2}, " +
                $"catFactor={categoryFactor:F1}, riskMult={userRiskMultiplier:F1}, weighted={weighted:F2}" +
                (isAllergen ? " [ALLERGEN]" : "") +
                (match.IsUnrecognized ? " [UNKNOWN]" : ""));
        }

        var rawScore = (totalWeighted / matches.Count) * 10m;
        var allergenPenaltyTotal = allergenCount * AllergenPenalty;
        var finalScore = (int)Math.Clamp(Math.Round(rawScore - allergenPenaltyTotal), 0, 100);

        explanation.AppendLine();
        explanation.AppendLine($"Raw score: ({totalWeighted:F2} / {matches.Count}) × 10 = {rawScore:F1}");
        if (allergenPenaltyTotal > 0)
            explanation.AppendLine($"Allergen penalty: -{allergenPenaltyTotal} ({allergenCount} allergen(s) × {AllergenPenalty})");
        explanation.AppendLine($"Final score: {finalScore}");

        var category = finalScore switch
        {
            >= 80 => ScoreCategory.Green,
            >= 50 => ScoreCategory.Yellow,
            _ => ScoreCategory.Red
        };

        // Top 3 concerns: lowest-rated recognized ingredients
        var topConcerns = details
            .Where(d => !d.IsUnrecognized && d.SafetyRating <= 5)
            .OrderBy(d => d.SafetyRating)
            .ThenByDescending(d => d.PositionWeight)
            .Take(3)
            .Select(d => new TopConcern
            {
                Name = d.MatchedName ?? d.RawName,
                Rating = d.SafetyRating,
                RiskTags = d.RiskTags,
                Explanation = BuildConcernExplanation(d)
            })
            .ToList();

        return new ScoringResult
        {
            Score = finalScore,
            Category = category,
            Details = details,
            TopConcerns = topConcerns,
            Explanation = explanation.ToString()
        };
    }

    /// <summary>
    /// Linear interpolation: position 1 → 1.0, last position → 0.3.
    /// Single ingredient gets weight 1.0.
    /// </summary>
    private static decimal CalculatePositionWeight(int position, int total)
    {
        if (total <= 1) return 1.0m;
        return Math.Max(0.3m, 1.0m - (position - 1) * 0.7m / (total - 1));
    }

    private decimal GetCategoryFactor(string? productCategory)
    {
        return productCategory?.ToLowerInvariant() switch
        {
            "skincare" => _config.GetValue("KleenScore:SkincareFactor", 1.2m),
            "supplement" => _config.GetValue("KleenScore:SupplementFactor", 1.1m),
            _ => _config.GetValue("KleenScore:DefaultCategoryFactor", 1.0m)
        };
    }

    private static string BuildConcernExplanation(IngredientScoreDetail detail)
    {
        var parts = new List<string>();

        if (detail.RiskTags.Contains(RiskTag.Carcinogen))
            parts.Add("Known carcinogen — avoid if possible.");
        if (detail.RiskTags.Contains(RiskTag.EndocrineDisruptor))
            parts.Add("Potential endocrine disruptor.");
        if (detail.RiskTags.Contains(RiskTag.Allergen))
            parts.Add("Common allergen — may cause reactions.");

        if (detail.SafetyRating <= 2)
            parts.Add($"Very low safety rating ({detail.SafetyRating}/10).");
        else if (detail.SafetyRating <= 4)
            parts.Add($"Below-average safety rating ({detail.SafetyRating}/10).");

        return parts.Count > 0
            ? string.Join(" ", parts)
            : $"Safety rating: {detail.SafetyRating}/10.";
    }
}
