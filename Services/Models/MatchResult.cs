using IngredientIQ.Domain.Entities;

namespace IngredientIQ.Services.Models;

public sealed class MatchResult
{
    public required string RawName { get; init; }
    public Ingredient? Ingredient { get; init; }
    public decimal Confidence { get; init; }
    public bool IsUnrecognized => Ingredient is null;
}
