using System.Security.Claims;
using IngredientIQ.Data;
using IngredientIQ.Domain.Entities;
using IngredientIQ.Domain.Validation;
using IngredientIQ.Services.Interfaces;
using IngredientIQ.Services.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace IngredientIQ.Services;

public sealed class AuthService : IAuthService
{
    private readonly IngredientIQDbContext _db;

    public AuthService(IngredientIQDbContext db)
    {
        _db = db;
    }

    public async Task<AuthResult> RegisterAsync(
        string email, string password, string firstName, string lastName)
    {
        var validation = UserValidator.ValidateRegistration(email, password, firstName, lastName);
        if (!validation.IsValid)
            return AuthResult.Fail(validation.Errors[0].Message);

        var normalizedEmail = email.Trim().ToLowerInvariant();

        var exists = await _db.Users.AnyAsync(u => u.Email == normalizedEmail);
        if (exists)
            return AuthResult.Fail("An account with this email already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12),
            IsEmailConfirmed = true, // MVP: auto-confirm
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return AuthResult.Success(user);
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return AuthResult.Fail("Email and password are required.");

        var normalizedEmail = email.Trim().ToLowerInvariant();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
        if (user is null)
            return AuthResult.Fail("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return AuthResult.Fail("Invalid email or password.");

        user.LastLogin = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return AuthResult.Success(user);
    }

    public async Task<User?> GetCurrentUserAsync(ClaimsPrincipal principal)
    {
        var idClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(idClaim, out var userId))
            return null;

        return await _db.Users.FindAsync(userId);
    }

    public async Task<bool> UpdateProfileAsync(Guid userId, List<string> allergens)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is null)
            return false;

        user.Allergens = allergens
            .Where(a => !string.IsNullOrWhiteSpace(a))
            .Select(a => a.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        await _db.SaveChangesAsync();
        return true;
    }

    public ClaimsPrincipal CreateClaimsPrincipal(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }
}
