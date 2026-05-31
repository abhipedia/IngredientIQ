using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using IngredientIQ.Services.Interfaces;
using IngredientIQ.Services.Models;

namespace IngredientIQ.Services;

/// <summary>
/// Calls an OpenAI-compatible or Anthropic-compatible LLM API with a structured
/// ingredient-analysis prompt grounded in scientific, regulatory, and clean-label databases.
/// Supports GPT (OpenAI) and Claude (Anthropic) via configuration.
/// </summary>
public sealed class AiAnalysisService : IAiAnalysisService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ILogger<AiAnalysisService> _logger;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public AiAnalysisService(
        IHttpClientFactory httpFactory,
        IConfiguration config,
        ILogger<AiAnalysisService> logger)
    {
        _http = httpFactory.CreateClient("AiAnalysis");
        _config = config;
        _logger = logger;
    }

    public async Task<AiAnalysisResult> AnalyzeAsync(List<string> ingredientNames, string productCategory, List<string>? userAllergens = null)
    {
        var provider = _config["AiAnalysis:Provider"]?.ToLowerInvariant() ?? "";
        var apiKey = _config["AiAnalysis:ApiKey"] ?? "";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogInformation("AI API key not configured. Generating manual prompt for user.");
            var manualPrompt = BuildManualPrompt(ingredientNames, productCategory, userAllergens);
            return new AiAnalysisResult
            {
                Success = false,
                Error = "AI analysis not configured.",
                ManualPrompt = manualPrompt
            };
        }

        var prompt = BuildPrompt(ingredientNames, productCategory, userAllergens);

        try
        {
            var rawJson = provider switch
            {
                "anthropic" or "claude" => await CallAnthropicAsync(apiKey, prompt),
                _ => await CallOpenAiAsync(apiKey, prompt)
            };

            return ParseResponse(rawJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI analysis call failed.");
            return new AiAnalysisResult { Success = false, Error = "AI analysis temporarily unavailable." };
        }
    }

    // ────────────────────────────────────────────
    //  PROMPT ENGINEERING
    // ────────────────────────────────────────────

    internal static string BuildPrompt(List<string> ingredientNames, string productCategory, List<string>? userAllergens = null)
    {
        var numberedList = string.Join(Environment.NewLine,
            ingredientNames.Select((n, i) => $"  {i + 1}. {n}"));

        var allergenSection = userAllergens?.Count > 0
            ? $"""

## USER-DECLARED SENSITIVITIES
The consumer has declared sensitivity to: {string.Join(", ", userAllergens)}.
In the allergenAudit section, specifically cross-reference EVERY ingredient against these declared sensitivities.
Flag derivatives, processing aids, and cross-contamination vectors for these specific allergens.
"""
            : "";

        return $$"""
You are a Food Labeling Compliance Inspector conducting market surveillance on a retail {{productCategory}} product.
Your mandate is to audit the ingredient list below for consumer safety and regulatory compliance.

You must be meticulous, authoritative, and legally precise. This is a food safety matter — errors can cause
allergic reactions, regulatory penalties, or consumer harm. Do NOT speculate. Do NOT fabricate data.
When uncertain, state "status not confirmed — verify with [specific source]".

## INGREDIENT LIST (as printed on label, position 1 = highest by weight per QUID rules)
{{numberedList}}{{allergenSection}}

## REGULATORY FRAMEWORK — AUDIT AGAINST ALL OF THESE

### Allergen Regulations (MANDATORY — audit every ingredient)
- **EU FIC Regulation 1169/2011 Annex II**: 14 mandatory allergens (cereals containing gluten, crustaceans,
  eggs, fish, peanuts, soybeans, milk/lactose, tree nuts, celery, mustard, sesame, sulphites >10mg/kg,
  lupin, molluscs) — must be emphasized in ingredient list (bold/CAPS)
- **US FALCPA + FASTER Act**: Big 9 allergens (milk, eggs, fish, shellfish, tree nuts, peanuts,
  wheat, soybeans, sesame) — must be declared in plain language
- **Health Canada**: Priority allergens (includes mustard, sulphites, sesame)
- **Codex Alimentarius General Standard 12-1981**: International allergen labeling baseline
- **HIDDEN ALLERGENS**: Check for allergen derivatives disguised under technical names
  (e.g., casein = milk, lecithin = soy, albumin = egg, hydrolyzed wheat protein = gluten)

### Food Additive Regulations
- **EU EC 1333/2008**: Approved food additives with E-numbers, usage limits, and permitted categories
- **FDA 21 CFR Parts 170-189**: GRAS substances, food additives requiring pre-market approval,
  color additives (21 CFR 73/74/82), prior-sanctioned substances
- **Codex GSFA (General Standard for Food Additives)**: INS numbers and maximum permitted levels
- **FSSAI (India)**: Food Safety and Standards (Food Products Standards and Food Additives) Regulations
- **JECFA ADI values**: Acceptable Daily Intake established by Joint FAO/WHO Expert Committee

### Contaminant & Safety Databases
- **IARC Monographs**: Carcinogenicity classification (Group 1, 2A, 2B, 3, not classifiable)
  — ONLY assign groups for substances actually evaluated and classified by IARC
- **EU Contaminant Regulation EC 1881/2006**: Maximum levels for contaminants in food
- **FDA Action Levels**: Lead, arsenic, mercury, cadmium limits in food
- **EFSA Scientific Opinions**: Risk assessments for specific additives (e.g., titanium dioxide)
- **NTP Report on Carcinogens**: "Known" vs "reasonably anticipated" carcinogens

### Label Compliance Rules
- **Ingredient ordering**: Descending order by weight at time of manufacture (EU FIC Art. 18,
  FDA 21 CFR 101.4) — flag if order appears implausible
- **QUID (Quantitative Ingredient Declaration)**: EU FIC Art. 22 — percentage required when
  ingredient appears in product name, is emphasized, or is essential to characterize
- **Compound ingredients**: Must list sub-ingredients if >2% of finished product (EU FIC Art. 18(4))
- **Additive class names**: Must be declared by category function + specific name or E-number
  (e.g., "Emulsifier: Soy Lecithin" not just "Lecithin")
- **"Natural" / "Clean" claims**: Must not be misleading per EU Claims Regulation EC 1924/2006
  and FDA 21 CFR 101.22

### Clean Label & Certification Bodies
- EWG Verified, COSMOS, NATRUE, USDA Organic, Non-GMO Project Verified
- Clean Label Project, Whole Foods Quality Standards

## REQUIRED JSON RESPONSE — RETURN ONLY VALID JSON

{
  "summary": "Executive audit summary: overall safety posture, key compliance findings, and critical flags (max 250 words).",
  "ingredientAnalysis": [
    {
      "name": "Ingredient exactly as listed on label",
      "safetyRating": 8,
      "why": "FDA GRAS (21 CFR 182.1), EU approved E-number E330, JECFA ADI 'not limited', EWG score 1. No allergen concerns.",
      "riskFlags": ["none"],
      "verdict": "safe"
    }
  ],
  "overallScore": 75,
  "allergenAudit": {
    "declaredAllergens": ["List of allergens properly declared on the label"],
    "hiddenAllergens": [
      {
        "ingredient": "Technical ingredient name on label",
        "allergenType": "The Big-14/Big-9 allergen it maps to (e.g., Milk, Soy, Wheat/Gluten)",
        "explanation": "Why this is an allergen concern (e.g., 'Casein is a milk protein — must be declared per FALCPA and EU FIC Annex II')"
      }
    ],
    "crossContaminationRisks": ["Ingredients commonly processed on shared equipment with major allergens"]
  },
  "labelCompliance": [
    {
      "issue": "Specific compliance deficiency found",
      "regulation": "Exact regulation reference (e.g., 'EU FIC 1169/2011 Art. 18(2)', 'FDA 21 CFR 101.4(a)')",
      "severity": "Critical | Major | Minor",
      "recommendation": "Corrective action needed"
    }
  ],
  "regulatoryFlags": [
    {
      "ingredient": "Name",
      "body": "FDA | EU/EFSA | IARC | FSSAI | Health Canada | Codex",
      "status": "Banned | Restricted | Under Review | Approved with Limits | GRAS",
      "detail": "Specific regulation, annex, or CFR section"
    }
  ],
  "interactions": ["Documented ingredient interactions relevant to safety (e.g., 'sodium benzoate + ascorbic acid can form benzene under heat/light')"],
  "cleanLabel": {
    "rating": "Clean | Mostly Clean | Moderate | Not Clean",
    "certificationEligibility": ["Which certifications this product could/could not qualify for and why"],
    "reasoning": "Assessment based on additive load, artificial ingredients, and processing level"
  },
  "populationWarnings": ["Age-specific (infants <12mo, children <3yr), pregnancy, medical conditions (PKU for aspartame, etc.)"],
  "alternatives": [
    {
      "forIngredient": "Concerning ingredient",
      "alternative": "Safer replacement",
      "reason": "Why preferable — cite regulatory status or toxicological advantage"
    }
  ]
}

## STRICT AUDIT RULES — VIOLATIONS WILL INVALIDATE THE ANALYSIS

1. Analyze EVERY ingredient. Skipping an ingredient is an audit failure.
2. The "why" field MUST cite at least one specific regulatory source with section/article number.
3. safetyRating scale: 1 (dangerous/banned) to 10 (safest/whole food). verdict: "safe" | "use-with-caution" | "avoid".
4. riskFlags — use ONLY: "carcinogen", "allergen", "hidden allergen", "endocrine disruptor",
   "unapproved additive", "banned substance", "exceeds limits", or "none".
5. ALLERGEN AUDIT IS MANDATORY: Check every ingredient against the EU Big-14 and US Big-9 allergen lists.
   Flag derivatives (casein→milk, lecithin→soy, albumin→egg, carmine→insect, etc.).
6. Do NOT assign IARC groups unless the substance appears in IARC Monographs volumes 1-134.
7. Do NOT fabricate E-numbers, CAS numbers, CFR sections, or ADI values.
8. If a regulation reference is uncertain, write the claim and append "(verify — [source])".
9. Check ingredient ORDERING plausibility: water/flour/sugar typically first in food products;
   flag if a minor additive appears before a major ingredient.
10. If the product is genuinely safe with no compliance issues, state that clearly.
    Do NOT manufacture concerns to appear thorough. False positives erode trust.
11. Max 5 alternatives, only for ingredients with safetyRating ≤ 3.
""";
    }

    /// <summary>
    /// Builds a concise, anti-hallucination prompt for manual use in public LLMs.
    /// Designed as a Food Labeling Compliance Inspector audit.
    /// </summary>
    internal static string BuildManualPrompt(List<string> ingredientNames, string productCategory, List<string>? userAllergens = null)
    {
        var numberedList = string.Join(Environment.NewLine,
            ingredientNames.Select((n, i) => $"  {i + 1}. {n}"));

        var allergenSection = userAllergens?.Count > 0
            ? $"""

PERSONAL SENSITIVITIES: I have declared sensitivity to: {string.Join(", ", userAllergens)}.
Cross-reference EVERY ingredient against these specific allergens — including derivatives,
processing aids, and cross-contamination vectors.
"""
            : "";

        return $$"""
Act as a Food Labeling Compliance Inspector conducting market surveillance on a retail {{productCategory}} product.
Your job is to audit the ingredient list below. Be meticulous, authoritative, and legally precise.
Do NOT speculate or fabricate data. When uncertain, state "not confirmed — verify with [source]".

INGREDIENT LIST (as printed on label, position 1 = highest concentration):

{{numberedList}}{{allergenSection}}

## AUDIT TASK 1: Per-Ingredient Safety Analysis

For EACH ingredient, provide:

1. **Safety Rating** (1-10, where 10 = safest whole food, 1 = banned/dangerous)
2. **Why** — cite SPECIFIC regulatory sources:
   - FDA status: GRAS? 21 CFR section? Requires pre-market approval?
   - EU status: E-number? Approved in EC 1333/2008? Any EFSA restrictions?
   - IARC classification — ONLY if actually classified (Group 1/2A/2B/3)
   - JECFA ADI (Acceptable Daily Intake) if established
   - Codex GSFA INS number if applicable
3. **Risk flags**: carcinogen | allergen | hidden allergen | endocrine disruptor | unapproved | none
4. **Verdict**: safe | use-with-caution | avoid

## AUDIT TASK 2: Allergen Compliance (MANDATORY)

Audit every ingredient against:
- **EU FIC 1169/2011 Annex II** (14 allergens: gluten cereals, crustaceans, eggs, fish, peanuts,
  soy, milk, tree nuts, celery, mustard, sesame, sulphites, lupin, molluscs)
- **US FALCPA + FASTER Act** (Big 9: milk, eggs, fish, shellfish, tree nuts, peanuts, wheat, soy, sesame)

Check for HIDDEN allergens — ingredients that are allergen derivatives under technical names:
- casein/whey/lactose → milk | lecithin → soy (unless sunflower) | albumin → egg
- hydrolyzed wheat protein/seitan → gluten | carmine/cochineal → insect
- anchovy extract → fish | gelatin → may be pork/beef/fish

## AUDIT TASK 3: Label Compliance Check

- Is the ingredient **ordering plausible** by descending weight? (EU FIC Art. 18, FDA 21 CFR 101.4)
- Are **additive class names** properly declared (function + name/E-number)?
- Any **compound ingredients** missing sub-ingredient breakdown (if >2% of product)?
- Any **QUID issues** (percentage declarations missing where required by EU FIC Art. 22)?
- Any **misleading claims** implied by ingredient naming?

## AUDIT TASK 4: Overall Assessment

- **Overall Safety Score** (0-100)
- **Clean Label Rating**: Clean | Mostly Clean | Moderate | Not Clean
- **Certification Eligibility**: USDA Organic, Non-GMO Project, EWG Verified, Clean Label Project — why or why not?
- **Top 3 Concerns** with regulation citations
- **Safer Alternatives** for any ingredient rated ≤ 3

## RULES — FOLLOW STRICTLY:

- Do NOT guess regulatory statuses. Write "not confirmed — verify with [source]" if uncertain.
- Do NOT assign IARC groups unless the substance is in IARC Monographs.
- Do NOT fabricate E-numbers, CAS numbers, CFR sections, or ADI values.
- Do NOT manufacture concerns for a safe product. False positives are an audit failure.
- Cite REAL, SPECIFIC regulation references (article numbers, annex numbers, CFR parts).
- Keep each ingredient analysis to 2-3 sentences. Be precise, not verbose.
""";
    }

    // ────────────────────────────────────────────
    //  API CALLS
    // ────────────────────────────────────────────

    private async Task<string> CallOpenAiAsync(string apiKey, string prompt)
    {
        var model = _config["AiAnalysis:Model"] ?? "gpt-4o-mini";
        var endpoint = _config["AiAnalysis:Endpoint"] ?? "https://api.openai.com/v1/chat/completions";

        var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = JsonContent.Create(new
        {
            model,
            messages = new[]
            {
                new { role = "system", content = "You are a Food Labeling Compliance Inspector conducting market surveillance. Respond with valid JSON only. Be meticulous and legally precise. Never fabricate regulatory data." },
                new { role = "user", content = prompt }
            },
            temperature = 0.1,
            max_tokens = 4000
        });

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("choices")[0]
                   .GetProperty("message")
                   .GetProperty("content")
                   .GetString() ?? "{}";
    }

    private async Task<string> CallAnthropicAsync(string apiKey, string prompt)
    {
        var model = _config["AiAnalysis:Model"] ?? "claude-sonnet-4-20250514";
        var endpoint = _config["AiAnalysis:Endpoint"] ?? "https://api.anthropic.com/v1/messages";

        var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        request.Headers.Add("x-api-key", apiKey);
        request.Headers.Add("anthropic-version", "2023-06-01");
        request.Content = JsonContent.Create(new
        {
            model,
            max_tokens = 4000,
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            system = "You are a Food Labeling Compliance Inspector conducting market surveillance. Respond with valid JSON only. Be meticulous and legally precise. Never fabricate regulatory data."
        });

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("content")[0]
                   .GetProperty("text")
                   .GetString() ?? "{}";
    }

    // ────────────────────────────────────────────
    //  RESPONSE PARSING
    // ────────────────────────────────────────────

    private AiAnalysisResult ParseResponse(string rawJson)
    {
        // Strip markdown code fences if LLM wraps response
        var json = rawJson.Trim();
        if (json.StartsWith("```"))
        {
            var firstNewline = json.IndexOf('\n');
            var lastFence = json.LastIndexOf("```");
            if (firstNewline > 0 && lastFence > firstNewline)
                json = json[(firstNewline + 1)..lastFence].Trim();
        }

        try
        {
            var parsed = JsonSerializer.Deserialize<AiAnalysisRawResponse>(json, JsonOpts);
            if (parsed is null)
                return new AiAnalysisResult { Success = false, Error = "Failed to parse AI response." };

            return new AiAnalysisResult
            {
                Success = true,
                Summary = parsed.Summary ?? "",
                OverallScore = parsed.OverallScore,
                IngredientAnalysis = parsed.IngredientAnalysis?.Select(ia => new IngredientAnalysisItem
                {
                    Name = ia.Name ?? "",
                    SafetyRating = ia.SafetyRating,
                    Why = ia.Why ?? "",
                    RiskFlags = ia.RiskFlags ?? [],
                    Verdict = ia.Verdict ?? "safe"
                }).ToList() ?? [],
                RegulatoryFlags = parsed.RegulatoryFlags?.Select(f => new RegulatoryFlag
                {
                    Ingredient = f.Ingredient ?? "",
                    Body = f.Body ?? "",
                    Status = f.Status ?? "",
                    Detail = f.Detail
                }).ToList() ?? [],
                Interactions = parsed.Interactions ?? [],
                CleanLabel = new CleanLabelAssessment
                {
                    Rating = parsed.CleanLabel?.Rating ?? "Unknown",
                    CertificationEligibility = parsed.CleanLabel?.CertificationEligibility ?? [],
                    Reasoning = parsed.CleanLabel?.Reasoning ?? ""
                },
                PopulationWarnings = parsed.PopulationWarnings ?? [],
                Alternatives = parsed.Alternatives?.Select(a => new AlternativeSuggestion
                {
                    ForIngredient = a.ForIngredient ?? "",
                    Alternative = a.Alternative ?? "",
                    Reason = a.Reason
                }).ToList() ?? [],
                AllergenAudit = new AllergenAudit
                {
                    DeclaredAllergens = parsed.AllergenAudit?.DeclaredAllergens ?? [],
                    HiddenAllergens = parsed.AllergenAudit?.HiddenAllergens?.Select(h => new HiddenAllergenFlag
                    {
                        Ingredient = h.Ingredient ?? "",
                        AllergenType = h.AllergenType ?? "",
                        Explanation = h.Explanation ?? ""
                    }).ToList() ?? [],
                    CrossContaminationRisks = parsed.AllergenAudit?.CrossContaminationRisks ?? []
                },
                LabelCompliance = parsed.LabelCompliance?.Select(lc => new LabelComplianceIssue
                {
                    Issue = lc.Issue ?? "",
                    Regulation = lc.Regulation ?? "",
                    Severity = lc.Severity ?? "Minor",
                    Recommendation = lc.Recommendation
                }).ToList() ?? []
            };
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse AI JSON response.");
            return new AiAnalysisResult { Success = false, Error = "AI returned an invalid response format." };
        }
    }

    // Internal DTOs for deserialization
    private sealed class AiAnalysisRawResponse
    {
        public string? Summary { get; set; }
        public int OverallScore { get; set; }
        public List<RawIngredientAnalysis>? IngredientAnalysis { get; set; }
        public List<RawRegulatoryFlag>? RegulatoryFlags { get; set; }
        public List<string>? Interactions { get; set; }
        public RawCleanLabel? CleanLabel { get; set; }
        public List<string>? PopulationWarnings { get; set; }
        public List<RawAlternative>? Alternatives { get; set; }
        public RawAllergenAudit? AllergenAudit { get; set; }
        public List<RawLabelCompliance>? LabelCompliance { get; set; }
    }

    private sealed class RawIngredientAnalysis
    {
        public string? Name { get; set; }
        public int SafetyRating { get; set; }
        public string? Why { get; set; }
        public List<string>? RiskFlags { get; set; }
        public string? Verdict { get; set; }
    }

    private sealed class RawRegulatoryFlag
    {
        public string? Ingredient { get; set; }
        public string? Body { get; set; }
        public string? Status { get; set; }
        public string? Detail { get; set; }
    }

    private sealed class RawCleanLabel
    {
        public string? Rating { get; set; }
        public List<string>? CertificationEligibility { get; set; }
        public string? Reasoning { get; set; }
    }

    private sealed class RawAlternative
    {
        public string? ForIngredient { get; set; }
        public string? Alternative { get; set; }
        public string? Reason { get; set; }
    }

    private sealed class RawAllergenAudit
    {
        public List<string>? DeclaredAllergens { get; set; }
        public List<RawHiddenAllergen>? HiddenAllergens { get; set; }
        public List<string>? CrossContaminationRisks { get; set; }
    }

    private sealed class RawHiddenAllergen
    {
        public string? Ingredient { get; set; }
        public string? AllergenType { get; set; }
        public string? Explanation { get; set; }
    }

    private sealed class RawLabelCompliance
    {
        public string? Issue { get; set; }
        public string? Regulation { get; set; }
        public string? Severity { get; set; }
        public string? Recommendation { get; set; }
    }
}
