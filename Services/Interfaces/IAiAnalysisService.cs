using IngredientIQ.Services.Models;

namespace IngredientIQ.Services.Interfaces;

public interface IAiAnalysisService
{
    /// <summary>
    /// Generates a structured AI analysis from raw ingredient names using an LLM.
    /// </summary>
    Task<AiAnalysisResult> AnalyzeAsync(List<string> ingredientNames, string productCategory, List<string>? userAllergens = null);
}
