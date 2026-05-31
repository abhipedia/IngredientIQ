namespace IngredientIQ.Domain.Validation;

public static class IngredientValidator
{
    private static readonly HashSet<string> ValidProductCategories =
        new(StringComparer.OrdinalIgnoreCase) { "Food", "Skincare", "Supplement" };

    public static ValidationResult Validate(
        string name, int safetyRating, string? casNumber = null)
    {
        var result = new ValidationResult();

        ValidateName(name, result);
        ValidateSafetyRating(safetyRating, result);
        ValidateCasNumber(casNumber, result);

        return result;
    }

    public static void ValidateName(string name, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            result.AddError(nameof(name), "Ingredient name is required.");
            return;
        }

        if (name.Length > 256)
        {
            result.AddError(nameof(name), "Ingredient name must not exceed 256 characters.");
        }
    }

    public static void ValidateSafetyRating(int rating, ValidationResult result)
    {
        if (rating < 1 || rating > 10)
        {
            result.AddError(nameof(rating), "Safety rating must be between 1 and 10.");
        }
    }

    public static void ValidateCasNumber(string? casNumber, ValidationResult result)
    {
        if (casNumber is not null && casNumber.Length > 50)
        {
            result.AddError(nameof(casNumber), "CAS number must not exceed 50 characters.");
        }
    }

    public static void ValidateProductCategory(string? category, ValidationResult result)
    {
        if (category is not null && !ValidProductCategories.Contains(category))
        {
            result.AddError(nameof(category), "Product category must be Food, Skincare, or Supplement.");
        }
    }
}
