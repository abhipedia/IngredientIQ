using System.ComponentModel.DataAnnotations;
using IngredientIQ.Data;
using IngredientIQ.Domain.Entities;
using IngredientIQ.Domain.Enums;
using IngredientIQ.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IngredientIQ.Pages.Admin.Ingredients;

public class CreateModel : PageModel
{
    private readonly IngredientIQDbContext _db;
    private readonly IAuthService _auth;

    public CreateModel(IngredientIQDbContext db, IAuthService auth)
    {
        _db = db;
        _auth = auth;
    }

    [BindProperty, Required, MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    [BindProperty]
    public string? CasNumber { get; set; }

    [BindProperty]
    public IngredientCategory Category { get; set; }

    [BindProperty, Range(1, 10)]
    public int SafetyRating { get; set; } = 5;

    [BindProperty]
    public IngredientStatus Status { get; set; } = IngredientStatus.Active;

    [BindProperty]
    public string? SynonymsRaw { get; set; }

    [BindProperty]
    public List<RiskTag> SelectedRiskTags { get; set; } = [];

    [BindProperty]
    public string? SourceReferencesRaw { get; set; }

    public string? ErrorMessage { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var validation = Domain.Validation.IngredientValidator.Validate(Name, SafetyRating, CasNumber);
        if (!validation.IsValid)
        {
            ErrorMessage = string.Join(" ", validation.Errors.Select(e => e.Message));
            return Page();
        }

        var user = await _auth.GetCurrentUserAsync(User);

        var ingredient = new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = Name.Trim(),
            CasNumber = CasNumber?.Trim(),
            Category = Category,
            SafetyRating = SafetyRating,
            Status = Status,
            Synonyms = ParseLines(SynonymsRaw),
            RiskTags = SelectedRiskTags.Where(t => t != RiskTag.None).ToList(),
            SourceReferences = ParseLines(SourceReferencesRaw),
            CreatedBy = user?.Id,
            CreatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        _db.Ingredients.Add(ingredient);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Admin/Ingredients/Index");
    }

    private static List<string> ParseLines(string? raw) =>
        string.IsNullOrWhiteSpace(raw)
            ? []
            : raw.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
}
