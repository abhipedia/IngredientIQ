using System.Text.RegularExpressions;

namespace IngredientIQ.Domain.Validation;

public static partial class UserValidator
{
    private static readonly Regex EmailRegex = GetEmailRegex();

    // Min 8 chars, at least 1 uppercase, 1 digit, 1 special character
    private static readonly Regex PasswordRegex = GetPasswordRegex();

    public static ValidationResult ValidateRegistration(
        string email, string password, string firstName, string lastName)
    {
        var result = new ValidationResult();

        ValidateEmail(email, result);
        ValidatePassword(password, result);
        ValidateName(firstName, nameof(firstName), "First name", result);
        ValidateName(lastName, nameof(lastName), "Last name", result);

        return result;
    }

    public static void ValidateEmail(string email, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            result.AddError(nameof(email), "Email is required.");
            return;
        }

        if (email.Length > 256)
        {
            result.AddError(nameof(email), "Email must not exceed 256 characters.");
            return;
        }

        if (!EmailRegex.IsMatch(email))
        {
            result.AddError(nameof(email), "Email format is invalid.");
        }
    }

    public static void ValidatePassword(string password, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            result.AddError(nameof(password), "Password is required.");
            return;
        }

        if (!PasswordRegex.IsMatch(password))
        {
            result.AddError(nameof(password),
                "Password must be at least 8 characters and include an uppercase letter, a number, and a special character.");
        }
    }

    private static void ValidateName(string value, string field, string label, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result.AddError(field, $"{label} is required.");
            return;
        }

        if (value.Length > 100)
        {
            result.AddError(field, $"{label} must not exceed 100 characters.");
        }
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex GetEmailRegex();

    [GeneratedRegex(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$", RegexOptions.Compiled)]
    private static partial Regex GetPasswordRegex();
}
