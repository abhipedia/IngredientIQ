using IngredientIQ.Data;
using IngredientIQ.Domain.Entities;
using IngredientIQ.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IngredientIQ.Pages.Admin.Ingredients;

public class IndexModel : PageModel
{
    private readonly IngredientIQDbContext _db;
    private const int PageSize = 20;

    public IndexModel(IngredientIQDbContext db) => _db = db;

    public List<Ingredient> Ingredients { get; set; } = [];
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public IngredientStatus? StatusFilter { get; set; }

    public async Task OnGetAsync(int page = 1)
    {
        CurrentPage = Math.Max(1, page);

        var query = _db.Ingredients.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(Search))
            query = query.Where(i => i.Name.Contains(Search));

        if (StatusFilter.HasValue)
            query = query.Where(i => i.Status == StatusFilter.Value);

        query = query.OrderBy(i => i.Name);

        var total = await query.CountAsync();
        TotalPages = (int)Math.Ceiling(total / (double)PageSize);
        CurrentPage = Math.Min(CurrentPage, Math.Max(1, TotalPages));

        Ingredients = await query
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }
}
