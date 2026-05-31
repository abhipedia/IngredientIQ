using System.ComponentModel.DataAnnotations;
using IngredientIQ.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IngredientIQ.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly IAuthService _auth;

    public RegisterModel(IAuthService auth) => _auth = auth;

    [BindProperty, Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [BindProperty, Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [BindProperty, Required, Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [BindProperty, Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [BindProperty, Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _auth.RegisterAsync(Email, Password, FirstName, LastName);
        if (!result.Succeeded)
        {
            ErrorMessage = result.ErrorMessage;
            return Page();
        }

        return RedirectToPage("/Account/Login", new { registered = true });
    }
}
