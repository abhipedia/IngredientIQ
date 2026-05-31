using IngredientIQ.Data;
using IngredientIQ.Services;
using IngredientIQ.Services.Interfaces;
using IngredientIQ.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IngredientIQ.Pages.Scan;

public class ResultModel : PageModel
{
    private readonly IAuthService _auth;
    private readonly IAiAnalysisService _ai;
    private readonly IngredientIQDbContext _db;
    private readonly IngredientTextParser _parser;

    public ResultModel(
        IAuthService auth,
        IAiAnalysisService ai,
        IngredientIQDbContext db,
        IngredientTextParser parser)
    {
        _auth = auth;
        _ai = ai;
        _db = db;
        _parser = parser;
    }

    public AiAnalysisResult? AiAnalysis { get; set; }
    public List<string> IngredientNames { get; set; } = [];
    public string ProductCategory { get; set; } = "Food";
    public Guid? ScanId { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id.HasValue)
            return await LoadSavedScan(id.Value);

        var ingredientsRaw = TempData["FinalIngredients"]?.ToString();
        if (string.IsNullOrWhiteSpace(ingredientsRaw))
            return RedirectToPage("/Scan/Upload");

        ProductCategory = TempData["ProductCategory"]?.ToString() ?? "Food";
        var ocrConfidence = decimal.TryParse(TempData["OcrConfidence"]?.ToString(), out var oc) ? oc : 0;

        IngredientNames = ingredientsRaw
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        // AI-powered analysis (or manual prompt if not configured)
        var user = await _auth.GetCurrentUserAsync(User);
        AiAnalysis = await _ai.AnalyzeAsync(IngredientNames, ProductCategory, user?.Allergens);
        var score = AiAnalysis.Success ? AiAnalysis.OverallScore : 0;
        var scan = new Domain.Entities.Scan
        {
            Id = Guid.NewGuid(),
            UserId = user?.Id,
            ExtractedText = ingredientsRaw,
            OcrConfidence = ocrConfidence,
            KleenScore = score,
            ScoreCategory = score >= 70 ? Domain.Enums.ScoreCategory.Green
                          : score >= 40 ? Domain.Enums.ScoreCategory.Yellow
                          : Domain.Enums.ScoreCategory.Red,
            ProductCategory = ProductCategory,
            CreatedAt = DateTime.UtcNow
        };

        _db.Scans.Add(scan);
        await _db.SaveChangesAsync();
        ScanId = scan.Id;

        if (user is null)
        {
            var count = HttpContext.Session.GetInt32("GuestScans") ?? 0;
            HttpContext.Session.SetInt32("GuestScans", count + 1);
        }

        return Page();
    }

    private async Task<IActionResult> LoadSavedScan(Guid scanId)
    {
        var scan = await _db.Scans.FindAsync(scanId);
        if (scan is null)
        {
            ErrorMessage = "Scan not found.";
            return Page();
        }

        IngredientNames = _parser.Parse(scan.ExtractedText ?? string.Empty);
        ProductCategory = scan.ProductCategory ?? "Food";
        var user = await _auth.GetCurrentUserAsync(User);
        AiAnalysis = await _ai.AnalyzeAsync(IngredientNames, ProductCategory, user?.Allergens);
        ScanId = scan.Id;

        return Page();
    }
}
