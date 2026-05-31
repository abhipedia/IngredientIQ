using FuzzySharp;
using IngredientIQ.Data;
using IngredientIQ.Domain.Entities;
using IngredientIQ.Domain.Enums;
using IngredientIQ.Services.Interfaces;
using IngredientIQ.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace IngredientIQ.Services;

public sealed class IngredientMatchingService : IIngredientMatchingService
{
    private const int FuzzyThreshold = 90;

    private readonly IngredientIQDbContext _db;

    public IngredientMatchingService(IngredientIQDbContext db)
    {
        _db = db;
    }

    public async Task<MatchResult> MatchAsync(string rawName)
    {
        var normalized = rawName.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(normalized))
            return new MatchResult { RawName = rawName, Confidence = 0 };

        // Load active ingredients once per request (scoped lifetime)
        var ingredients = await GetActiveIngredientsAsync();

        // 1) Exact match on name
        var exact = ingredients.FirstOrDefault(i =>
            i.Name.Equals(normalized, StringComparison.OrdinalIgnoreCase));
        if (exact is not null)
            return new MatchResult { RawName = rawName, Ingredient = exact, Confidence = 100 };

        // 2) Exact match on synonyms
        var synonymMatch = ingredients.FirstOrDefault(i =>
            i.Synonyms.Any(s => s.Equals(normalized, StringComparison.OrdinalIgnoreCase)));
        if (synonymMatch is not null)
            return new MatchResult { RawName = rawName, Ingredient = synonymMatch, Confidence = 100 };

        // 3) Fuzzy match on name and synonyms
        var bestScore = 0;
        Ingredient? bestMatch = null;

        foreach (var ingredient in ingredients)
        {
            var nameScore = Fuzz.Ratio(normalized, ingredient.Name.ToLowerInvariant());
            if (nameScore > bestScore)
            {
                bestScore = nameScore;
                bestMatch = ingredient;
            }

            foreach (var synonym in ingredient.Synonyms)
            {
                var synScore = Fuzz.Ratio(normalized, synonym.ToLowerInvariant());
                if (synScore > bestScore)
                {
                    bestScore = synScore;
                    bestMatch = ingredient;
                }
            }
        }

        if (bestScore >= FuzzyThreshold && bestMatch is not null)
            return new MatchResult { RawName = rawName, Ingredient = bestMatch, Confidence = bestScore };

        // No match
        return new MatchResult { RawName = rawName, Confidence = 0 };
    }

    public async Task<List<MatchResult>> MatchAllAsync(List<string> rawNames)
    {
        var results = new List<MatchResult>(rawNames.Count);
        foreach (var name in rawNames)
        {
            results.Add(await MatchAsync(name));
        }
        return results;
    }

    private Task<List<Ingredient>> GetActiveIngredientsAsync()
    {
        return _db.Ingredients
            .Where(i => i.Status == IngredientStatus.Active)
            .AsNoTracking()
            .ToListAsync();
    }
}
