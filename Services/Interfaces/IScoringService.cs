using IngredientIQ.Services.Models;

namespace IngredientIQ.Services.Interfaces;

public interface IScoringService
{
    ScoringResult Calculate(List<MatchResult> matches, string? productCategory, List<string>? userAllergens);
}
