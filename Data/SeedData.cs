using IngredientIQ.Domain.Entities;
using IngredientIQ.Domain.Enums;

namespace IngredientIQ.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IngredientIQDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<IngredientIQDbContext>>();

        context.Database.EnsureCreated();

        SeedAdminUser(context, logger);
        SeedIngredients(context, logger);
    }

    private static void SeedAdminUser(IngredientIQDbContext context, ILogger logger)
    {
        if (context.Users.Any(u => u.Role == UserRole.SuperAdmin))
            return;

        var admin = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@ingredientiq.com",
            FirstName = "Admin",
            LastName = "User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123!"),
            Role = UserRole.SuperAdmin,
            IsEmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(admin);
        context.SaveChanges();
        logger.LogInformation("Seeded admin user: admin@ingredientiq.com / Admin@123!");
    }

    private static void SeedIngredients(IngredientIQDbContext context, ILogger logger)
    {
        if (context.Ingredients.Any())
            return;

        var ingredients = BuildIngredientList();
        context.Ingredients.AddRange(ingredients);
        context.SaveChanges();
        logger.LogInformation("Seeded {Count} ingredients.", ingredients.Count);
    }

    private static List<Ingredient> BuildIngredientList()
    {
        var now = DateTime.UtcNow;

        return
        [
            // ===== BENEFICIAL (Rating 8–10) =====
            Ing("Water", 10, IngredientCategory.Other, [RiskTag.Natural],
                synonyms: ["aqua", "purified water", "deionized water"]),
            Ing("Vitamin E", 9, IngredientCategory.Vitamin, [RiskTag.Natural, RiskTag.Vegan],
                synonyms: ["tocopherol", "alpha-tocopherol", "tocopheryl acetate"],
                cas: "59-02-9"),
            Ing("Vitamin C", 9, IngredientCategory.Vitamin, [RiskTag.Natural, RiskTag.Vegan],
                synonyms: ["ascorbic acid", "l-ascorbic acid", "sodium ascorbate"],
                cas: "50-81-7"),
            Ing("Aloe Vera", 9, IngredientCategory.SkincareActive, [RiskTag.Natural, RiskTag.Vegan, RiskTag.Organic],
                synonyms: ["aloe barbadensis", "aloe barbadensis leaf juice"]),
            Ing("Shea Butter", 9, IngredientCategory.Fat, [RiskTag.Natural, RiskTag.Vegan],
                synonyms: ["butyrospermum parkii", "butyrospermum parkii butter"]),
            Ing("Glycerin", 8, IngredientCategory.SkincareActive, [RiskTag.Natural],
                synonyms: ["glycerol", "vegetable glycerin"]),
            Ing("Hyaluronic Acid", 9, IngredientCategory.SkincareActive, [RiskTag.Natural],
                synonyms: ["sodium hyaluronate", "ha"],
                cas: "9004-61-9"),
            Ing("Green Tea Extract", 9, IngredientCategory.SkincareActive, [RiskTag.Natural, RiskTag.Vegan, RiskTag.Organic],
                synonyms: ["camellia sinensis", "camellia sinensis leaf extract"]),
            Ing("Jojoba Oil", 9, IngredientCategory.Fat, [RiskTag.Natural, RiskTag.Vegan],
                synonyms: ["simmondsia chinensis", "simmondsia chinensis seed oil"]),
            Ing("Coconut Oil", 8, IngredientCategory.Fat, [RiskTag.Natural, RiskTag.Vegan],
                synonyms: ["cocos nucifera", "cocos nucifera oil"]),
            Ing("Niacinamide", 9, IngredientCategory.Vitamin, [RiskTag.Natural],
                synonyms: ["vitamin b3", "nicotinamide"],
                cas: "98-92-0"),
            Ing("Zinc Oxide", 8, IngredientCategory.Mineral, [RiskTag.Natural],
                synonyms: ["zno"],
                cas: "1314-13-2"),
            Ing("Oat Extract", 8, IngredientCategory.SkincareActive, [RiskTag.Natural, RiskTag.Vegan],
                synonyms: ["avena sativa", "colloidal oatmeal"]),
            Ing("Chamomile", 8, IngredientCategory.SkincareActive, [RiskTag.Natural, RiskTag.Vegan, RiskTag.Organic],
                synonyms: ["chamomilla recutita", "matricaria chamomilla", "bisabolol"]),
            Ing("Rosehip Oil", 9, IngredientCategory.Fat, [RiskTag.Natural, RiskTag.Vegan, RiskTag.Organic],
                synonyms: ["rosa canina", "rosa canina fruit oil"]),
            Ing("Vitamin A", 8, IngredientCategory.Vitamin, [RiskTag.Natural],
                synonyms: ["retinol", "retinyl palmitate"],
                cas: "68-26-8"),
            Ing("Panthenol", 8, IngredientCategory.Vitamin, [RiskTag.Natural],
                synonyms: ["provitamin b5", "d-panthenol", "vitamin b5"],
                cas: "81-13-0"),
            Ing("Argan Oil", 9, IngredientCategory.Fat, [RiskTag.Natural, RiskTag.Vegan],
                synonyms: ["argania spinosa", "argania spinosa kernel oil"]),

            // ===== NEUTRAL (Rating 4–7) =====
            Ing("Citric Acid", 7, IngredientCategory.Preservative, [RiskTag.Natural, RiskTag.Vegan],
                synonyms: ["e330"],
                cas: "77-92-9"),
            Ing("Sodium Chloride", 6, IngredientCategory.Mineral, [RiskTag.Natural],
                synonyms: ["salt", "table salt", "sea salt"],
                cas: "7647-14-5"),
            Ing("Xanthan Gum", 6, IngredientCategory.Emulsifier, [RiskTag.Natural, RiskTag.Vegan],
                synonyms: ["e415"],
                cas: "11138-66-2"),
            Ing("Lecithin", 6, IngredientCategory.Emulsifier, [RiskTag.Natural],
                synonyms: ["soy lecithin", "sunflower lecithin", "e322"]),
            Ing("Pectin", 6, IngredientCategory.Additive, [RiskTag.Natural, RiskTag.Vegan],
                synonyms: ["e440"],
                cas: "9000-69-5"),
            Ing("Gelatin", 5, IngredientCategory.Protein, [],
                synonyms: ["gelatine"]),
            Ing("Stearic Acid", 5, IngredientCategory.Emulsifier, [RiskTag.Natural],
                synonyms: ["octadecanoic acid"],
                cas: "57-11-4"),
            Ing("Cetyl Alcohol", 5, IngredientCategory.Emulsifier, [],
                synonyms: ["cetearyl alcohol", "1-hexadecanol"],
                cas: "36653-82-4"),
            Ing("Dimethicone", 5, IngredientCategory.SkincareActive, [RiskTag.Artificial],
                synonyms: ["polydimethylsiloxane", "pdms"],
                cas: "9006-65-9"),
            Ing("Titanium Dioxide", 5, IngredientCategory.Coloring, [],
                synonyms: ["tio2", "e171", "ci 77891"],
                cas: "13463-67-7"),
            Ing("Fragrance", 4, IngredientCategory.Flavoring, [RiskTag.Artificial, RiskTag.Allergen],
                synonyms: ["parfum", "aroma"]),
            Ing("Caffeine", 6, IngredientCategory.SkincareActive, [RiskTag.Natural],
                synonyms: ["1,3,7-trimethylxanthine"],
                cas: "58-08-2"),
            Ing("Phenoxyethanol", 5, IngredientCategory.Preservative, [RiskTag.Artificial],
                synonyms: ["2-phenoxyethanol"],
                cas: "122-99-6"),
            Ing("Sodium Benzoate", 5, IngredientCategory.Preservative, [RiskTag.Artificial],
                synonyms: ["e211"],
                cas: "532-32-1"),
            Ing("Potassium Sorbate", 5, IngredientCategory.Preservative, [],
                synonyms: ["e202"],
                cas: "24634-61-5"),
            Ing("Propylene Glycol", 4, IngredientCategory.SkincareActive, [RiskTag.Artificial],
                synonyms: ["1,2-propanediol", "pg"],
                cas: "57-55-6"),

            // ===== HARMFUL (Rating 1–3) =====
            Ing("Benzene", 1, IngredientCategory.Other, [RiskTag.Carcinogen, RiskTag.Artificial],
                cas: "71-43-2"),
            Ing("Formaldehyde", 1, IngredientCategory.Preservative, [RiskTag.Carcinogen, RiskTag.Artificial],
                synonyms: ["formalin", "methanal", "formol"],
                cas: "50-00-0"),
            Ing("Parabens", 3, IngredientCategory.Preservative, [RiskTag.EndocrineDisruptor, RiskTag.Artificial],
                synonyms: ["methylparaben", "propylparaben", "butylparaben", "ethylparaben"]),
            Ing("BHA", 2, IngredientCategory.Preservative, [RiskTag.Carcinogen, RiskTag.Artificial],
                synonyms: ["butylated hydroxyanisole", "e320"],
                cas: "25013-16-5"),
            Ing("BHT", 3, IngredientCategory.Preservative, [RiskTag.EndocrineDisruptor, RiskTag.Artificial],
                synonyms: ["butylated hydroxytoluene", "e321"],
                cas: "128-37-0"),
            Ing("Triclosan", 2, IngredientCategory.Preservative, [RiskTag.EndocrineDisruptor, RiskTag.Artificial],
                synonyms: ["irgasan"],
                cas: "3380-34-5"),
            Ing("Phthalates", 2, IngredientCategory.Other, [RiskTag.EndocrineDisruptor, RiskTag.Artificial],
                synonyms: ["diethyl phthalate", "dep", "dbp", "dehp"]),
            Ing("Lead", 1, IngredientCategory.Mineral, [RiskTag.Carcinogen],
                synonyms: ["lead acetate", "pb"],
                cas: "7439-92-1"),
            Ing("Mercury", 1, IngredientCategory.Mineral, [RiskTag.Carcinogen],
                synonyms: ["thimerosal", "mercuric chloride", "hg"],
                cas: "7439-97-6"),
            Ing("Sodium Lauryl Sulfate", 3, IngredientCategory.Emulsifier, [RiskTag.Artificial, RiskTag.Allergen],
                synonyms: ["sls", "sodium dodecyl sulfate", "sds"],
                cas: "151-21-3"),
            Ing("Hydroquinone", 2, IngredientCategory.SkincareActive, [RiskTag.Carcinogen, RiskTag.Artificial],
                cas: "123-31-9"),
            Ing("Toluene", 1, IngredientCategory.Other, [RiskTag.Carcinogen, RiskTag.Artificial],
                synonyms: ["methylbenzene"],
                cas: "108-88-3"),
            Ing("Asbestos", 1, IngredientCategory.Other, [RiskTag.Carcinogen],
                synonyms: ["chrysotile", "tremolite"]),
            Ing("Coal Tar", 1, IngredientCategory.Coloring, [RiskTag.Carcinogen, RiskTag.Artificial],
                synonyms: ["coal tar solution"]),
            Ing("PFAS", 2, IngredientCategory.Other, [RiskTag.Carcinogen, RiskTag.Artificial],
                synonyms: ["ptfe", "perfluorooctanoic acid", "pfoa", "pfos"]),
        ];
    }

    private static Ingredient Ing(
        string name, int rating, IngredientCategory category, List<RiskTag> riskTags,
        List<string>? synonyms = null, string? cas = null)
    {
        return new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = name,
            SafetyRating = rating,
            Category = category,
            RiskTags = riskTags,
            Synonyms = synonyms ?? [],
            CasNumber = cas,
            SourceReferences = [],
            Status = IngredientStatus.Active,
            CreatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };
    }
}
