using IngredientIQ.Domain.Validation;
using IngredientIQ.Services;
using IngredientIQ.Services.Interfaces;
using IngredientIQ.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IngredientIQ.Pages.Scan;

public class UploadModel : PageModel
{
    private readonly IOcrService _ocr;
    private readonly IngredientTextParser _parser;

    public UploadModel(IOcrService ocr, IngredientTextParser parser)
    {
        _ocr = ocr;
        _parser = parser;
    }

    [BindProperty]
    public IFormFile? ImageFile { get; set; }

    [BindProperty]
    public string? ManualText { get; set; }

    public string? ErrorMessage { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        // Guest scan limit check
        if (User.Identity?.IsAuthenticated != true)
        {
            var guestCount = HttpContext.Session.GetInt32("GuestScans") ?? 0;
            if (guestCount >= 3)
            {
                ErrorMessage = "You've used all 3 free scans. Please register to continue.";
                return Page();
            }
        }

        string rawText;
        decimal confidence;

        if (ImageFile is not null && ImageFile.Length > 0)
        {
            // Validate image
            var validation = ImageValidator.Validate(
                ImageFile.FileName, ImageFile.Length, ImageFile.ContentType);
            if (!validation.IsValid)
            {
                ErrorMessage = validation.Errors[0].Message;
                return Page();
            }

            using var stream = ImageFile.OpenReadStream();
            var ocrResult = await _ocr.ExtractTextAsync(stream);
            rawText = ocrResult.ExtractedText;
            confidence = ocrResult.Confidence;
        }
        else if (!string.IsNullOrWhiteSpace(ManualText))
        {
            rawText = ManualText;
            confidence = 100;
        }
        else
        {
            ErrorMessage = "Please upload an image or enter ingredients manually.";
            return Page();
        }

        // Parse ingredients
        var ingredients = _parser.Parse(rawText);

        // Store in TempData for Review page
        TempData["RawText"] = rawText;
        TempData["OcrConfidence"] = confidence.ToString("F0");
        TempData["ParsedIngredients"] = string.Join("\n", ingredients);

        return RedirectToPage("/Scan/Review");
    }
}
