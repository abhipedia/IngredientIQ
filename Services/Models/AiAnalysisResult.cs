namespace IngredientIQ.Services.Models;

/// <summary>
/// Structured AI analysis result from LLM ingredient intelligence.
/// </summary>
public sealed class AiAnalysisResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }

    /// <summary>When AI is not configured, contains a ready-to-paste prompt for public LLMs.</summary>
    public string? ManualPrompt { get; init; }

    /// <summary>AI-determined overall product safety score (0-100).</summary>
    public int OverallScore { get; init; }

    /// <summary>Per-ingredient analysis with safety rating and reasoning.</summary>
    public List<IngredientAnalysisItem> IngredientAnalysis { get; init; } = [];

    /// <summary>One-paragraph executive summary of the product's ingredient profile.</summary>
    public string Summary { get; init; } = string.Empty;

    /// <summary>Regulatory standing across FDA, EU, FSSAI, Codex, etc.</summary>
    public List<RegulatoryFlag> RegulatoryFlags { get; init; } = [];

    /// <summary>Known interactions or synergies between ingredients in this list.</summary>
    public List<string> Interactions { get; init; } = [];

    /// <summary>Clean-label assessment referencing EWG, COSMOS, USDA Organic, etc.</summary>
    public CleanLabelAssessment CleanLabel { get; init; } = new();

    /// <summary>Specific population warnings (pregnancy, children, allergies).</summary>
    public List<string> PopulationWarnings { get; init; } = [];

    /// <summary>Suggested safer alternatives for concerning ingredients.</summary>
    public List<AlternativeSuggestion> Alternatives { get; init; } = [];

    /// <summary>Allergen audit: hidden allergens, derivatives, cross-contamination risks.</summary>
    public AllergenAudit AllergenAudit { get; init; } = new();

    /// <summary>Label compliance issues: ordering, QUID, formatting, missing declarations.</summary>
    public List<LabelComplianceIssue> LabelCompliance { get; init; } = [];
}

public sealed class RegulatoryFlag
{
    public required string Ingredient { get; init; }
    public required string Body { get; init; }       // e.g., "EU SCCS", "FDA", "Health Canada"
    public required string Status { get; init; }      // e.g., "Banned", "Restricted", "Under Review"
    public string? Detail { get; init; }
}

public sealed class CleanLabelAssessment
{
    public string Rating { get; init; } = "Unknown";  // e.g., "Clean", "Moderate", "Not Clean"
    public List<string> CertificationEligibility { get; init; } = [];  // e.g., "EWG Verified", "COSMOS Natural"
    public string Reasoning { get; init; } = string.Empty;
}

public sealed class AlternativeSuggestion
{
    public required string ForIngredient { get; init; }
    public required string Alternative { get; init; }
    public string? Reason { get; init; }
}

public sealed class IngredientAnalysisItem
{
    public required string Name { get; init; }
    public int SafetyRating { get; init; }
    public required string Why { get; init; }
    public List<string> RiskFlags { get; init; } = [];
    public required string Verdict { get; init; }  // "safe", "use-with-caution", "avoid"
}

/// <summary>
/// Allergen audit results from food labeling compliance analysis.
/// </summary>
public sealed class AllergenAudit
{
    public List<string> DeclaredAllergens { get; init; } = [];
    public List<HiddenAllergenFlag> HiddenAllergens { get; init; } = [];
    public List<string> CrossContaminationRisks { get; init; } = [];
}

public sealed class HiddenAllergenFlag
{
    public required string Ingredient { get; init; }
    public required string AllergenType { get; init; }  // e.g. "Milk", "Soy", "Wheat/Gluten"
    public required string Explanation { get; init; }    // e.g. "Casein is a milk-derived protein"
}

/// <summary>
/// Label compliance issue detected during regulatory audit.
/// </summary>
public sealed class LabelComplianceIssue
{
    public required string Issue { get; init; }
    public required string Regulation { get; init; }   // e.g. "EU FIC 1169/2011 Art. 18"
    public required string Severity { get; init; }     // "Critical", "Major", "Minor"
    public string? Recommendation { get; init; }
}
