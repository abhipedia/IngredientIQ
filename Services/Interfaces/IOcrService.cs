using IngredientIQ.Services.Models;

namespace IngredientIQ.Services.Interfaces;

public interface IOcrService
{
    Task<OcrExtractionResult> ExtractTextAsync(Stream imageStream);
}
