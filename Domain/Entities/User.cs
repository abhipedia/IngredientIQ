using System.ComponentModel.DataAnnotations;
using IngredientIQ.Domain.Enums;

namespace IngredientIQ.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    [Required, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.User;

    public List<string> Allergens { get; set; } = [];

    public bool IsEmailConfirmed { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLogin { get; set; }

    public bool IsDeleted { get; set; }

    // Navigation
    public ICollection<Scan> Scans { get; set; } = [];
}
