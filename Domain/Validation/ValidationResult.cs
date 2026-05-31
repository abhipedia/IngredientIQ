namespace IngredientIQ.Domain.Validation;

public sealed class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<ValidationError> Errors { get; } = [];

    public static ValidationResult Success() => new();

    public static ValidationResult Failure(string field, string message)
    {
        var result = new ValidationResult();
        result.Errors.Add(new ValidationError(field, message));
        return result;
    }

    public void AddError(string field, string message)
        => Errors.Add(new ValidationError(field, message));
}

public sealed record ValidationError(string Field, string Message);
