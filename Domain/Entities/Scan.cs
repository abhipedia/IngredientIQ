using System.ComponentModel.DataAnnotations;
using IngredientIQ.Domain.Enums;

namespace IngredientIQ.Domain.Entities;

public class Scan
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    [MaxLength(2048)]
    public string? ImageUrl { get; set; }

    public string? ExtractedText { get; set; }

    [Range(0, 100)]
    public decimal OcrConfidence { get; set; }

    [Range(0, 100)]
    public int KleenScore { get; set; }

    public ScoreCategory ScoreCategory { get; set; }

    [MaxLength(50)]
    public string? ProductCategory { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User? User { get; set; }
    public ICollection<ScanIngredientDetail> IngredientDetails { get; set; } = [];
}
