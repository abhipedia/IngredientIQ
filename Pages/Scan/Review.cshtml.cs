using IngredientIQ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IngredientIQ.Pages.Scan;

public class ReviewModel : PageModel
{
    private readonly IngredientTextParser _parser;

    public ReviewModel(IngredientTextParser parser) => _parser = parser;

    public string RawText { get; set; } = string.Empty;
    public string OcrConfidence { get; set; } = "0";

    [BindProperty]
    public string IngredientsText { get; set; } = string.Empty;

    [BindProperty]
    public string ProductCategory { get; set; } = "Food";

    public IActionResult OnGet()
    {
        RawText = TempData["RawText"]?.ToString() ?? string.Empty;
        OcrConfidence = TempData["OcrConfidence"]?.ToString() ?? "0";
        IngredientsText = TempData["ParsedIngredients"]?.ToString() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(RawText) && string.IsNullOrWhiteSpace(IngredientsText))
            return RedirectToPage("/Scan/Upload");

        // Persist confidence across the POST round-trip
        TempData.Keep("OcrConfidence");

        return Page();
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(IngredientsText))
        {
            ModelState.AddModelError(nameof(IngredientsText), "Please enter at least one ingredient.");
            return Page();
        }

        // Re-parse the edited text to get clean list
        var ingredients = _parser.Parse(IngredientsText.Replace("\n", ","));

        TempData["FinalIngredients"] = string.Join("\n", ingredients);
        TempData["ProductCategory"] = ProductCategory;
        TempData["OcrConfidence"] = TempData["OcrConfidence"]?.ToString() ?? "0";

        return RedirectToPage("/Scan/Result");
    }
}
