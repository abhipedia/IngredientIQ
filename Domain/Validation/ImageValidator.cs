namespace IngredientIQ.Domain.Validation;

public static class ImageValidator
{
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB
    private const int MinDimensionPx = 300;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/heic"
    };

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".heic"
    };

    public static ValidationResult Validate(string fileName, long fileSizeBytes, string? contentType)
    {
        var result = new ValidationResult();

        ValidateExtension(fileName, result);
        ValidateSize(fileSizeBytes, result);
        ValidateContentType(contentType, result);

        return result;
    }

    public static void ValidateExtension(string fileName, ValidationResult result)
    {
        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
        {
            result.AddError(nameof(fileName),
                "Supported image formats are JPG, PNG, WEBP, and HEIC.");
        }
    }

    public static void ValidateSize(long fileSizeBytes, ValidationResult result)
    {
        if (fileSizeBytes <= 0)
        {
            result.AddError(nameof(fileSizeBytes), "File is empty.");
            return;
        }

        if (fileSizeBytes > MaxFileSizeBytes)
        {
            result.AddError(nameof(fileSizeBytes), "File size must not exceed 10 MB.");
        }
    }

    public static void ValidateContentType(string? contentType, ValidationResult result)
    {
        if (contentType is not null && !AllowedContentTypes.Contains(contentType))
        {
            result.AddError(nameof(contentType),
                "Supported image formats are JPG, PNG, WEBP, and HEIC.");
        }
    }
}
