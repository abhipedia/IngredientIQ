using IngredientIQ.Data;
using IngredientIQ.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using ScanEntity = IngredientIQ.Domain.Entities.Scan;

namespace IngredientIQ.Pages.History;

public class IndexModel : PageModel
{
    private readonly IngredientIQDbContext _db;
    private readonly IAuthService _auth;
    private const int PageSize = 15;

    public IndexModel(IngredientIQDbContext db, IAuthService auth)
    {
        _db = db;
        _auth = auth;
    }

    public List<ScanEntity> Scans { get; set; } = [];
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }

    public async Task OnGetAsync(int page = 1)
    {
        var user = await _auth.GetCurrentUserAsync(User);
        if (user is null) return;

        CurrentPage = Math.Max(1, page);

        var query = _db.Scans
            .Where(s => s.UserId == user.Id)
            .OrderByDescending(s => s.CreatedAt)
            .AsNoTracking();

        var total = await query.CountAsync();
        TotalPages = (int)Math.Ceiling(total / (double)PageSize);
        CurrentPage = Math.Min(CurrentPage, Math.Max(1, TotalPages));

        Scans = await query
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }
}
