using System.Security.Claims;
using IngredientIQ.Domain.Entities;
using IngredientIQ.Services.Models;

namespace IngredientIQ.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string email, string password, string firstName, string lastName);
    Task<AuthResult> LoginAsync(string email, string password);
    Task<User?> GetCurrentUserAsync(ClaimsPrincipal principal);
    Task<bool> UpdateProfileAsync(Guid userId, List<string> allergens);
    ClaimsPrincipal CreateClaimsPrincipal(User user);
}
