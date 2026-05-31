using IngredientIQ.Domain.Entities;

namespace IngredientIQ.Services.Models;

public sealed class AuthResult
{
    public bool Succeeded { get; init; }
    public string? ErrorMessage { get; init; }
    public User? User { get; init; }

    public static AuthResult Success(User user) => new() { Succeeded = true, User = user };
    public static AuthResult Fail(string message) => new() { Succeeded = false, ErrorMessage = message };
}
