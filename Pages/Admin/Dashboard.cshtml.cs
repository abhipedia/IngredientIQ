using IngredientIQ.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IngredientIQ.Pages.Admin;

public class DashboardModel : PageModel
{
    private readonly IngredientIQDbContext _db;

    public DashboardModel(IngredientIQDbContext db) => _db = db;

    public int TotalIngredients { get; set; }
    public int ActiveIngredients { get; set; }
    public int ReviewIngredients { get; set; }
    public int TotalScans { get; set; }
    public int TotalUsers { get; set; }
    public double AverageScore { get; set; }

    public async Task OnGetAsync()
    {
        TotalIngredients = await _db.Ingredients.IgnoreQueryFilters().CountAsync();
        ActiveIngredients = await _db.Ingredients.CountAsync(i => i.Status == Domain.Enums.IngredientStatus.Active);
        ReviewIngredients = await _db.Ingredients.CountAsync(i => i.Status == Domain.Enums.IngredientStatus.Review);
        TotalScans = await _db.Scans.CountAsync();
        TotalUsers = await _db.Users.CountAsync();
        AverageScore = await _db.Scans.AnyAsync()
            ? await _db.Scans.AverageAsync(s => (double)s.KleenScore)
            : 0;
    }
}
