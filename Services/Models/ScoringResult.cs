using IngredientIQ.Domain.Enums;

namespace IngredientIQ.Services.Models;

public sealed class ScoringResult
{
    public int Score { get; init; }
    public ScoreCategory Category { get; init; }
    public List<IngredientScoreDetail> Details { get; init; } = [];
    public List<TopConcern> TopConcerns { get; init; } = [];
    public string Explanation { get; init; } = string.Empty;
}

public sealed class IngredientScoreDetail
{
    public required string RawName { get; init; }
    public string? MatchedName { get; init; }
    public int SafetyRating { get; init; }
    public decimal PositionWeight { get; init; }
    public decimal WeightedScore { get; init; }
    public List<RiskTag> RiskTags { get; init; } = [];
    public decimal Confidence { get; init; }
    public bool IsRisk { get; init; }
    public bool IsUnrecognized { get; init; }
}

public sealed class TopConcern
{
    public required string Name { get; init; }
    public int Rating { get; init; }
    public List<RiskTag> RiskTags { get; init; } = [];
    public required string Explanation { get; init; }
}
