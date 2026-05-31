using IngredientIQ.Services.Models;

namespace IngredientIQ.Services.Interfaces;

public interface IIngredientMatchingService
{
    Task<MatchResult> MatchAsync(string rawName);
    Task<List<MatchResult>> MatchAllAsync(List<string> rawNames);
}
