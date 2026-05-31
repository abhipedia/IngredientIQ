# IngredientIQ — Quick Reference Guide

## 🚀 Quick Start (2 minutes)

```bash
# 1. Clone repo
git clone https://github.com/yourusername/IngredientIQ.git
cd IngredientIQ

# 2. Restore & Build
dotnet restore
dotnet build

# 3. Update Database
dotnet ef database update

# 4. Run
dotnet run

# 5. Open browser
https://localhost:7001
```

**Default Admin Account:** `admin@ingredientiq.com` / `Admin@123!`

---

## 📋 Project Structure at a Glance

```
IngredientIQ/
├── Domain/                  → Entities, enums, validation
├── Data/                    → DbContext, migrations
├── Services/                → Business logic, AI integration
├── Pages/                   → Razor Pages UI
├── wwwroot/                 → CSS, JS, images
├── Program.cs               → Startup configuration
├── appsettings.json         → Configuration
└── README.md                → Full documentation
```

---

## 🔑 Key Features

| Feature | Status | Notes |
|---------|--------|-------|
| User Auth | ✅ | JWT + BCrypt |
| AI Analysis | ✅ | OpenAI/Anthropic or manual mode |
| Allergen Tracking | ✅ | Personalized risk assessment |
| Scan History | ✅ | Paginated, searchable |
| Admin Portal | ✅ | Ingredient CRUD, analytics |
| Mobile Responsive | ✅ | 320px+ screens |
| Security | ✅ | HTTPS, antiforgery, rate limiting |

---

## 🧪 Common Tasks

### Add a New Feature

```csharp
// 1. Add model/DTO in Services/Models/
public class MyFeatureDto { }

// 2. Add service interface in Services/Interfaces/
public interface IMyFeatureService { }

// 3. Implement service in Services/
public class MyFeatureService : IMyFeatureService { }

// 4. Register in Program.cs
builder.Services.AddScoped<IMyFeatureService, MyFeatureService>();

// 5. Use in Razor Page
public class MyPageModel : PageModel {
    private readonly IMyFeatureService _service;
    public MyPageModel(IMyFeatureService service) => _service = service;
}
```

### Configure AI Analysis

**For OpenAI (GPT):**
```json
{
  "AiAnalysis": {
    "Provider": "openai",
    "ApiKey": "sk-...",
    "Model": "gpt-4o-mini"
  }
}
```

**For Anthropic (Claude):**
```json
{
  "AiAnalysis": {
    "Provider": "anthropic",
    "ApiKey": "sk-ant-...",
    "Model": "claude-sonnet-4-20250514"
  }
}
```

### Add Database Migration

```bash
# Create migration
dotnet ef migrations add MyMigration

# Apply migration
dotnet ef database update

# Revert last migration
dotnet ef database update PreviousMigrationName
```

### Run Tests

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter ClassName

# With coverage
dotnet test /p:CollectCoverage=true
```

### Debug in Visual Studio

1. **Set Breakpoint:** Click left margin in code
2. **Run:** Press `F5` or Debug → Start Debugging
3. **Step Through:** Use F10 (step over) or F11 (step into)
4. **View Variables:** Hover over variables or use Debug → Windows → Locals

---

## 🔐 Authentication Quick Reference

### User Roles

| Role | Capabilities |
|------|--------------|
| **User** | Scan, History, Profile |
| **Editor** | User + Create/Edit ingredients |
| **Reviewer** | Editor + Approve/reject ingredients |
| **SuperAdmin** | All access |

### Authorization in Razor Pages

```csharp
[Authorize]  // Any authenticated user
public class MyPage : PageModel { }

[Authorize(Roles = "SuperAdmin")]  // Specific role
public class AdminPage : PageModel { }
```

### Get Current User

```csharp
var user = await _authService.GetCurrentUserAsync(User);
if (user != null) {
    // User is authenticated
}
```

---

## 🎨 UI Components & Styling

### Bootstrap Classes Used

```html
<!-- Container & Layout -->
<div class="container">...</div>
<div class="row g-4">...</div>
<div class="col-md-6">...</div>

<!-- Buttons -->
<button class="btn btn-primary">Primary</button>
<button class="btn btn-success">Success</button>
<button class="btn btn-danger">Danger</button>

<!-- Colors -->
<span class="text-success">Green (Safe)</span>
<span class="text-warning">Yellow (Caution)</span>
<span class="text-danger">Red (Risk)</span>

<!-- Badges -->
<span class="badge bg-success">Beneficial</span>
<span class="badge bg-danger">Harmful</span>
```

### Custom Classes (site.css)

```css
/* Score Gauge */
.score-gauge { /* Large circular score display */ }

/* Feature Cards */
.feature-card { /* Card-based feature showcase */ }

/* Progress Indicator */
.scan-step { /* 3-step progress indicator */ }

/* Risk Badge */
.badge-risk { /* Color-coded risk tags */ }
```

---

## 🧠 AI Analysis Workflow

```
User Input (Ingredients)
    ↓
IngredientTextParser.ParseIngredients()
    ↓
IngredientMatchingService.MatchAllAsync()
    ↓
AiAnalysisService.AnalyzeAsync()
    ↓
Call LLM (OpenAI/Anthropic) with:
- Ingredients list
- User allergens
- Product category
- Regulatory framework prompt
    ↓
Parse JSON Response
    ↓
Display Results
- Overall score
- Ingredient breakdown
- Allergen audit
- Label compliance
- Regulatory flags
```

---

## 📊 Database Queries Reference

### Find Ingredient
```csharp
var ingredient = await _db.Ingredients
    .FirstOrDefaultAsync(i => i.Name == "Vitamin E");
```

### Get User Scans
```csharp
var scans = await _db.Scans
    .Where(s => s.UserId == userId)
    .OrderByDescending(s => s.CreatedAt)
    .Take(20)
    .ToListAsync();
```

### Find Unrecognized Ingredients
```csharp
var unrecognized = await _db.ScanIngredientDetails
    .Where(d => d.IngredientId == null)
    .GroupBy(d => d.RawName)
    .Select(g => new { Name = g.Key, Count = g.Count() })
    .OrderByDescending(x => x.Count)
    .ToListAsync();
```

---

## 🐛 Troubleshooting

| Issue | Solution |
|-------|----------|
| **Cannot connect to DB** | Check connection string, start SQL LocalDB: `sqllocaldb start mssqllocaldb` |
| **Migration failed** | Delete migrations, run `dotnet ef database drop --force`, recreate migration |
| **Port 5001 in use** | Kill process: `netstat -ano \| findstr :5001`, `taskkill /PID <PID> /F` |
| **AI API error** | Check API key in appsettings.json, verify provider config |
| **Authentication error** | Clear cookies, try incognito window, check JWT secret key length (≥32 chars) |
| **Images not uploading** | Check file size (<10MB), format (JPG/PNG/HEIC/WEBP), folder permissions |

---

## 📱 Responsive Design Breakpoints

```css
/* Mobile (320px - 767px) */
@media (max-width: 767px) { }

/* Tablet (768px - 1023px) */
@media (min-width: 768px) { }

/* Desktop (1024px+) */
@media (min-width: 1024px) { }
```

---

## 🔒 Security Checklist

- ✅ Use HTTPS only (enforced in Program.cs)
- ✅ Never commit secrets (use appsettings.json or environment variables)
- ✅ Hash passwords with BCrypt (built-in)
- ✅ Validate user input (server-side)
- ✅ Use parameterized queries (EF Core default)
- ✅ Add antiforgery tokens (Razor Pages default)
- ✅ Set secure session cookies
- ✅ Implement rate limiting (100 req/min per user)

---

## 📚 Useful Links

- [Microsoft Docs - ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Bootstrap 5 Documentation](https://getbootstrap.com)
- [C# Language Reference](https://docs.microsoft.com/dotnet/csharp)
- [OpenAI API Docs](https://platform.openai.com/docs)
- [Anthropic Claude Docs](https://docs.anthropic.com)

---

## 💡 Pro Tips

1. **Enable Hot Reload:** `dotnet watch run` for auto-reload on file changes
2. **Debug SQL Queries:** Add `.LogTo(Console.WriteLine)` to DbContext to see generated SQL
3. **Format Code:** `dotnet format` to auto-format codebase
4. **View Entity Changes:** Use `_db.ChangeTracker.Entries()` to debug state changes
5. **Async All The Way:** Never use `.Result` or `.Wait()` — can cause deadlocks
6. **Use Constants:** Define magic strings as constants in separate file
7. **Validate Early:** Validate input at page boundary, not in service

---

## 📞 Getting Help

- **Issues:** [GitHub Issues](https://github.com/yourusername/IngredientIQ/issues)
- **Documentation:** See README.md and IMPLEMENTATION_PLAN.md
- **Code Comments:** Hover over types/methods to see XML docs
- **Logs:** Check `ILogger` output in Debug console

---

**Last Updated:** 2026 | **Version:** 1.0.0-MVP

