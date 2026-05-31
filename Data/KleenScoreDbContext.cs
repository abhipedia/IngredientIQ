using System.Text.Json;
using IngredientIQ.Domain.Entities;
using IngredientIQ.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace IngredientIQ.Data;

public class IngredientIQDbContext : DbContext
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public IngredientIQDbContext(DbContextOptions<IngredientIQDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<Scan> Scans => Set<Scan>();
    public DbSet<ScanIngredientDetail> ScanIngredientDetails => Set<ScanIngredientDetail>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUser(modelBuilder);
        ConfigureIngredient(modelBuilder);
        ConfigureScan(modelBuilder);
        ConfigureScanIngredientDetail(modelBuilder);
        ConfigureAuditLog(modelBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyTimestamps();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyTimestamps()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity is User u && u.CreatedAt == default)
                        u.CreatedAt = now;
                    else if (entry.Entity is Ingredient i && i.CreatedAt == default)
                    {
                        i.CreatedAt = now;
                        i.LastUpdated = now;
                    }
                    else if (entry.Entity is Scan s && s.CreatedAt == default)
                        s.CreatedAt = now;
                    else if (entry.Entity is AuditLog a && a.ChangedAt == default)
                        a.ChangedAt = now;
                    break;

                case EntityState.Modified:
                    if (entry.Entity is Ingredient ing)
                        ing.LastUpdated = now;
                    break;
            }
        }
    }

    // --- Entity configurations ---

    private static void ConfigureUser(ModelBuilder mb)
    {
        mb.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).HasMaxLength(256).IsRequired();
            e.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            e.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);

            e.Property(u => u.Allergens)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, JsonOptions) ?? new List<string>())
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(StringListComparer());

            e.HasMany(u => u.Scans)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasQueryFilter(u => !u.IsDeleted);
        });
    }

    private static void ConfigureIngredient(ModelBuilder mb)
    {
        mb.Entity<Ingredient>(e =>
        {
            e.HasKey(i => i.Id);
            e.HasIndex(i => i.Name).IsUnique();
            e.Property(i => i.Name).HasMaxLength(256).IsRequired();
            e.Property(i => i.CasNumber).HasMaxLength(50);
            e.Property(i => i.Category).HasConversion<string>().HasMaxLength(30);
            e.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);

            e.Property(i => i.Synonyms)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, JsonOptions) ?? new List<string>())
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(StringListComparer());

            e.Property(i => i.RiskTags)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonOptions),
                    v => JsonSerializer.Deserialize<List<RiskTag>>(v, JsonOptions) ?? new List<RiskTag>())
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(RiskTagListComparer());

            e.Property(i => i.SourceReferences)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, JsonOptions) ?? new List<string>())
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(StringListComparer());

            e.HasIndex(i => i.Category);
            e.HasIndex(i => i.Status);
            e.HasIndex(i => i.SafetyRating);
        });
    }

    private static void ConfigureScan(ModelBuilder mb)
    {
        mb.Entity<Scan>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.ImageUrl).HasMaxLength(2048);
            e.Property(s => s.ProductCategory).HasMaxLength(50);
            e.Property(s => s.ScoreCategory).HasConversion<string>().HasMaxLength(10);
            e.Property(s => s.OcrConfidence).HasPrecision(5, 2);

            e.HasOne(s => s.User)
                .WithMany(u => u.Scans)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasMany(s => s.IngredientDetails)
                .WithOne(d => d.Scan)
                .HasForeignKey(d => d.ScanId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(s => s.UserId);
            e.HasIndex(s => s.CreatedAt);
        });
    }

    private static void ConfigureScanIngredientDetail(ModelBuilder mb)
    {
        mb.Entity<ScanIngredientDetail>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.RawName).HasMaxLength(256).IsRequired();
            e.Property(d => d.MatchConfidence).HasPrecision(5, 2);

            e.HasOne(d => d.Scan)
                .WithMany(s => s.IngredientDetails)
                .HasForeignKey(d => d.ScanId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(d => d.Ingredient)
                .WithMany()
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasIndex(d => d.ScanId);
            e.HasIndex(d => d.IngredientId);
        });
    }

    private static void ConfigureAuditLog(ModelBuilder mb)
    {
        mb.Entity<AuditLog>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.EntityType).HasMaxLength(100).IsRequired();
            e.Property(a => a.Action).HasMaxLength(50).IsRequired();
            e.Property(a => a.OldValues).HasColumnType("nvarchar(max)");
            e.Property(a => a.NewValues).HasColumnType("nvarchar(max)");

            e.HasIndex(a => a.EntityType);
            e.HasIndex(a => a.ChangedAt);
        });
    }

    // --- Value comparers for JSON lists (EF Core change tracking) ---

    private static ValueComparer<List<string>> StringListComparer() =>
        new(
            (a, b) => a != null && b != null && a.SequenceEqual(b),
            c => c.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
            c => c.ToList());

    private static ValueComparer<List<RiskTag>> RiskTagListComparer() =>
        new(
            (a, b) => a != null && b != null && a.SequenceEqual(b),
            c => c.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
            c => c.ToList());
}
