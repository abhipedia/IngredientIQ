using System.ComponentModel.DataAnnotations;

namespace IngredientIQ.Domain.Entities;

public class ScanIngredientDetail
{
    public Guid Id { get; set; }

    public Guid ScanId { get; set; }

    public Guid? IngredientId { get; set; }

    [Required, MaxLength(256)]
    public string RawName { get; set; } = string.Empty;

    [Range(0, 100)]
    public decimal MatchConfidence { get; set; }

    public int PositionInList { get; set; }

    public bool IsRiskFlag { get; set; }

    [Range(0, 10)]
    public int SafetyRating { get; set; }

    // Navigation
    public Scan Scan { get; set; } = null!;
    public Ingredient? Ingredient { get; set; }
}
