namespace IngredientIQ.Services.Models;

public sealed class OcrExtractionResult
{
    public string ExtractedText { get; init; } = string.Empty;
    public decimal Confidence { get; init; }
}
