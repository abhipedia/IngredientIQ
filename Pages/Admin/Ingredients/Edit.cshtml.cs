using System.ComponentModel.DataAnnotations;
using IngredientIQ.Data;
using IngredientIQ.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IngredientIQ.Pages.Admin.Ingredients;

public class EditModel : PageModel
{
    private readonly IngredientIQDbContext _db;

    public EditModel(IngredientIQDbContext db) => _db = db;

    [BindProperty]
    public Guid Id { get; set; }

    [BindProperty, Required, MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    [BindProperty]
    public string? CasNumber { get; set; }

    [BindProperty]
    public IngredientCategory Category { get; set; }

    [BindProperty, Range(1, 10)]
    public int SafetyRating { get; set; } = 5;

    [BindProperty]
    public IngredientStatus Status { get; set; }

    [BindProperty]
    public string? SynonymsRaw { get; set; }

    [BindProperty]
    public List<RiskTag> SelectedRiskTags { get; set; } = [];

    [BindProperty]
    public string? SourceReferencesRaw { get; set; }

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var ing = await _db.Ingredients.FindAsync(id);
        if (ing is null) return NotFound();

        Id = ing.Id;
        Name = ing.Name;
        CasNumber = ing.CasNumber;
        Category = ing.Category;
        SafetyRating = ing.SafetyRating;
        Status = ing.Status;
        SynonymsRaw = string.Join('\n', ing.Synonyms);
        SelectedRiskTags = ing.RiskTags;
        SourceReferencesRaw = string.Join('\n', ing.SourceReferences);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var ing = await _db.Ingredients.FindAsync(Id);
        if (ing is null) return NotFound();

        var validation = Domain.Validation.IngredientValidator.Validate(Name, SafetyRating, CasNumber);
        if (!validation.IsValid)
        {
            ErrorMessage = string.Join(" ", validation.Errors.Select(e => e.Message));
            return Page();
        }

        ing.Name = Name.Trim();
        ing.CasNumber = CasNumber?.Trim();
        ing.Category = Category;
        ing.SafetyRating = SafetyRating;
        ing.Status = Status;
        ing.Synonyms = ParseLines(SynonymsRaw);
        ing.RiskTags = SelectedRiskTags.Where(t => t != RiskTag.None).ToList();
        ing.SourceReferences = ParseLines(SourceReferencesRaw);
        ing.LastUpdated = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return RedirectToPage("/Admin/Ingredients/Index");
    }

    private static List<string> ParseLines(string? raw) =>
        string.IsNullOrWhiteSpace(raw)
            ? []
            : raw.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
}
