# IngredientIQ 🧪

**AI-Powered Ingredient Analysis for Informed Consumer Decisions**

Transform ingredient transparency into actionable insights. IngredientIQ empowers consumers to make informed purchasing decisions by analyzing product ingredients with AI-driven compliance auditing.

---

## 🎯 Quick Overview

**What is IngredientIQ?**

IngredientIQ is a web platform that allows users to:
1. **Upload or manually enter** a product's ingredient list
2. **Get AI-powered analysis** using FDA, EU, IARC, and other regulatory frameworks
3. **Receive a safety score** with detailed ingredient breakdown and risk assessment
4. **Track personal allergens** and receive personalized risk alerts

**Key Features:**
- ✅ **AI Compliance Auditing** — Grounds analysis in 8+ regulatory frameworks (EU FIC 1169/2011, FALCPA, JECFA, FSSAI, IARC, EFSA, FDA 21 CFR, Codex)
- ✅ **Allergen Detection** — Identifies hidden allergens and derivatives (e.g., casein→milk, lecithin→soy)
- ✅ **Label Compliance Checking** — Flags ingredient ordering issues, missing QUID declarations, improper additive naming
- ✅ **Personalized Risk Assessment** — Adjusts scoring based on user-declared sensitivities
- ✅ **Manual & AI Analysis** — Works with or without configured LLM API
- ✅ **Admin Portal** — Ingredient management, analytics, approval workflows
- ✅ **Scan History** — Track and revisit past analyses
- ✅ **Mobile Responsive** — Seamless experience on all devices

---

## 🚀 Getting Started

### Prerequisites

- **.NET 10** SDK ([download](https://dotnet.microsoft.com/download))
- **SQL Server LocalDB** (comes with Visual Studio) or external SQL Server instance
- **Visual Studio 2026** (Enterprise/Community) or VS Code with C# extension
- **PowerShell 5+** or Bash terminal

### Local Setup (5 minutes)

#### 1. Clone & Navigate

```bash
git clone https://github.com/yourusername/IngredientIQ.git
cd IngredientIQ
```

#### 2. Install Dependencies

```bash
dotnet restore
```

#### 3. Setup Database

```bash
# Create initial migration (if not already present)
dotnet ef migrations add InitialCreate --project IngredientIQ

# Apply migrations to LocalDB
dotnet ef database update --project IngredientIQ
```

Alternatively, open **Package Manager Console** in Visual Studio and run:
```powershell
Update-Database
```

The database will be automatically seeded with:
- **Admin user:** `admin@ingredientiq.com` / `Admin@123!`
- **50+ sample ingredients** across beneficial, neutral, and harmful categories

#### 4. Configure AI Analysis (Optional)

To enable AI-powered analysis, update `appsettings.json`:

**For OpenAI (GPT-4o-mini):**
```json
{
  "AiAnalysis": {
    "Provider": "openai",
    "ApiKey": "sk-...",
    "Model": "gpt-4o-mini",
    "Endpoint": "https://api.openai.com/v1/chat/completions"
  }
}
```

**For Anthropic (Claude):**
```json
{
  "AiAnalysis": {
    "Provider": "anthropic",
    "ApiKey": "sk-ant-...",
    "Model": "claude-sonnet-4-20250514",
    "Endpoint": "https://api.anthropic.com/v1/messages"
  }
}
```

**Without API Key:**
- The app will operate in **manual mode**
- Users see a structured prompt template they can use with public LLMs
- All analysis features remain available

#### 5. Run Application

```bash
dotnet run
```

Application launches at:
- **HTTP:** `https://localhost:7001`
- **HTTPS:** `https://localhost:7002`

---

## 📋 Default Credentials

| Role | Email | Password | Access |
|------|-------|----------|--------|
| **Admin** | `admin@ingredientiq.com` | `Admin@123!` | Admin Dashboard, Ingredient Management |
| **User** | (Register via UI) | (Your choice) | Scan, History, Profile |
| **Guest** | None | None | 3 free scans, then register prompt |

⚠️ **Change admin password in production immediately!**

---

## 🏗️ Project Architecture

### Single-Project, Folder-Based Clean Architecture

```
IngredientIQ/
├── Domain/                          # Core business entities & validation
│   ├── Entities/                    # User, Ingredient, Scan, etc.
│   ├── Enums/                       # IngredientCategory, RiskTag, ScoreCategory, etc.
│   └── Validation/                  # Validation rules & logic
├── Data/                            # EF Core DbContext & migrations
│   ├── KleenScoreDbContext.cs       # DbContext with entity configurations
│   └── SeedData.cs                  # Pre-load admin user & 50 sample ingredients
├── Services/                        # Business logic & integrations
│   ├── Interfaces/                  # Service contracts
│   ├── AiAnalysisService.cs         # LLM integration (OpenAI/Claude)
│   ├── AuthService.cs               # JWT auth & user management
│   ├── IngredientMatchingService.cs # Fuzzy matching logic
│   ├── ScoringService.cs            # Score calculation (legacy, not used in AI-only mode)
│   ├── OcrService.cs                # OCR stub (manual entry)
│   └── IngredientTextParser.cs      # Ingredient list parsing
├── Pages/                           # Razor Pages UI
│   ├── Account/                     # Auth pages (Register, Login, Profile)
│   ├── Scan/                        # Scan flow (Upload → Review → Result)
│   ├── History/                     # Scan history listing
│   ├── Admin/                       # Admin portal (Dashboard, Ingredients CRUD)
│   └── Shared/                      # Layout, nav, partials
├── wwwroot/                         # Static assets
│   ├── css/                         # Styling (Bootstrap + custom)
│   └── js/                          # Client-side scripts
└── Program.cs                       # Startup & DI configuration
```

### Technology Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| **Framework** | ASP.NET Core / Razor Pages | .NET 10 |
| **Database** | SQL Server / LocalDB | 2019+ |
| **ORM** | Entity Framework Core | 9.0.8 |
| **Authentication** | JWT + Cookie | Built-in |
| **Password Hashing** | BCrypt.Net-Next | 4.0.3 |
| **String Matching** | FuzzySharp | 2.0.2 |
| **AI Integration** | OpenAI / Anthropic | Latest APIs |
| **Styling** | Bootstrap 5 + Custom CSS | - |
| **Frontend JS** | Vanilla JS (no frameworks) | ES6+ |

---

## 📖 User Workflows

### 1. Guest Scan (No Login Required)

```
Homepage
  ↓
[Scan a Product] CTA
  ↓
Upload/Capture Image OR Manual Entry
  ↓
Review Extracted Ingredients
  ↓
Get AI Analysis & Safety Score
  ↓
After 3 scans → Registration prompt
```

### 2. Registered User

```
Register/Login
  ↓
Set Personal Allergens (Profile)
  ↓
Scan Product
  ↓
AI analysis includes personalized allergen checking
  ↓
Save to History
  ↓
Track multiple scans, view trends
```

### 3. Admin Portal

```
Login as Admin
  ↓
Admin Dashboard (stats, overview)
  ↓
Manage Ingredients (CRUD, bulk import)
  ↓
Review user-submitted ingredients
  ↓
Approve/reject, update safety ratings
  ↓
Changes reflected in next scan analysis
```

---

## 🧠 AI Analysis Features

### Regulatory Frameworks Integrated

- **EU FIC Regulation 1169/2011** — European allergen labeling & ingredient ordering
- **US FALCPA + FASTER Act** — US allergen declarations (Big 9)
- **FDA 21 CFR Parts 170-189** — Food additives & GRAS substances
- **JECFA ADI Values** — Acceptable daily intake limits
- **Codex Alimentarius** — International food safety standards
- **FSSAI Standards** — Indian food safety regulations
- **IARC Monographs** — Carcinogenicity classifications
- **EFSA Scientific Opinions** — European Food Safety Authority risk assessments

### Analysis Includes

✅ **Per-Ingredient Safety Rating** (1–10 scale) with regulatory citations
✅ **Allergen Audit** — Declared allergens + hidden allergens (e.g., casein→milk)
✅ **Label Compliance Checking** — Ingredient ordering, QUID issues, additive naming
✅ **Cross-Contamination Risks** — Ingredients processed on shared equipment
✅ **Regulatory Flags** — Banned substances, restricted ingredients, under review
✅ **Population-Specific Warnings** — Infants, pregnancy, medical conditions (PKU, etc.)
✅ **Safer Alternatives** — Replacement ingredients with better safety profiles

### Output Format

AI analysis returns structured JSON with:
- **Summary** — Executive audit summary (max 250 words)
- **Ingredient Analysis** — Name, safety rating, why, risk flags, verdict
- **Overall Score** — 0-100 composite safety assessment
- **Allergen Audit** — Declared/hidden allergens, cross-contamination risks
- **Label Compliance** — Issues with severity (Critical/Major/Minor)
- **Regulatory Flags** — Regulatory body, status, specific CFR/article
- **Interactions** — Documented ingredient interactions
- **Clean Label** — Rating + certification eligibility
- **Population Warnings** — Age/condition-specific alerts
- **Alternatives** — Safer ingredient suggestions

---

## 🔐 Authentication & Security

### JWT Authentication

- **Token Expiry:** 24 hours
- **Refresh:** Automatic on login
- **Storage:** HTTP-only secure cookie
- **Validation:** Signature verification with secret key

### Password Security

- **Hashing:** BCrypt with 12 rounds
- **Minimum Requirements:** 8 characters, uppercase, number, special character
- **Reset:** Email link with 15-minute expiry (future implementation)

### Authorization Policies

| Role | Access |
|------|--------|
| **User** | Profile, History, Scan, Upload |
| **Editor** | All User access + Create/Edit ingredients |
| **Reviewer** | All Editor access + Approve/reject ingredients |
| **SuperAdmin** | All access + User management, analytics |

### Security Headers

- ✅ HTTPS (TLS 1.3)
- ✅ HSTS (HTTP Strict Transport Security)
- ✅ X-Content-Type-Options: nosniff
- ✅ X-Frame-Options: DENY
- ✅ CSP (Content Security Policy)
- ✅ Antiforgery tokens on all forms
- ✅ CORS restricted to known domains
- ✅ Rate limiting (100 requests/minute per user)

---

## 📱 Mobile & Responsive Design

- **Minimum Screen Size:** 320px (iPhone SE)
- **Breakpoints:** 320px, 768px, 1024px, 1440px, 2560px
- **Touch Targets:** ≥48px for accessibility
- **Responsive Images:** Optimized for 4G networks
- **Performance:** Page load <2 seconds on 4G

**Tested Browsers:**
- Chrome 120+
- Firefox 121+
- Edge 120+
- Safari 17+

---

## 🧪 Testing

### Run All Tests

```bash
dotnet test
```

### Test Coverage

- **Unit Tests:** Service logic, scoring calculations, matching algorithms
- **Integration Tests:** End-to-end scan pipeline, auth flows, database operations
- **Target Coverage:** ≥80% of business logic

### Manual QA Checklist

- [ ] Guest can scan 3 products without login
- [ ] 4th scan shows registration prompt
- [ ] Registered user can set allergens
- [ ] Scan with user allergens shows reduced score
- [ ] Admin can create/edit/delete ingredients
- [ ] Ingredient changes reflected in next scan
- [ ] AI analysis returned when API key configured
- [ ] Manual prompt displayed when API key missing
- [ ] Mobile layout responsive at 320px, 768px, 1024px
- [ ] Error messages clear and actionable

---

## 📊 Database Schema

### Core Entities

| Entity | Purpose |
|--------|---------|
| **User** | Registered users with auth credentials & allergens |
| **Scan** | Product scan records with extracted ingredients & AI analysis |
| **Ingredient** | Curated ingredient master data (name, safety rating, risk tags) |
| **ScanIngredientDetail** | Join table linking scans to ingredients with position & confidence |
| **AuditLog** | Complete audit trail of data changes (who, what, when) |

### Key Relationships

```
User (1) ──────── (Many) Scan
                     ├─── ExtractedText
                     ├─── AiAnalysisResult (JSON)
                     └─── (Many) ScanIngredientDetail
                            └─── (1) Ingredient
```

### Sample Queries

```sql
-- Find scans with allergen flags
SELECT * FROM Scans 
WHERE AiAnalysisResult LIKE '%hiddenAllergens%'
ORDER BY CreatedAt DESC;

-- Top 10 most scanned ingredients
SELECT TOP 10 i.Name, COUNT(sid.ScanId) as ScanCount
FROM Ingredients i
JOIN ScanIngredientDetails sid ON i.Id = sid.IngredientId
GROUP BY i.Name
ORDER BY ScanCount DESC;

-- User scan history with scores
SELECT s.CreatedAt, s.Id, JSON_VALUE(s.AiAnalysisResult, '$.overallScore') as Score
FROM Scans s
WHERE s.UserId = @UserId
ORDER BY s.CreatedAt DESC;
```

---

## 🔧 Configuration & Environment Variables

### appsettings.json Structure

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=IngredientIQ;..."
  },
  "Jwt": {
    "Key": "your-secret-key-min-32-chars",
    "Issuer": "IngredientIQ",
    "Audience": "IngredientIQ",
    "ExpiryHours": 24
  },
  "AiAnalysis": {
    "Provider": "openai",
    "ApiKey": "",
    "Model": "gpt-4o-mini",
    "Endpoint": ""
  },
  "IngredientIQ": {
    "DefaultCategoryFactor": 1.0,
    "SkincareFactor": 1.2,
    "SupplementFactor": 1.1
  }
}
```

### Environment Variables (Production)

```bash
# Database
SQLSERVER_CONNECTION_STRING="Server=prod-sql.example.com;Database=IngredientIQ;..."

# JWT
JWT_KEY="your-long-secure-key-here"
JWT_ISSUER="IngredientIQ"

# AI API
AI_PROVIDER="openai"
AI_API_KEY="sk-..."
AI_MODEL="gpt-4o-mini"
```

---

## 🚀 Deployment

### Docker Support

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 80 443

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src
COPY ["IngredientIQ/IngredientIQ.csproj", "IngredientIQ/"]
RUN dotnet restore "IngredientIQ/IngredientIQ.csproj"
COPY . .
RUN dotnet build "IngredientIQ/IngredientIQ.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IngredientIQ/IngredientIQ.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IngredientIQ.dll"]
```

### Cloud Deployment Options

**Azure App Service:**
```bash
# Create resource group
az group create -n IngredientIQ-rg -l eastus

# Create App Service
az appservice plan create -g IngredientIQ-rg -n IngredientIQ-plan --sku B2

# Deploy
dotnet publish -c Release
cd bin/Release/net10.0/publish
zip -r app.zip .
az webapp deployment source config-zip -g IngredientIQ-rg -n IngredientIQ --src app.zip
```

**AWS Elastic Beanstalk:**
```bash
eb init -p "ASP.NET Core on Linux" IngredientIQ
eb create ingredientiq-env
eb deploy
```

---

## 📝 API Endpoints (Key Routes)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| **POST** | `/Account/Register` | None | Register new user |
| **POST** | `/Account/Login` | None | Login & get JWT cookie |
| **POST** | `/Account/Logout` | User | Clear auth cookie |
| **POST** | `/Scan/Upload` | None | Upload product image or manual entry |
| **POST** | `/Scan/Review` | None | Process extracted ingredients |
| **POST** | `/Scan/Result` | None | Calculate AI analysis & display score |
| **GET** | `/History/Index` | User | View user's scan history |
| **GET** | `/Admin/Dashboard` | Admin | Analytics & overview |
| **GET** | `/Admin/Ingredients/Index` | Admin | List all ingredients |
| **POST** | `/Admin/Ingredients/Create` | Admin | Add new ingredient |
| **POST** | `/Admin/Ingredients/Edit/{id}` | Admin | Update ingredient |
| **POST** | `/Admin/ReviewQueue` | Admin | Approve/reject pending ingredients |

---

## 🐛 Troubleshooting

### Database Connection Failed

```
Error: Cannot connect to (localdb)\MSSQLLocalDB
```

**Solution:**
1. Check SQL Server LocalDB is installed: `sqllocaldb info`
2. Start LocalDB: `sqllocaldb start mssqllocaldb`
3. Verify connection string in `appsettings.json`

### Entity Framework Migrations Error

```
Error: Package 'Microsoft.EntityFrameworkCore.Tools' is not installed
```

**Solution:**
```bash
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

### AI Analysis Not Working

```
Error: AI API key not configured
```

**Solution:**
- Add API key to `appsettings.json` (OpenAI or Anthropic)
- Or use manual prompt mode (no API key needed)
- Manual mode displays structured prompt for public LLMs (ChatGPT, Claude, Copilot)

### Authentication Issues

```
Error: 401 Unauthorized
```

**Solution:**
1. Verify JWT key is ≥32 characters
2. Check cookie is set: DevTools → Application → Cookies
3. Clear browser cache & cookies
4. Try incognito/private window

### Port Already in Use

```
Error: Address already in use: 5001
```

**Solution:**
```bash
# Find process using port
netstat -ano | findstr :5001

# Kill process (Windows)
taskkill /PID <PID> /F

# Or use different port
dotnet run -- --urls=https://localhost:7003
```

---

## 📚 Documentation

- **[IMPLEMENTATION_PLAN.md](./Architecture/IMPLEMENTATION_PLAN.md)** — Phase-by-phase development breakdown
- **[API Documentation](#)** — Swagger docs available at `/swagger` when running locally

---

## 🤝 Contributing

We welcome contributions! Please follow these steps:

1. **Fork** the repository
2. **Create** a feature branch: `git checkout -b feature/your-feature`
3. **Commit** changes: `git commit -m 'Add feature'`
4. **Push** to branch: `git push origin feature/your-feature`
5. **Open** a Pull Request with clear description

### Code Standards

- Follow C# style guidelines ([EditorConfig](https://editorconfig.org/))
- Write unit tests for new logic
- Update documentation for breaking changes
- Keep commit messages descriptive & concise

---

## 📄 License

This project is licensed under the **MIT License** — see [LICENSE](LICENSE) file for details.

---

## 🆘 Support

- **Issues:** [GitHub Issues](https://github.com/yourusername/IngredientIQ/issues)
- **Discussions:** [GitHub Discussions](https://github.com/yourusername/IngredientIQ/discussions)
- **Email:** support@ingredientiq.com (future)

---

## 🙏 Acknowledgments

- **AI Analysis Framework:** Inspired by regulatory standards from FDA, EU, JECFA, FSSAI, IARC, EFSA
- **Open Source Libraries:** Entity Framework Core, Bootstrap, FuzzySharp, BCrypt.Net-Next
- **Community:** Thanks to all contributors and users providing feedback

---

## 🎯 Roadmap

### Phase 2 
- [ ] Barcode scanning integration
- [ ] Product comparison tool
- [ ] Advanced user dashboard with trends
- [ ] Email notifications for saved products
- [ ] API for third-party integrations

### Phase 3 
- [ ] Native mobile apps (iOS & Android)
- [ ] Multi-language support
- [ ] ML-powered ingredient recommendations
- [ ] Brand partnerships & affiliate links
- [ ] Blockchain verification (aspirational)

### Phase 4+ 
- [ ] Advanced analytics & reporting
- [ ] Community ingredient reviews
- [ ] Integration with health platforms (Apple Health, Google Fit)
- [ ] Enterprise solutions for retailers & manufacturers

---

**Built with ❤️ by the IngredientIQ Team**

**Last Updated:** 2026 | **Version:** 1.0.0-MVP

