using System.ComponentModel.DataAnnotations;

namespace IngredientIQ.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }

    [Required, MaxLength(100)]
    public string EntityType { get; set; } = string.Empty;

    public Guid EntityId { get; set; }

    [Required, MaxLength(50)]
    public string Action { get; set; } = string.Empty;

    public Guid? ChangedBy { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }
}
