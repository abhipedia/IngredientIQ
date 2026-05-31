using IngredientIQ.Services.Interfaces;
using IngredientIQ.Services.Models;
using Tesseract;

namespace IngredientIQ.Services;

/// <summary>
/// OCR service using Tesseract for local text extraction from images.
/// Expects tessdata folder with eng.traineddata at app root or configured path.
/// </summary>
public sealed class OcrService : IOcrService
{
    private readonly string _tessDataPath;
    private readonly ILogger<OcrService> _logger;

    public OcrService(IWebHostEnvironment env, ILogger<OcrService> logger)
    {
        _logger = logger;
        _tessDataPath = Path.Combine(env.ContentRootPath, "tessdata");
    }

    public async Task<OcrExtractionResult> ExtractTextAsync(Stream imageStream)
    {
        if (!Directory.Exists(_tessDataPath) ||
            !File.Exists(Path.Combine(_tessDataPath, "eng.traineddata")))
        {
            _logger.LogWarning(
                "Tesseract data not found at {Path}. Returning empty result. " +
                "Download eng.traineddata from https://github.com/tesseract-ocr/tessdata and place in tessdata/ folder.",
                _tessDataPath);

            return new OcrExtractionResult
            {
                ExtractedText = string.Empty,
                Confidence = 0
            };
        }

        // Read stream to memory (Tesseract needs a seekable/pixel source)
        using var ms = new MemoryStream();
        await imageStream.CopyToAsync(ms);
        var imageBytes = ms.ToArray();

        return await Task.Run(() => PerformOcr(imageBytes));
    }

    private OcrExtractionResult PerformOcr(byte[] imageBytes)
    {
        try
        {
            using var engine = new TesseractEngine(_tessDataPath, "eng", EngineMode.Default);
            using var pix = Pix.LoadFromMemory(imageBytes);
            using var page = engine.Process(pix);

            var text = page.GetText()?.Trim() ?? string.Empty;
            var confidence = (decimal)page.GetMeanConfidence() * 100;

            _logger.LogInformation(
                "OCR extracted {Length} chars with {Confidence:F1}% confidence.",
                text.Length, confidence);

            return new OcrExtractionResult
            {
                ExtractedText = text,
                Confidence = confidence
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tesseract OCR failed.");
            return new OcrExtractionResult
            {
                ExtractedText = string.Empty,
                Confidence = 0
            };
        }
    }
}
