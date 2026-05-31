using System.ComponentModel.DataAnnotations;
using IngredientIQ.Domain.Enums;

namespace IngredientIQ.Domain.Entities;

public class Ingredient
{
    public Guid Id { get; set; }

    [Required, MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    public List<string> Synonyms { get; set; } = [];

    [MaxLength(50)]
    public string? CasNumber { get; set; }

    public IngredientCategory Category { get; set; } = IngredientCategory.Other;

    [Range(1, 10)]
    public int SafetyRating { get; set; } = 5;

    public List<RiskTag> RiskTags { get; set; } = [];

    public List<string> SourceReferences { get; set; } = [];

    public IngredientStatus Status { get; set; } = IngredientStatus.Active;

    public Guid? CreatedBy { get; set; }

    public Guid? ApprovedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
