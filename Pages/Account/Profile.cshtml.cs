using IngredientIQ.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IngredientIQ.Pages.Account;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly IAuthService _auth;

    public ProfileModel(IAuthService auth) => _auth = auth;

    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    [BindProperty]
    public string AllergensText { get; set; } = string.Empty;

    public bool Saved { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _auth.GetCurrentUserAsync(User);
        if (user is null) return RedirectToPage("/Account/Login");

        Email = user.Email;
        FullName = $"{user.FirstName} {user.LastName}";
        AllergensText = string.Join(", ", user.Allergens);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _auth.GetCurrentUserAsync(User);
        if (user is null) return RedirectToPage("/Account/Login");

        var allergens = AllergensText
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        await _auth.UpdateProfileAsync(user.Id, allergens);

        Email = user.Email;
        FullName = $"{user.FirstName} {user.LastName}";
        Saved = true;
        return Page();
    }
}
