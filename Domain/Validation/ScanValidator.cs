namespace IngredientIQ.Domain.Validation;

public static class ScanValidator
{
    public static ValidationResult ValidateIngredientList(IReadOnlyList<string> ingredients)
    {
        var result = new ValidationResult();

        if (ingredients is null || ingredients.Count == 0)
        {
            result.AddError(nameof(ingredients), "At least one ingredient is required.");
            return result;
        }

        for (var i = 0; i < ingredients.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(ingredients[i]))
            {
                result.AddError($"ingredients[{i}]", $"Ingredient at position {i + 1} is empty.");
            }
        }

        return result;
    }
}
