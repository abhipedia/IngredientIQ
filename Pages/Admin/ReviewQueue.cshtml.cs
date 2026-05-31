using IngredientIQ.Data;
using IngredientIQ.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IngredientIQ.Pages.Admin;

public class ReviewQueueModel : PageModel
{
    private readonly IngredientIQDbContext _db;

    public ReviewQueueModel(IngredientIQDbContext db) => _db = db;

    public List<Ingredient> PendingIngredients { get; set; } = [];

    public async Task OnGetAsync()
    {
        PendingIngredients = await _db.Ingredients
            .Where(i => i.Status == Domain.Enums.IngredientStatus.Review)
            .OrderBy(i => i.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostApproveAsync(Guid id)
    {
        var ing = await _db.Ingredients.FindAsync(id);
        if (ing is null) return NotFound();

        ing.Status = Domain.Enums.IngredientStatus.Active;
        ing.LastUpdated = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeprecateAsync(Guid id)
    {
        var ing = await _db.Ingredients.FindAsync(id);
        if (ing is null) return NotFound();

        ing.Status = Domain.Enums.IngredientStatus.Deprecated;
        ing.LastUpdated = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }
}
