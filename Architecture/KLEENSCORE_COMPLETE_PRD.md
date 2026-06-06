# IngredientIQ - Complete Project Requirements & .NET Implementation Guide



---

## Table of Contents

1. [Executive Summary](#executive-summary)
2. [Business Objectives](#business-objectives)
3. [Project Scope](#project-scope)
4. [Stakeholders](#stakeholders)
5. [User Personas](#user-personas)
6. [Functional Requirements](#functional-requirements)
7. [Non-Functional Requirements](#non-functional-requirements)
8. [System Architecture](#system-architecture)
9. [Data Model](#data-model)
10. [User Journey](#user-journey)
11. [Assumptions & Constraints](#assumptions--constraints)
12. [Risk Management](#risk-management)
13. [Success Metrics](#success-metrics)
14. [Release Plan](#release-plan)
15. [Acceptance Criteria](#acceptance-criteria)
16. [.NET Solution Structure](#net-solution-structure)
17. [Key Implementation Files](#key-implementation-files)

---

## Executive Summary

**IngredientIQ** is a web-based platform that empowers consumers to make informed purchasing decisions by analyzing the ingredient list of any product. Users upload a photograph of a product's ingredient label, and the system uses Optical Character Recognition (OCR) and image processing to extract ingredients, match them against a curated database of "good" and "bad" ingredients, and return a **Kleen Score** (0–100) along with detailed insights.

**Product Vision:** Transform ingredient transparency into accessible, actionable insights for every consumer.

**Target Users:**
- Health-conscious shoppers
- Parents evaluating product safety
- Allergy sufferers
- Fitness enthusiasts

**Key Differentiator:** AI-powered ingredient analysis with transparent scoring methodology and personalized risk assessment.

---

## Business Objectives

| # | Objective | Success Metric | Target Timeline |
|---|-----------|----------------|-----------------|
| BO-1 | Enable consumers to instantly evaluate product safety before purchase | 80% of users rate safety insights as "valuable" | Month 3 |
| BO-2 | Build a trusted, science-backed ingredient knowledge base | 500+ ingredients in database by launch | Month 3 |
| BO-3 | Drive user engagement through scan history & recommendations | 2.5+ scans per user/month | Month 6 |
| BO-4 | Monetize via freemium, affiliate links & brand partnerships | 15% conversion to paid by Month 6 | Month 6 |
| BO-5 | Achieve >85% OCR accuracy on standard product labels | <8 second average response time | Month 3 |

---

## Project Scope

### 3.1 In Scope (MVP - Phase 1)

✅ **User Management**
- Email-based registration & authentication
- Secure password management
- User profiles with allergen tracking
- Guest mode (3 free scans)

✅ **Image Upload & Processing**
- Multi-format image upload (JPG, PNG, HEIC, WEBP)
- Direct camera capture capability
- Image quality validation
- Multi-image support (front & back label)
- Real-time preview functionality

✅ **OCR & Text Extraction**
- Azure Computer Vision API integration
- Automatic ingredient section detection
- Text normalization & synonym mapping
- Manual ingredient review & editing
- Confidence scoring display

✅ **Ingredient Database**
- 500+ pre-loaded ingredients
- Safety ratings (1-10 scale)
- Risk tagging system
- Source references & citations
- Admin CRUD operations
- Bulk import capability

✅ **Scoring Engine**
- Weighted formula calculation
- Position-based concentration weighting
- User allergen personalization
- Score categorization (Green/Yellow/Red)
- Transparency in methodology

✅ **Results & Reporting**
- Visual score display with gauges
- Ingredient breakdown tables
- Risk highlights
- Save to history
- PDF export
- Shareable links

✅ **Admin Portal**
- Role-based access control
- Ingredient management
- Approval workflows
- Analytics dashboard
- Audit logging

### 3.2 Out of Scope (Phase 2+)

❌ Native mobile apps (iOS/Android)
❌ Barcode scanning
❌ E-commerce checkout integration
❌ Multi-language support (beyond English)
❌ Machine learning recommendations (Phase 3)
❌ Blockchain verification (aspirational)
❌ API marketplace for third-party integrations

---

## Stakeholders

| Stakeholder | Role | Key Responsibilities | Involvement |
|-------------|------|----------------------|--------------|
| **End Users (Consumers)** | Primary Users | Upload images, view scores, manage history | High |
| **Product Owner** | Vision & Strategy | Define roadmap, prioritize features, track KPIs | High |
| **Nutritionists / Dermatologists** | Subject Matter Experts | Curate ingredient database, validate safety ratings, review controversial items | High |
| **Admin / Content Team** | Operations | Maintain ingredient lists, approve new entries, handle user-submitted ingredients | Medium |
| **Developers / QA** | Execution | Build, test, deploy, maintain system | High |
| **Marketing & Partnerships** | Growth | Drive user acquisition, establish brand partnerships, manage affiliate relationships | Medium |
| **Regulatory Advisor** | Compliance | Ensure legal compliance, manage disclaimers, handle liability | Medium |
| **DevOps / Infrastructure** | Operations | Manage cloud infrastructure, monitoring, backups, security | Medium |

---

## User Personas

### Persona 1: Health-conscious Shopper
**Name:** Priya, 32  
**Background:** Tech professional, fitness enthusiast  
**Goal:** Quickly verify safety of skincare & wellness products  
**Pain Point:** Doesn't understand complex chemical ingredient names  
**Usage Pattern:** 2-3 scans per week, primarily skincare & supplements  
**Motivation:** Wants transparent, science-backed product information  

### Persona 2: Responsible Parent
**Name:** Rahul, 38  
**Background:** Father of two young children  
**Goal:** Ensure baby products (food, lotion, wipes) are safe and non-toxic  
**Pain Point:** Concerned about harmful chemicals in children's products  
**Usage Pattern:** 4-5 scans per week, focused on baby products  
**Motivation:** Protect children from dangerous chemicals  

### Persona 3: Allergy Sufferer
**Name:** Sam, 24  
**Background:** Severe peanut & dairy allergies  
**Goal:** Quickly detect allergens in products before purchase  
**Pain Point:** Risk of accidental exposure due to unclear or hidden allergens  
**Usage Pattern:** 3-4 scans per shopping trip, before purchase  
**Motivation:** Prevent life-threatening allergic reactions  

### Persona 4: Fitness Enthusiast
**Name:** Anita, 28  
**Background:** Competitive runner, very health-conscious  
**Goal:** Avoid additives, sugars, and synthetic ingredients  
**Pain Point:** Misleading "natural" and "healthy" marketing claims  
**Usage Pattern:** Daily scanning of supplements & foods  
**Motivation:** Maintain optimal health and athletic performance  

---

## Functional Requirements

### FR-1: User Management (FR-UM)

| ID | Requirement | Description | Priority |
|----|-------------|-------------|----------|
| FR-UM-01 | User Registration | Users can register via email with strong password requirements (min 8 chars, uppercase, number, special char). Email verification link sent to confirm account. | **MUST** |
| FR-UM-02 | User Authentication | JWT-based token authentication with 24-hour expiry and refresh token mechanism | **MUST** |
| FR-UM-03 | Password Management | Secure password reset via email link with 15-minute expiry. Passwords hashed using bcrypt with salt. | **MUST** |
| FR-UM-04 | User Profiles | Users can save allergens, dietary preferences, skin type, and health conditions | **MUST** |
| FR-UM-05 | Guest Mode | Anonymous users can perform up to 3 free scans before sign-up prompt | **MUST** |
| FR-UM-06 | Account Deletion | GDPR-compliant full data removal within 30 days of deletion request | **MUST** |
| FR-UM-07 | Social Login (Phase 2) | Google & Apple SSO integration (future enhancement) | **SHOULD** |
| FR-UM-08 | Two-Factor Auth (Phase 2) | Optional 2FA via authenticator app or SMS (future) | **COULD** |

**Business Rules:**
- Email must be unique and valid
- Username 3-20 alphanumeric characters, case-insensitive
- Users must accept Terms of Service & Privacy Policy
- Email confirmation required within 24 hours

---

### FR-2: Image Upload & Capture (FR-IM)

| ID | Requirement | Description | Priority |
|----|-------------|-------------|----------|
| FR-IM-01 | File Upload | Support JPG, PNG, HEIC, WEBP formats with max 10 MB size limit | **MUST** |
| FR-IM-02 | Camera Capture | Direct photo capture from device camera with preview | **MUST** |
| FR-IM-03 | Image Validation | Reject blurry/low-res images (min 300x300px). Test focus clarity using Laplacian variance. | **MUST** |
| FR-IM-04 | Multi-image Upload | Support uploading front & back label images in single session | **MUST** |
| FR-IM-05 | Image Storage | Secure storage in AWS S3 with AES-256 encryption at rest | **MUST** |
| FR-IM-06 | Image Metadata | Capture upload time, device type, camera quality metrics | **SHOULD** |
| FR-IM-07 | On-screen Guidance | Visual tips for alignment, lighting, distance, focus | **MUST** |
| FR-IM-08 | Image Auto-deletion | Automatically purge images after 30 days unless explicitly saved | **MUST** |
| FR-IM-09 | Compression | Optimize large images for faster processing without quality loss | **SHOULD** |

**Acceptance Criteria:**
- Upload completes in <5 seconds for 5MB file on 4G
- Image preview shown before processing
- Clear, user-friendly error messages for rejected files
- Mobile responsiveness on all devices

---

### FR-3: Image Processing & OCR (FR-OCR)

| ID | Requirement | Description | Priority |
|----|-------------|-------------|----------|
| FR-OCR-01 | Image Pre-processing | Auto deskew (rotate if needed), denoise (reduce noise), enhance contrast | **MUST** |
| FR-OCR-02 | Glare Removal | Remove reflections & glare from glossy labels using advanced algorithms | **MUST** |
| FR-OCR-03 | OCR Extraction | Use Azure Computer Vision API for text extraction with >80% accuracy target | **MUST** |
| FR-OCR-04 | Section Detection | Automatically locate and isolate "INGREDIENTS:" section from product label | **MUST** |
| FR-OCR-05 | Text Tokenization | Parse ingredients separated by commas, semicolons, parentheses, hyphens, bullets | **MUST** |
| FR-OCR-06 | Normalization | Convert to lowercase, strip punctuation, expand abbreviations (e.g., "Vit E" → "Vitamin E") | **MUST** |
| FR-OCR-07 | Synonym Mapping | Map known synonyms: "tocopherol" → "Vitamin E", "salt" → "sodium chloride", "ascorbic acid" → "Vitamin C" | **MUST** |
| FR-OCR-08 | Manual Review UI | Users can view extracted text and manually correct misread items before scoring | **MUST** |
| FR-OCR-09 | Confidence Scoring | Display OCR confidence % for transparency (e.g., "94% confidence in extraction") | **MUST** |
| FR-OCR-10 | Fallback Handling | If OCR confidence <75%, flag for manual review or allow user to manually enter ingredients | **MUST** |

**Business Rules:**
- Minimum OCR confidence: 75%
- Flag low-confidence extractions for admin review
- Archive original image for audit trail
- Maximum processing time: 8 seconds from upload to results

---

### FR-4: Ingredient Database (FR-DB)

| ID | Requirement | Description | Priority |
|----|-------------|-------------|----------|
| FR-DB-01 | Master Database | 500+ ingredients with comprehensive attributes (name, synonyms, CAS#, category, rating, tags, references) | **MUST** |
| FR-DB-02 | Safety Ratings | Scale 1-10 where: 1-3 (Harmful), 4-7 (Neutral/Acceptable), 8-10 (Beneficial). Must be science-backed. | **MUST** |
| FR-DB-03 | Risk Tags | Multiple tags per ingredient: carcinogen, allergen, endocrine disruptor, GMO, artificial, natural, vegan, etc. | **MUST** |
| FR-DB-04 | Source References | Links to scientific studies, FDA reports, certifications, Wikipedia, PubMed | **MUST** |
| FR-DB-05 | Categories | Food additives, preservatives, flavorings, colorings, skincare actives, emulsifiers, vitamins, etc. | **MUST** |
| FR-DB-06 | Admin CRUD | Full Create, Read, Update, Delete operations via Admin Portal | **MUST** |
| FR-DB-07 | Bulk Import | CSV import for batch ingredient additions with validation | **MUST** |
| FR-DB-08 | Versioning | Track all changes with version history and rollback capability | **SHOULD** |
| FR-DB-09 | Caching | Redis cache for frequently accessed ingredients with 1-hour TTL | **MUST** |
| FR-DB-10 | Approval Workflow | New ingredients require expert (nutritionist/dermatologist) approval before use in scoring | **MUST** |
| FR-DB-11 | Deprecation | Mark ingredients as deprecated (kept for audit trail) rather than deleting | **SHOULD** |
| FR-DB-12 | Search & Filter | Full-text search, filter by category/rating/risk tags, sort by safety rating | **SHOULD** |

**Data Structure:**
```
Ingredient:
- Id (GUID)
- Name (string, indexed, unique)
- Synonyms (List<string>, searchable)
- CAS_Number (string, indexed)
- Category (enum: Additive, Preservative, Vitamin, Skincare, etc.)
- Safety_Rating (1-10, int)
- Risk_Tags (List<enum>)
- Source_References (List<URL>)
- Created_By (GUID, FK to User)
- Approved_By (GUID?, FK to User)
- Status (enum: Active, Review, Deprecated)
- Created_At (DateTime, indexed)
- Last_Updated (DateTime)
```

---

### FR-5: Kleen Score Engine (FR-SC)

| ID | Requirement | Description | Priority |
|----|-------------|-------------|----------|
| FR-SC-01 | Ingredient Matching | Exact match + fuzzy match (Levenshtein distance ≥90% confidence) for robustness | **MUST** |
| FR-SC-02 | Score Calculation | Weighted formula: `(∑[Rating × Position_Weight] ÷ Count) × 10`, capped at 0-100 | **MUST** |
| FR-SC-03 | Concentration Factor | Ingredients listed first = higher concentration. Weight decreases: 1st: 100%, 5th: 60%, 10th: 40% | **MUST** |
| FR-SC-04 | Personalization | Adjust score based on user's known allergens (-20 points per allergen found) | **MUST** |
| FR-SC-05 | Category Weighting | Different weights for food vs. skincare: Food = 1.0x, Skincare = 1.2x (stricter), Supplements = 1.1x | **SHOULD** |
| FR-SC-06 | Score Categorization | Green (80-100, Excellent), Yellow (50-79, Moderate), Red (0-49, Poor) | **MUST** |
| FR-SC-07 | Unrecognized Handling | Queue unknown ingredients for admin review. Treat as neutral (-0 points) initially. | **MUST** |
| FR-SC-08 | Score Caching | Cache scores for identical ingredient lists to improve performance | **SHOULD** |
| FR-SC-09 | Audit Trail | Log scoring logic and final calculation for transparency & debugging | **SHOULD** |
| FR-SC-10 | Real-time Updates | Scoring reflects latest ingredient database version (invalidate cache if ingredient rating changes) | **MUST** |

**Scoring Formula (Detailed):**
```
Total Score = Σ[(Safety_Rating × Position_Weight × Category_Factor) × User_Risk_Multiplier] ÷ Total_Ingredients × 10

Where:
- Safety_Rating: 1-10 (from ingredient database)
- Position_Weight: 1st ingredient = 1.0, 5th = 0.6, 10th = 0.4, etc.
- Category_Factor: Food additive = 1.0, Skincare = 1.2 (more stringent)
- User_Risk_Multiplier: 1.0 (normal) or 0.5 (user allergic to this ingredient)
- Final capped at 0-100 range

Example Calculation:
Ingredients: Vitamin E (rating 9, pos 1), Water (rating 10, pos 2), Benzene (rating 1, pos 3)

Score = [(9 × 1.0 × 1.0 × 1.0) + (10 × 0.8 × 1.0 × 1.0) + (1 × 0.6 × 1.0 × 1.0)] ÷ 3 × 10
      = [9 + 8 + 0.6] ÷ 3 × 10
      = 17.6 ÷ 3 × 10
      = 58.7 → 59 (Yellow, Moderate)
```

---

### FR-6: Results & Reporting (FR-RR)

| ID | Requirement | Description | Priority |
|----|-------------|-------------|----------|
| FR-RR-01 | Score Display | Visual gauge (0-100) with large, color-coded circle. Emoji indicator: 😟 (Red), 😐 (Yellow), 😊 (Green) | **MUST** |
| FR-RR-02 | Ingredient Breakdown | Table showing: Name, Rating, Risk Tags, Match Confidence. Icons: ✅ (good), ❌ (bad), ❓ (unknown) | **MUST** |
| FR-RR-03 | Explanation | Collapsible "Why this score?" section explaining calculation logic and key findings | **MUST** |
| FR-RR-04 | Risk Highlights | Bold/highlight top 3 concerning ingredients with explanation of risks | **MUST** |
| FR-RR-05 | Save Results | Users can bookmark scans to personal history for future reference | **MUST** |
| FR-RR-06 | Share Results | Generate time-limited shareable link (valid 24 hours) to share with friends/family | **MUST** |
| FR-RR-07 | Social Sharing | One-click share to Twitter/Facebook/WhatsApp with score summary | **SHOULD** |
| FR-RR-08 | PDF Export | Download full report as PDF with branding, score, ingredients, and recommendations | **MUST** |
| FR-RR-09 | Print View | Print-optimized layout without ads or unnecessary UI elements | **SHOULD** |
| FR-RR-10 | Scan History | Paginated list of user's past scans (50 per page) with filters by date/score/category | **MUST** |

**Report Contents:**
- Kleen Score (0-100) with visual gauge
- Product label image (if saved)
- Extracted ingredients list
- Ingredient-by-ingredient breakdown with ratings
- Risk assessment summary
- Top 3 concerning ingredients with details
- Scan date & OCR confidence
- Disclaimer: "This is not medical advice"
- User allergen impact (if applicable)

---

### FR-7: Admin Portal (FR-AD)

| ID | Requirement | Description | Priority |
|----|-------------|-------------|----------|
| FR-AD-01 | Admin Authentication | Secure login with role-based access control (Super Admin, Editor, Reviewer) | **MUST** |
| FR-AD-02 | Ingredient Management | Full CRUD (Create, Read, Update, Delete) for ingredient database via clean UI | **MUST** |
| FR-AD-03 | Bulk Import | Upload CSV file to import multiple ingredients with validation and error reporting | **MUST** |
| FR-AD-04 | Review Queue | Queue of unrecognized ingredients from user scans pending expert approval | **MUST** |
| FR-AD-05 | Approval Workflow | Multi-level approval: Editor proposes → Reviewer approves → Goes live | **SHOULD** |
| FR-AD-06 | Analytics Dashboard | Real-time stats: scans/day, top ingredients, OCR accuracy, user growth, engagement | **MUST** |
| FR-AD-07 | User Management | View accounts, ban users, view scan history, manage permissions | **SHOULD** |
| FR-AD-08 | Audit Logs | Complete activity history (100% retention): who changed what, when, with old/new values | **MUST** |
| FR-AD-09 | Data Export | Export scan results, ingredient ratings, audit logs to CSV/Excel for analysis | **SHOULD** |
| FR-AD-10 | System Health | Monitor API performance, OCR latency, error rates, database health | **SHOULD** |
| FR-AD-11 | Report Generation | Generate monthly reports: user growth, popular ingredients, feedback summary | **COULD** |
| FR-AD-12 | Content Moderation | Review user-submitted ingredients, flag spam, manage disputes | **SHOULD** |

---

## Non-Functional Requirements

### NFR-1: Performance

| Requirement | Target | Metric |
|-------------|--------|--------|
| API Response Time (p50) | 500ms | End-to-end from request to response |
| API Response Time (p95) | 4 seconds | 95% of requests must complete within 4 seconds |
| API Response Time (p99) | 8 seconds | 99% of requests must complete within 8 seconds |
| OCR Extraction Time | 3-5 seconds | Average time to extract text from image |
| Score Calculation Time | <100ms | Database lookup + scoring logic |
| Image Upload Time | <5 seconds | For 5MB file on 4G network |
| Page Load Time | <2 seconds | Initial page load on 4G |
| Database Query Time | <100ms | Ingredient lookup queries (with cache hit) |
| Cache Hit Ratio | >80% | Ingredient cache efficiency |

**Performance Testing Strategy:**
- Load test with k6 or Apache JMeter
- Test with 10,000 concurrent users
- Spike testing for sudden traffic spikes
- Endurance testing for 24-hour runs

---

### NFR-2: Scalability

| Requirement | Target | Details |
|-------------|--------|---------|
| Initial Concurrent Users | 10,000 | Peak concurrent users at launch |
| Horizontal Scaling | 100,000+ | Capability to scale to 100k users without code changes |
| Database Scaling | Partitioning ready | Architecture supports future sharding by user_id |
| API Stateless | 100% | All API services are stateless (enables horizontal scaling) |
| Load Balancing | Round-robin | Distribute traffic across API instances |
| Auto-scaling | CPU >70% threshold | Auto-scale up/down based on CPU and memory |
| CDN Integration | Global | Serve images via CloudFront/Azure CDN |

---

### NFR-3: Availability

| Requirement | Target | Implementation |
|-------------|--------|-----------------|
| Uptime SLA | 99.5% | Max 8.76 hours downtime/month allowed |
| Recovery Time Objective (RTO) | 15 minutes | Time to recover from failure |
| Recovery Point Objective (RPO) | 5 minutes | Max data loss acceptable |
| Health Checks | Every 30 seconds | Synthetic monitoring of critical endpoints |
| Auto-failover | 2-minute timeout | Automatic failover to backup database |
| Database Replication | Real-time | Master-slave replication with automatic failover |
| Backup Strategy | Daily + hourly | Point-in-time restore capability |
| Disaster Recovery Plan | Documented | Step-by-step recovery procedures |

---

### NFR-4: Security

| Requirement | Standard | Implementation |
|-------------|----------|-----------------|
| Encryption in Transit | TLS 1.3 | HTTPS for all endpoints, A+ SSL rating |
| Encryption at Rest | AES-256 | Database and S3 encryption |
| Authentication | JWT | Token-based, 24-hour expiry, refresh tokens |
| Password Hashing | bcrypt | Salt + 12 rounds minimum |
| OWASP Compliance | OWASP Top 10 | Regular security audits and penetration testing |
| SQL Injection Prevention | Parameterized Queries | Entity Framework Core prevents SQL injection |
| XSS Prevention | Content Security Policy | CSP headers, input sanitization |
| CSRF Protection | CSRF Tokens | SameSite cookies, token validation |
| Rate Limiting | 100 req/min per user | Throttle excessive requests |
| IP Whitelisting (Admin) | Admin access | Restrict admin portal to known IPs (optional) |
| API Key Management | Secrets vault | Store keys in Azure Key Vault, not in code |
| Dependency Scanning | Weekly | Check for known vulnerabilities in dependencies |
| Security Headers | All enabled | HSTS, X-Frame-Options, X-Content-Type-Options |

**Security Checklist:**
- ✅ HTTPS on all endpoints
- ✅ JWT authentication with secure key storage
- ✅ Password hashing with bcrypt
- ✅ OWASP Top 10 protection
- ✅ Regular security audits
- ✅ Penetration testing annually
- ✅ Data encryption at rest
- ✅ Audit logging for compliance

---

### NFR-5: Privacy & Compliance

| Requirement | Standard | Implementation |
|-------------|----------|-----------------|
| GDPR Compliance | EU Regulation | Data subject rights (access, deletion, portability) |
| CCPA Compliance | California Privacy | Consumer rights and opt-out mechanisms |
| Data Retention | 30 days default | Delete images after 30 days unless explicitly saved |
| PII Protection | Zero PII in logs | Never log passwords, email, medical data |
| Data Anonymization | Hashed user IDs | Scannable data separated from user identity |
| User Consent | Explicit opt-in | Clear terms of service & privacy policy |
| Audit Trail | 100% retention | Keep all changes for regulatory review |
| Data Portability | Export format | JSON/CSV download of user's data |
| Account Deletion | 30-day grace period | Right to be forgotten after 30 days |
| Cookie Policy | Explicit consent | Clear cookie banner with consent tracking |

**Privacy Commitments:**
- We never sell user data
- Images auto-deleted after 30 days
- Users can download their data anytime
- Full account deletion on request
- Transparent about data usage

---

### NFR-6: Accessibility

| Requirement | Standard | Details |
|-------------|----------|---------|
| WCAG Compliance | 2.1 Level AA | Accessible to 80%+ of users with disabilities |
| Screen Reader Support | Semantic HTML | Proper heading hierarchy, ARIA labels |
| Keyboard Navigation | 100% | All features accessible via keyboard only |
| Color Contrast | 4.5:1 minimum | WCAG AA contrast for text |
| Font Size | Scalable | Support browser zoom to 200% |
| Touch Targets | 48px minimum | Mobile button/link tap areas |
| Alt Text | All images | Descriptive alt text for images |
| Form Labels | Explicit | All inputs have associated labels |
| Error Messages | Clear | User-friendly, actionable error messages |
| Focus Visible | Always visible | Clear focus indicator for keyboard navigation |
| Language Tags | HTML lang | Proper language markup |

**Testing Tools:**
- WAVE Web Accessibility Evaluation Tool
- Axe DevTools browser extension
- NVDA screen reader (Windows)
- VoiceOver (macOS)

---

### NFR-7: Usability

| Requirement | Target | Details |
|-------------|--------|---------|
| First-time Completion | ≤3 clicks | New user should scan product in ≤3 clicks (excluding image capture) |
| Error Recovery | <30 seconds | Time to correct a mistake and retry |
| Mobile Experience | 100% responsive | Seamless experience on all screen sizes (320px+) |
| Page Load Time | <2 seconds | On 4G network, including all assets |
| Transaction Success Rate | >95% | Successful scan completion rate |
| User Confusion | <5% | Support tickets about confusing features |
| Onboarding Time | <2 minutes | New user should understand app in 2 minutes |
| Search Performance | <500ms | Ingredient search results appear instantly |

**Usability Testing:**
- User testing with personas
- Heatmap analysis (Hotjar, Crazy Egg)
- A/B testing of UI changes
- NPS surveys (target: 40+)

---

### NFR-8: Compatibility

| Category | Requirement | Details |
|----------|------------|---------|
| **Browsers** | Latest 2 versions | Chrome, Firefox, Edge, Safari, Opera |
| **Mobile Browsers** | iOS Safari, Android Chrome | Latest 2 versions |
| **Screen Sizes** | 320px to 4K | Responsive design breakpoints at 320, 768, 1024, 1440, 2560px |
| **Operating Systems** | Windows, macOS, Linux | Server-side compatibility |
| **JavaScript** | ES2020+ | Modern JavaScript features |
| **Databases** | PostgreSQL 13+ | Version compatibility testing |
| **APIs** | REST, OpenAPI 3.0 | Standard API contracts |

---

## System Architecture

### High-Level Architecture

```
┌────────────────────────────────────────────────────────────────┐
│                        FRONTEND LAYER                          │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐    │
│  │ Web Browser  │    │ Mobile Web   │    │ Admin Portal │    │
│  │ (React 18)   │    │ (Responsive) │    │  (React)     │    │
│  └──────────────┘    └──────────────┘    └──────────────┘    │
└────────────────────────┬─────────────────────────────────────┘
                         │ HTTPS / REST API
                         │
┌────────────────────────▼──────────────────────────────────────┐
│                    API GATEWAY LAYER                          │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ Swagger/OpenAPI 3.0 Documentation                       │ │
│  │ Rate Limiting (100 req/min per user)                    │ │
│  │ CORS Policy & Security Headers                          │ │
│  └──────────────────────────────────────────────────────────┘ │
└────────────────────────┬─────────────────────────────────────┘
                         │
         ┌───────────────┼───────────────┐
         │               │               │
    ┌────▼────┐    ┌────▼────┐    ┌────▼────┐
    │Auth     │    │Uploads  │    │API      │
    │Service  │    │Service  │    │Routes   │
    └────┬────┘    └────┬────┘    └────┬────┘
         │               │               │
┌────────▼───────────────▼───────────────▼────────────────────┐
│              BUSINESS LOGIC LAYER (.NET 8)                  │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ MediatR CQRS Handlers (Commands & Queries)          │   │
│  │ - CreateScanCommand → ScanService                   │   │
│  │ - CalculateScoreCommand → ScoringService            │   │
│  │ - AddIngredientCommand → IngredientService          │   │
│  └─────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ Application Services                                │   │
│  │ - IOcrService (Azure Computer Vision)               │   │
│  │ - IScoringService (Score Calculation)               │   │
│  │ - IIngredientMatchingService (Fuzzy Matching)       │   │
│  │ - IStorageService (AWS S3)                          │   │
│  │ - ICacheService (Redis)                             │   │
│  │ - IEmailService (SendGrid)                          │   │
│  └─────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ Domain Models (Entities, Value Objects, Enums)      │   │
│  │ - User, Scan, Ingredient, ScanIngredientDetail      │   │
│  └─────────────────────────────────────────────────────┘   │
└────────────────────────┬────────────────────────────────────┘
                         │
              ┌──────────┴──────────┐
              │                     │
        ┌─────▼─────┐        ┌─────▼─────┐
        │ PostgreSQL│        │   Redis   │
        │ (Primary) │        │  (Cache)  │
        └─────┬─────┘        └───────────┘
              │
        ┌─────▼──────────┐
        │ Replication    │
        │ & Backup       │
        └────────────────┘

┌──────────────────────────────────────────────────────────────┐
│                  EXTERNAL SERVICES                           │
│  ┌──────────────────┐  ┌──────────────────┐                 │
│  │ Azure Computer   │  │ AWS S3           │                 │
│  │ Vision (OCR)     │  │ (Image Storage)  │                 │
│  └──────────────────┘  └──────────────────┘                 │
│  ┌──────────────────┐  ┌──────────────────┐                 │
│  │ SendGrid         │  │ Application      │                 │
│  │ (Email)          │  │ Insights (APM)   │                 │
│  └──────────────────┘  └──────────────────┘                 │
└──────────────────────────────────────────────────────────────┘
```

### Technology Stack

| Layer | Technology | Version | Purpose |
|-------|-----------|---------|---------|
| **Frontend** | React | 18.x | UI framework |
| | Next.js | 14 | SSR, routing, optimization |
| | TypeScript | Latest | Type safety |
| | Tailwind CSS | 3.x | Responsive styling |
| | SWR / React Query | Latest | Data fetching & caching |
| **Backend** | .NET | 8.0 | Framework |
| | ASP.NET Core | 8.0 | REST API, middleware |
| | Entity Framework Core | 8.0 | ORM & database access |
| | MediatR | Latest | CQRS pattern implementation |
| | FluentValidation | Latest | Business rule validation |
| | AutoMapper | Latest | DTO mapping |
| | Serilog | Latest | Structured logging |
| **Database** | PostgreSQL | 16 | Primary relational database |
| | Npgsql | Latest | PostgreSQL ADO.NET provider |
| **Caching** | Redis | 7.x | High-speed ingredient cache |
| **Storage** | AWS S3 | Current | Image storage & retrieval |
| **OCR** | Azure Computer Vision | Latest | Text extraction from images |
| **Email** | SendGrid | API v3 | Email notifications |
| **Monitoring** | Application Insights | Latest | APM & diagnostics |
| **Testing** | xUnit | Latest | Unit testing framework |
| | Moq | Latest | Mocking & stubbing |
| | FluentAssertions | Latest | Assertion library |
| **CI/CD** | GitHub Actions | Latest | Automated build & deploy |
| **API Docs** | Swagger/OpenAPI | 3.0 | Interactive API documentation |

---

## Data Model

### Entity Relationship Diagram

```
┌───────────────┐
│    User       │
├───────────────┤
│ Id (PK)       │──┐
│ Email         │  │
│ FirstName     │  │
│ LastName      │  │ 1
│ PasswordHash  │  │
│ Allergens     │  │ *
│ Created_At    │  │
│ Last_Login    │  │
└───────────────┘  │
                   │
                   │
           ┌───────▼────────────┐
           │   Scan Result      │
           ├────────────────────┤
           │ Id (PK)            │──┐
           │ UserId (FK)        │  │
           │ ImageUrl           │  │ 1
           │ ExtractedText      │  │
           │ OCR_Confidence     │  │ *
           │ Kleen_Score        │  │
           │ Score_Category     │  │
           │ Created_At         │  │
           └────────────────────┘  │
                                   │
                    ┌──────────────┘
                    │
           ┌────────▼──────────────┐
           │ ScanIngredientDetail  │
           ├───────────────────────┤
           │ Id (PK)               │
           │ ScanId (FK)           │
           │ IngredientId (FK)─┐   │
           │ MatchConfidence   │   │
           │ PositionInList    │   │
           │ IsRiskFlag        │   │
           └───────────────────┼───┘
                               │
                    ┌──────────┘
                    │
           ┌────────▼──────────────┐
           │    Ingredient         │
           ├───────────────────────┤
           │ Id (PK)               │
           │ Name                  │
           │ Synonyms (JSON)       │
           │ CAS_Number            │
           │ Category              │
           │ Safety_Rating         │
           │ Risk_Tags (JSON)      │
           │ Source_References     │
           │ Status                │
           │ CreatedBy             │
           │ ApprovedBy            │
           │ Created_At            │
           │ Last_Updated          │
           └───────────────────────┘

┌──────────────────┐
│   Audit_Log      │
├──────────────────┤
│ Id (PK)          │
│ Entity_Type      │
│ Entity_Id        │
│ Action           │
│ Changed_By (FK)  │
│ Changed_At       │
│ Old_Values       │
│ New_Values       │
└──────────────────┘
```

### Entity Details

#### User Entity (Domain)
```csharp
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PasswordHash { get; set; }
    public List<string> Allergens { get; set; } // JSON array
    public bool IsEmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsDeleted { get; set; } // Soft delete
    
    // Navigation properties
    public ICollection<Scan> Scans { get; set; }
}
```

#### Ingredient Entity (Domain)
```csharp
public class Ingredient : IAuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<string> Synonyms { get; set; } // JSON array
    public string CasNumber { get; set; }
    public IngredientCategory Category { get; set; }
    public int SafetyRating { get; set; } // 1-10
    public List<RiskTag> RiskTags { get; set; } // JSON array of enums
    public List<string> SourceReferences { get; set; } // JSON array
    public IngredientStatus Status { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdated { get; set; }
}
```

#### Scan Result Entity (Domain)
```csharp
public class Scan : IAuditableEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ImageUrl { get; set; }
    public string ExtractedText { get; set; }
    public decimal OcrConfidence { get; set; }
    public int IngredientIQ { get; set; }
    public ScoreCategory ScoreCategory { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdated { get; set; }
    
    // Navigation properties
    public User User { get; set; }
    public ICollection<ScanIngredientDetail> IngredientDetails { get; set; }
}
```

#### ScanIngredientDetail Entity (Domain)
```csharp
public class ScanIngredientDetail
{
    public Guid Id { get; set; }
    public Guid ScanId { get; set; }
    public Guid IngredientId { get; set; }
    public decimal MatchConfidence { get; set; } // 0.0 to 1.0
    public int PositionInList { get; set; }
    public bool IsRiskFlag { get; set; }
    
    // Navigation properties
    public Scan Scan { get; set; }
    public Ingredient Ingredient { get; set; }
}
```

---

## User Journey

### Happy Path: Product Scanning

**Step 1: User Arrives at Platform**
```
Homepage
├── Hero Section
│   ├── Headline: "Know Your Ingredients"
│   ├── Subheading: "Scan. Analyze. Decide."
│   └── CTA: "Scan a Product" (primary button)
├── Features Overview
│   ├── "Upload from Gallery"
│   ├── "Capture Photo"
│   └── "Get Safety Score"
└── Social Proof
    ├── "500,000+ ingredients analyzed"
    ├── "4.8★ rating"
    └── User testimonials
```

**Step 2: Upload/Capture Image**
```
Select Image Source
├── Option 1: Upload from Device
│   ├── File picker opens
│   ├── Accept JPG, PNG, HEIC, WEBP
│   ├── Max 10MB enforced
│   └── Image preview shown
├── Option 2: Capture Photo
│   ├── Camera permission requested
│   ├── On-screen guidance shown:
│   │   ├── "Position label in frame"
│   │   ├── "Ensure good lighting"
│   │   ├── "Keep parallel to camera"
│   │   └── "Zoom to fit frame"
│   └── Real-time focus indicator
└── Image Quality Check
    ├── Minimum resolution: 300x300px
    ├── Blur detection (Laplacian variance)
    ├── Reject if quality too low
    └── Show error message & allow retry
```

**Step 3: Image Processing (Backend)**
```
1. Store image temporarily in S3
2. Pre-process image
   ├── Auto-correct rotation (deskew)
   ├── Reduce noise (denoise)
   ├── Normalize contrast
   └── Remove glare reflections
3. Send to Azure Computer Vision API
4. Extract text with confidence scoring
5. Detect "Ingredients:" section
6. Parse ingredients into list
```

**Step 4: User Review Extracted Ingredients**
```
Review Screen
├── Display
│   ├── Original image (thumb preview)
│   ├── Extracted text
│   ├── OCR confidence: "94%"
│   └── Warning if <75%: "Quality was low - review carefully"
├── Edit Capability
│   ├── Click to edit any extracted ingredient
│   ├── Add missing ingredients
│   ├── Delete incorrectly recognized items
│   └── "Save & Continue" button
└── Alternative Actions
    ├── "Retake Photo" (start over)
    ├── "Manual Entry" (type ingredients)
    └── "Cancel"
```

**Step 5: Calculate Kleen Score**
```
Backend Processing
├── For each extracted ingredient:
│   ├── Try exact match against database
│   ├── If no match, try fuzzy matching (≥90% confidence)
│   ├── Get safety rating (1-10)
│   ├── Get risk tags
│   └── Queue unrecognized for admin review
├── Apply position-based weighting
├── Check for user allergens
├── Apply personalization penalty
├── Calculate final score (0-100)
├── Determine category (Green/Yellow/Red)
└── Save to database
```

**Step 6: Display Results**
```
Results Screen
├── Score Display (Large, Center)
│   ├── Visual gauge: 0 — [█████████░] — 100
│   ├── Large number: "72"
│   ├── Category label: "MODERATE" (Yellow background)
│   ├── Emoji: 😐
│   └── Summary: "This product has some concerns"
├── Ingredient Breakdown Table
│   ├── Columns: Ingredient | Rating | Risk Tags | Confidence
│   ├── Row icons: ✅ (Good) | ❌ (Bad) | ❓ (Unknown)
│   └── Sortable, filterable
├── Top Concerns Section
│   ├── 1. "Benzene" - Carcinogen (Rating: 1/10)
│   │   └── "Avoid if possible"
│   ├── 2. "Artificial Color" - (Rating: 3/10)
│   │   └── "May cause hyperactivity in children"
│   └── 3. "Sodium" - High (Rating: 4/10)
│       └── "Exceeds daily recommended intake"
├── "Why This Score?" (Collapsible)
│   ├── Calculation methodology
│   ├── Ingredient breakdown
│   ├── User allergen impact (if applicable)
│   └── Recommendations
└── Action Buttons
    ├── "Save to History" (requires login)
    ├── "Share" (social/link)
    ├── "Download PDF"
    └── "Scan Another"
```

**Step 7: Take Action**
```
User Actions
├── Save Result
│   ├── Requires login
│   ├── Stores in personal history
│   └── Can add notes: "Avoid - contains allergen"
├── Share Result
│   ├── Generate unique shareable link
│   ├── Link expires in 24 hours
│   ├── Include: Score, ingredients, image
│   └── Share to: WhatsApp, Facebook, Twitter
├── Download PDF
│   ├── Full report with branding
│   ├── Score summary & breakdown
│   ├── Ingredient list with ratings
│   ├── Recommendations
│   └── Disclaimer: "Not medical advice"
└── View Similar Products (Phase 2)
    ├── Suggest alternatives with better scores
    ├── Link to affiliate product pages
    └── Earn commission on sales
```

---

## Assumptions & Constraints

### Key Assumptions

1. **User Environment**
   - Assumption: Users have smartphone or webcam capable of capturing legible product label photos
   - Impact: Guides image validation requirements

2. **Image Quality**
   - Assumption: Most label images will be relatively clear and in reasonable focus
   - Impact: Minimum quality standards set accordingly

3. **Language Support**
   - Assumption: Ingredient labels are predominantly in English at MVP launch
   - Impact: Dates multi-language support to Phase 3

4. **Internet Connectivity**
   - Assumption: Users have stable internet connection for API calls
   - Impact: Offline mode not required in MVP

5. **OCR Accuracy**
   - Assumption: Cloud OCR APIs (Azure Computer Vision) achieve ≥85% accuracy on standard food/cosmetic labels
   - Impact: Performance benchmarks based on this

6. **Database Maintenance**
   - Assumption: SME team can curate and maintain ingredient database with reasonable effort
   - Impact: Staffing plan includes nutritionists/dermatologists

7. **User Behavior**
   - Assumption: Users will scan products before purchase (not post-consumption)
   - Impact: Affects value proposition and use cases

8. **Regulatory Environment**
   - Assumption: Ingredient safety ratings are relatively stable (not changing rapidly)
   - Impact: Database updates quarterly, not real-time

### Constraints

1. **OCR Limitations**
   - Constraint: OCR accuracy varies significantly with label quality, font size, language, and glare
   - Mitigation: Provide capture guidance + manual edit capability

2. **Database Coverage**
   - Constraint: Not all obscure or regional ingredients will be in database at launch
   - Mitigation: Queue unknown ingredients for admin review, treat as neutral initially

3. **Regulatory & Liability**
   - Constraint: Must include clear "not medical advice" disclaimers per legal guidance
   - Mitigation: Prominent disclaimers, insurance, regular legal review

4. **Operational Costs**
   - Constraint: OCR API calls and S3 storage add significant operational costs
   - Mitigation: Implement caching, image compression, price-based tiering

5. **Latency from External APIs**
   - Constraint: Third-party OCR services may add 2-4 seconds to response time
   - Mitigation: Use async processing, queue long requests

6. **Database Query Performance**
   - Constraint: Ingredient lookup on first scan can be slow without proper indexing
   - Mitigation: Strategic indexing, Redis caching, query optimization

7. **Image Storage Volume**
   - Constraint: Storage costs grow with user base and scans
   - Mitigation: Auto-delete images after 30 days, implement compression

8. **Data Privacy**
   - Constraint: GDPR/CCPA regulations limit data collection and retention
   - Mitigation: Privacy-first design, minimal PII collection, clear consent

---

## Risk Management

### Risk Register

| Risk ID | Risk | Likelihood | Impact | Severity | Mitigation Strategy |
|---------|------|-----------|--------|----------|-------------------|
| **R1** | Low OCR accuracy on poorly lit/curved labels | HIGH | HIGH | **CRITICAL** | Provide detailed capture guidance; implement manual edit UI; test on 100+ real labels |
| **R2** | Incomplete ingredient database at launch | HIGH | HIGH | **CRITICAL** | Crowdsource ingredients from users; partner with nutritionists; pre-load 500+ items |
| **R3** | Legal liability from health claims | MEDIUM | CRITICAL | **CRITICAL** | Clear disclaimers; insurance; SME review; legal team oversight; content review process |
| **R4** | Data breach exposing user allergen/health data | LOW | CRITICAL | **CRITICAL** | Encryption at rest/transit; regular audits; penetration testing; bug bounty program |
| **R5** | API rate limiting from OCR service | MEDIUM | MEDIUM | **HIGH** | Implement queue system; batch processing; backup OCR provider; cost monitoring |
| **R6** | Database query performance degradation | MEDIUM | HIGH | **HIGH** | Strategic indexing; Redis caching; query optimization; load testing before launch |
| **R7** | User confusion with scoring methodology | MEDIUM | MEDIUM | **HIGH** | Transparent formula display; educational content; user testing; clear explanations |
| **R8** | Negative brand impact from inaccurate scores | MEDIUM | HIGH | **HIGH** | Extensive testing; expert validation; feedback loops; rapid fixes |
| **R9** | Image storage costs exceed budget | MEDIUM | MEDIUM | **MEDIUM** | Image compression; 30-day auto-deletion; cost monitoring; tiered pricing |
| **R10** | Third-party API downtime (Azure OCR) | MEDIUM | HIGH | **HIGH** | Backup OCR provider; graceful degradation; user communication |

### Risk Mitigation Timeline

| Phase | Mitigation Actions |
|-------|-------------------|
| **Pre-Launch** | Liability insurance, legal review, OCR accuracy testing, user testing |
| **Launch** | Monitor OCR accuracy, gather feedback, 24/7 monitoring, rapid response team |
| **Month 1** | Database expansion, user feedback implementation, security audit |
| **Month 2-3** | Performance optimization, cost analysis, user behavior analytics |

---

## Success Metrics (KPIs)

### User Engagement Metrics

| Metric | Target (Month 6) | Measurement Method | Threshold |
|--------|-----------------|-------------------|-----------|
| **Monthly Active Users (MAU)** | 50,000 | Analytics dashboard, unique user count | Pass if ≥40,000 |
| **Daily Active Users (DAU)** | 15,000 | Daily unique visitors | Pass if ≥12,000 |
| **Scans per User/Month** | 2.5+ | Total scans ÷ active users | Pass if ≥2.0 |
| **7-Day Retention (D7)** | >50% | Cohort analysis (users active Day 1 & Day 7) | Pass if >45% |
| **30-Day Retention (D30)** | >30% | Cohort analysis (users active Day 1 & Day 30) | Pass if >25% |
| **Signup Conversion** | 25% | Guests converted to signup ÷ guests | Pass if >20% |
| **Scan Completion Rate** | >85% | Successful scans ÷ started scans | Pass if >80% |
| **Average Session Duration** | 5+ minutes | Google Analytics | Pass if >4 minutes |

### Product Quality Metrics

| Metric | Target | Measurement Method | Benchmark |
|--------|--------|-------------------|-----------|
| **OCR Accuracy** | >85% | Manual validation sampling (test 100+ labels) | Industry: 80-90% |
| **Ingredient Match Rate** | >90% | Matched ingredients ÷ total ingredients | Custom: Good accuracy |
| **Response Time (p50)** | <500ms | APM monitoring | Excellent |
| **Response Time (p95)** | <4 seconds | APM monitoring | Good |
| **Response Time (p99)** | <8 seconds | APM monitoring | Acceptable |
| **API Uptime** | 99.5% | Synthetic monitoring | SLA commitment |
| **Zero Data Loss** | 100% | Backup verification | Production standard |

### Business Metrics

| Metric | Target | Measurement | Impact |
|--------|--------|-------------|--------|
| **NPS Score** | 40+ | Quarterly surveys | User satisfaction |
| **Customer Support Response Time** | <4 hours | Support ticket timestamps | User experience |
| **Cost per OCR Scan** | <$0.05 | Total API costs ÷ scans | Profitability |
| **Paid Subscription Conversion** | 10%+ | Paid users ÷ registered users | Revenue |
| **Customer Lifetime Value** | $15+ | Revenue per user - acquisition cost | Long-term value |
| **User Acquisition Cost** | <$2 | Marketing spend ÷ new users | Efficiency |
| **Brand Sentiment** | Positive | Social media monitoring, reviews | Reputation |

### Technical Metrics

| Metric | Target | Tool | Threshold |
|--------|--------|------|-----------|
| **Code Coverage** | ≥80% | xUnit + Coverlet | Pass if >75% |
| **Critical Bugs** | 0 | Bug tracking system | Zero tolerance |
| **Security Vulnerabilities** | 0 critical | SAST scanning + penetration testing | Zero critical |
| **Deployment Frequency** | 2x per week | CI/CD pipeline | Agile deployment |
| **Lead Time for Changes** | <2 days | Git commit to production | Rapid iteration |
| **Mean Time to Recovery** | <15 minutes | Incident tracking | Quick recovery |

---

## Release Plan

### MVP (Phase 1) — Months 1-3

**Objectives:**
- Launch fully functional MVP with core features
- Establish market presence
- Gather user feedback
- Prove product-market fit

**Deliverables:**
1. User registration & authentication system
2. Image upload & camera capture
3. OCR extraction with manual review
4. 300+ ingredient database (curated by SMEs)
5. Score calculation & display
6. Scan history management
7. Admin ingredient management portal
8. Comprehensive API documentation (Swagger)
9. Monitoring & logging infrastructure
10. Security audit & penetration testing

**Success Criteria:**
- ✅ 10,000+ registered users
- ✅ 50,000+ scans performed
- ✅ OCR accuracy >85%
- ✅ Response time p95 <4s
- ✅ NPS score >30
- ✅ Zero critical security issues
- ✅ 99.5% uptime

---

### Phase 2 — Months 4-5

**Focus:** User Experience & Engagement

**New Features:**
- Advanced user profiles with preferences
- Allergen tracking & personalized alerts
- Scan history with filters & search
- Social sharing (Twitter, Facebook, WhatsApp)
- PDF export of reports
- Email notifications for saved products
- Enhanced admin dashboard with analytics
- Bulk ingredient import (CSV)
- Ingredient approval workflow

**Improvements:**
- UI/UX refinements based on user feedback
- Performance optimization
- Mobile experience enhancement
- Search functionality

**Targets:**
- 30,000+ registered users
- 150,000+ total scans
- D30 retention >30%
- Signup conversion 25%+

---

### Phase 3 — Months 6-7

**Focus:** Monetization & Scale

**Premium Features:**
- Alternative product suggestions with affiliate links
- Product comparison tools
- Batch scanning (upload 10+ labels at once)
- Custom ingredient lists for teams/organizations
- API for third-party integrations
- Advanced analytics & reporting

**Partnerships:**
- Brand partnerships for featured products
- Retailer integrations (Whole Foods, Amazon Fresh)
- Health app integrations (Apple Health, Google Fit)

**Targets:**
- 50,000+ registered users
- 250,000+ total scans
- 10%+ paid subscription conversion
- Revenue target: $50k/month

---

### Phase 4+ — Month 8+

**Future Roadmap:**
- Native mobile apps (iOS & Android)
- Barcode scanning capability
- Multi-language support (Spanish, French, German)
- AI-powered ingredient recommendations
- Blockchain-based verification (aspirational)
- Partner ecosystem expansion

---

## Acceptance Criteria

### AC-1: MVP Functional Completeness

- ✅ User can register with email & password in <30 seconds
- ✅ User can upload JPG/PNG/HEIC image and system validates within 2 seconds
- ✅ User can capture photo via device camera and see real-time preview
- ✅ OCR extraction completes in <8 seconds with ≥85% accuracy
- ✅ User can manually edit extracted ingredients and system reflects changes
- ✅ System calculates Kleen Score following documented formula
- ✅ Score displayed with visual gauge, color coding, and emoji indicator
- ✅ Ingredient breakdown shown with individual ratings and risk tags
- ✅ User can save scan to history (requires login)
- ✅ User can share scan via link (24-hour expiry)
- ✅ User can download PDF report with all details
- ✅ Admin can add/edit/delete ingredients via portal UI
- ✅ New ingredients appear in scoring within 1 minute
- ✅ All pages load in <2 seconds on 4G network
- ✅ Mobile experience fully responsive on 320px+ screens

### AC-2: Quality & Reliability Standards

- ✅ Unit test coverage ≥80% of business logic
- ✅ Integration tests cover all critical paths
- ✅ Zero critical security vulnerabilities (OWASP Top 10)
- ✅ API response times meet SLA: p50 <500ms, p95 <4s, p99 <8s
- ✅ Database queries optimized with proper indexing
- ✅ Zero data loss in production (backup/restore tested)
- ✅ Graceful error handling with user-friendly messages
- ✅ All API endpoints documented in Swagger

### AC-3: Admin Functionality

- ✅ Admin can perform CRUD operations on ingredients
- ✅ Admin can bulk import ingredients from CSV
- ✅ Admin can approve/reject user-submitted ingredients
- ✅ Admin can view real-time dashboard with stats
- ✅ Audit logs capture all changes (who, what, when)
- ✅ Admin role-based permissions enforced

### AC-4: User Experience & Accessibility

- ✅ First-time user completes scan in ≤3 clicks (excluding image capture)
- ✅ Error messages are clear, specific, and actionable
- ✅ Mobile experience seamless without horizontal scrolling
- ✅ All buttons/links have minimum 48px touch target
- ✅ Color contrast meets WCAG 2.1 AA (4.5:1 for text)
- ✅ Keyboard navigation works fully (Tab through all elements)
- ✅ Screen reader compatible (NVDA/JAWS testing)
- ✅ Focus indicators visible throughout

### AC-5: Performance & Scalability

- ✅ System handles 10,000 concurrent users without degradation
- ✅ Auto-scaling works (CPU >70% triggers scale-up)
- ✅ Load test passes: 10,000 RPS with p95 <4s
- ✅ Database connection pooling prevents exhaustion
- ✅ Redis cache hits >80% for ingredient lookups
- ✅ OCR API calls queued efficiently (no rate limiting)

### AC-6: Security & Privacy

- ✅ All endpoints require HTTPS (TLS 1.3)
- ✅ Passwords hashed with bcrypt (12 rounds)
- ✅ JWT tokens expire in 24 hours
- ✅ CORS policy restricts to known domains only
- ✅ Rate limiting enforced: 100 req/min per user
- ✅ SQL injection prevention (parameterized queries)
- ✅ XSS prevention (input sanitization + CSP headers)
- ✅ CSRF protection enabled (SameSite cookies)
- ✅ PII never logged (passwords, emails, etc.)
- ✅ User data encrypted at rest (AES-256)
- ✅ GDPR data export working (JSON format)
- ✅ Account deletion completes within 30 days

---

# .NET Solution Structure & Implementation Guide

## Recommended .NET Project Architecture

### Solution Organization

```
IngredientIQ.sln (Solution file)
│
├── src/
│   ├── IngredientIQ.Api/                              [API Entry Point]
│   │   ├── Controllers/
│   │   │   ├── UsersController.cs
│   │   │   ├── ScansController.cs
│   │   │   ├── IngredientsController.cs
│   │   │   └── AdminController.cs
│   │   ├── Middleware/
│   │   │   ├── ErrorHandlingMiddleware.cs
│   │   │   ├── AuthenticationMiddleware.cs
│   │   │   └── RateLimitingMiddleware.cs
│   │   ├── Program.cs                               [Startup & DI configuration]
│   │   ├── Startup.cs                               [Legacy startup (if needed)]
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   ├── appsettings.Production.json
│   │   └── IngredientIQ.Api.csproj
│   │
│   ├── IngredientIQ.Application/                      [Business Logic & CQRS]
│   │   ├── Features/
│   │   │   ├── Auth/
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── RegisterUserCommand.cs
│   │   │   │   │   ├── RegisterUserCommandHandler.cs
│   │   │   │   │   ├── LoginCommand.cs
│   │   │   │   │   └── LoginCommandHandler.cs
│   │   │   │   ├── Queries/
│   │   │   │   │   ├── GetCurrentUserQuery.cs
│   │   │   │   │   └── GetCurrentUserQueryHandler.cs
│   │   │   │   └── DTOs/
│   │   │   │       ├── RegisterUserDto.cs
│   │   │   │       ├── LoginRequestDto.cs
│   │   │   │       └── LoginResponseDto.cs
│   │   │   │
│   │   │   ├── Scans/
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── CreateScanCommand.cs
│   │   │   │   │   ├── CreateScanCommandHandler.cs
│   │   │   │   │   ├── SaveScanCommand.cs
│   │   │   │   │   └── SaveScanCommandHandler.cs
│   │   │   │   ├── Queries/
│   │   │   │   │   ├── GetUserScansQuery.cs
│   │   │   │   │   ├── GetUserScansQueryHandler.cs
│   │   │   │   │   ├── GetScanByIdQuery.cs
│   │   │   │   │   └── GetScanByIdQueryHandler.cs
│   │   │   │   └── DTOs/
│   │   │   │       ├── CreateScanDto.cs
│   │   │   │       ├── ScanResultDto.cs
│   │   │   │       └── IngredientBreakdownDto.cs
│   │   │   │
│   │   │   ├── Ingredients/
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── AddIngredientCommand.cs
│   │   │   │   │   ├── AddIngredientCommandHandler.cs
│   │   │   │   │   ├── UpdateIngredientCommand.cs
│   │   │   │   │   ├── BulkImportIngredientsCommand.cs
│   │   │   │   │   └── BulkImportIngredientsCommandHandler.cs
│   │   │   │   ├── Queries/
│   │   │   │   │   ├── GetIngredientsQuery.cs
│   │   │   │   │   ├── GetIngredientsQueryHandler.cs
│   │   │   │   │   ├── SearchIngredientsQuery.cs
│   │   │   │   │   └── SearchIngredientsQueryHandler.cs
│   │   │   │   └── DTOs/
│   │   │   │       ├── IngredientDto.cs
│   │   │   │       ├── CreateIngredientDto.cs
│   │   │   │       └── IngredientSearchResultDto.cs
│   │   │   │
│   │   │   └── Scoring/
│   │   │       ├── Commands/
│   │   │       │   ├── CalculateScoreCommand.cs
│   │   │       │   └── CalculateScoreCommandHandler.cs
│   │   │       ├── Queries/
│   │   │       │   └── (Query types if needed)
│   │   │       └── DTOs/
│   │   │           └── ScoringResultDto.cs
│   │   │
│   │   ├── Services/
│   │   │   ├── IOcrService.cs
│   │   │   ├── IScoringService.cs
│   │   │   ├── IIngredientMatchingService.cs
│   │   │   ├── ICacheService.cs
│   │   │   ├── IEmailService.cs
│   │   │   ├── IStorageService.cs
│   │   │   ├── IAuthenticationService.cs
│   │   │   └── IPasswordHasher.cs
│   │   │
│   │   ├── Validators/
│   │   │   ├── RegisterUserValidator.cs
│   │   │   ├── CreateScanValidator.cs
│   │   │   ├── AddIngredientValidator.cs
│   │   │   └── LoginValidator.cs
│   │   │
│   │   ├── Exceptions/
│   │   │   ├── DomainException.cs
│   │   │   ├── ValidationException.cs
│   │   │   ├── NotFoundException.cs
│   │   │   ├── UnauthorizedException.cs
│   │   │   └── ConflictException.cs
│   │   │
│   │   ├── Mappings/
│   │   │   └── MappingProfile.cs                    [AutoMapper profiles]
│   │   │
│   │   └── IngredientIQ.Application.csproj
│   │
│   ├── IngredientIQ.Domain/                           [Core Domain & Entities]
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Scan.cs
│   │   │   ├── Ingredient.cs
│   │   │   ├── ScanIngredientDetail.cs
│   │   │   └── AuditLog.cs
│   │   │
│   │   ├── Enums/
│   │   │   ├── IngredientCategory.cs
│   │   │   ├── IngredientStatus.cs
│   │   │   ├── RiskTag.cs
│   │   │   ├── ScoreCategory.cs
│   │   │   └── UserRole.cs
│   │   │
│   │   ├── ValueObjects/
│   │   │   ├── Email.cs
│   │   │   ├── Score.cs
│   │   │   └── SafetyRating.cs
│   │   │
│   │   ├── Interfaces/
│   │   │   ├── IRepository.cs                       [Generic repository interface]
│   │   │   ├── IUnitOfWork.cs                       [Unit of work pattern]
│   │   │   └── IAuditableEntity.cs
│   │   │
│   │   ├── Constants/
│   │   │   └── DomainConstants.cs
│   │   │
│   │   └── IngredientIQ.Domain.csproj
│   │
│   ├── IngredientIQ.Infrastructure/                   [EF Core, External Services]
│   │   ├── Persistence/
│   │   │   ├── IngredientIQDbContext.cs               [EF Core DbContext]
│   │   │   ├── Repositories/
│   │   │   │   ├── GenericRepository.cs             [Generic repository implementation]
│   │   │   │   ├── UserRepository.cs
│   │   │   │   ├── ScanRepository.cs
│   │   │   │   └── IngredientRepository.cs
│   │   │   ├── Migrations/
│   │   │   │   ├── 20240101000000_InitialCreate.cs
│   │   │   │   └── (Future migrations...)
│   │   │   ├── Configurations/
│   │   │   │   ├── UserConfiguration.cs
│   │   │   │   ├── ScanConfiguration.cs
│   │   │   │   ├── IngredientConfiguration.cs
│   │   │   │   ├── ScanIngredientDetailConfiguration.cs
│   │   │   │   └── AuditLogConfiguration.cs
│   │   │   └── UnitOfWork.cs                        [Unit of work implementation]
│   │   │
│   │   ├── Services/
│   │   │   ├── OcrService.cs                        [Azure Computer Vision integration]
│   │   │   ├── ScoringService.cs
│   │   │   ├── IngredientMatchingService.cs         [Fuzzy matching logic]
│   │   │   ├── CacheService.cs                      [Redis wrapper]
│   │   │   ├── StorageService.cs                    [AWS S3 integration]
│   │   │   ├── EmailService.cs                      [SendGrid integration]
│   │   │   ├── AuthenticationService.cs             [JWT token management]
│   │   │   └── PasswordHasher.cs
│   │   │
│   │   ├── ExternalApis/
│   │   │   ├── AzureComputerVisionClient.cs
│   │   │   └── AzureComputerVisionOptions.cs
│   │   │
│   │   ├── Caching/
│   │   │   ├── RedisCacheService.cs
│   │   │   └── CacheKeys.cs
│   │   │
│   │   ├── DependencyInjection.cs                   [Register infrastructure services]
│   │   ├── appsettings.Infrastructure.json
│   │   └── IngredientIQ.Infrastructure.csproj
│   │
│   └── IngredientIQ.Shared/                           [Shared Utilities & DTOs]
│       ├── Responses/
│       │   ├── ApiResponse.cs                       [Generic API response wrapper]
│       │   ├── ErrorResponse.cs
│       │   └── PagedResponse.cs
│       ├── Constants/
│       │   ├── ValidationMessages.cs
│       │   ├── ErrorMessages.cs
│       │   └── ApiRoutes.cs
│       ├── Extensions/
│       │   ├── StringExtensions.cs
│       │   ├── CollectionExtensions.cs
│       │   └── DateTimeExtensions.cs
│       ├── Utilities/
│       │   ├── JwtTokenGenerator.cs
│       │   ├── PasswordHasher.cs
│       │   └── DateTimeProvider.cs
│       └── IngredientIQ.Shared.csproj
│
├── tests/
│   ├── IngredientIQ.UnitTests/
│   │   ├── Features/
│   │   │   ├── AuthTests/
│   │   │   │   ├── RegisterUserCommandHandlerTests.cs
│   │   │   │   ├── LoginCommandHandlerTests.cs
│   │   │   │   └── PasswordHasherTests.cs
│   │   │   ├── ScansTests/
│   │   │   │   ├── CreateScanCommandHandlerTests.cs
│   │   │   │   └── ScanRepositoryTests.cs
│   │   │   └── ScoringTests/
│   │   │       ├── ScoringServiceTests.cs
│   │   │       └── IngredientMatchingServiceTests.cs
│   │   ├── Services/
│   │   │   ├── OcrServiceTests.cs
│   │   │   ├── IngredientMatchingServiceTests.cs
│   │   │   └── ScoringServiceTests.cs
│   │   ├── Validators/
│   │   │   ├── RegisterUserValidatorTests.cs
│   │   │   ├── CreateScanValidatorTests.cs
│   │   │   └── AddIngredientValidatorTests.cs
│   │   ├── Fixtures/
│   │   │   ├── TestDataFixture.cs
│   │   │   └── AutoMockerFixture.cs
│   │   ├── GlobalUsings.cs
│   │   └── IngredientIQ.UnitTests.csproj
│   │
│   └── IngredientIQ.IntegrationTests/
│       ├── Features/
│       │   ├── AuthTests/
│       │   │   ├── UserRegistrationIntegrationTests.cs
│       │   │   └── UserLoginIntegrationTests.cs
│       │   ├── ScansTests/
│       │   │   ├── ScanCreationIntegrationTests.cs
│       │   │   └── ScanHistoryIntegrationTests.cs
│       │   └── IngredientsTests/
│       │       └── IngredientManagementIntegrationTests.cs
│       ├── ApiTests/
│       │   ├── UsersControllerTests.cs
│       │   ├── ScansControllerTests.cs
│       │   └── IngredientsControllerTests.cs
│       ├── Fixtures/
│       │   ├── TestDatabaseFixture.cs
│       │   ├── WebApplicationFactory.cs
│       │   └── SqliteInMemoryDatabaseFixture.cs
│       ├── Setup/
│       │   └── TestStartup.cs
│       ├── GlobalUsings.cs
│       └── IngredientIQ.IntegrationTests.csproj
│
├── docs/
│   ├── ARCHITECTURE.md
│   ├── API_DOCUMENTATION.md
│   ├── DATABASE_SCHEMA.md
│   ├── SETUP_GUIDE.md
│   ├── DEPLOYMENT.md
│   └── CONTRIBUTION_GUIDELINES.md
│
├── .github/
│   └── workflows/
│       ├── build-and-test.yml
│       ├── deploy-staging.yml
│       ├── deploy-production.yml
│       └── security-scan.yml
│
├── .gitignore
├── README.md
├── LICENSE
└── IngredientIQ.sln
```

---

## Project Creation Steps

### Step 1: Create Solution & Projects

```bash
# Navigate to repository root
cd C:\Users\sinhaa19\source\repos\Kleen\ Label\ AI

# Create solution
dotnet new sln -n IngredientIQ

# Create Domain project
dotnet new classlib -n IngredientIQ.Domain -o src/IngredientIQ.Domain -f net8.0

# Create Application project
dotnet new classlib -n IngredientIQ.Application -o src/IngredientIQ.Application -f net8.0

# Create Infrastructure project
dotnet new classlib -n IngredientIQ.Infrastructure -o src/IngredientIQ.Infrastructure -f net8.0

# Create Shared project
dotnet new classlib -n IngredientIQ.Shared -o src/IngredientIQ.Shared -f net8.0

# Create API project
dotnet new webapi -n IngredientIQ.Api -o src/IngredientIQ.Api -f net8.0

# Create Unit Tests project
dotnet new xunit -n IngredientIQ.UnitTests -o tests/IngredientIQ.UnitTests -f net8.0

# Create Integration Tests project
dotnet new xunit -n IngredientIQ.IntegrationTests -o tests/IngredientIQ.IntegrationTests -f net8.0
```

### Step 2: Add Projects to Solution

```bash
cd C:\Users\sinhaa19\source\repos\Kleen\ Label\ AI

# Add all projects to solution
dotnet sln add src/IngredientIQ.Domain
dotnet sln add src/IngredientIQ.Application
dotnet sln add src/IngredientIQ.Infrastructure
dotnet sln add src/IngredientIQ.Shared
dotnet sln add src/IngredientIQ.Api
dotnet sln add tests/IngredientIQ.UnitTests
dotnet sln add tests/IngredientIQ.IntegrationTests
```

### Step 3: Setup Project References

```bash
# Application layer references Domain & Shared
cd src/IngredientIQ.Application
dotnet add reference ../IngredientIQ.Domain ../IngredientIQ.Shared
cd ../..

# Infrastructure layer references Domain, Application & Shared
cd src/IngredientIQ.Infrastructure
dotnet add reference ../IngredientIQ.Domain ../IngredientIQ.Application ../IngredientIQ.Shared
cd ../..

# API layer references all layers
cd src/IngredientIQ.Api
dotnet add reference ../IngredientIQ.Domain ../IngredientIQ.Application ../IngredientIQ.Infrastructure ../IngredientIQ.Shared
cd ../..

# Test projects
cd tests/IngredientIQ.UnitTests
dotnet add reference ../../src/IngredientIQ.Domain ../../src/IngredientIQ.Application ../../src/IngredientIQ.Infrastructure
cd ../..

cd tests/IngredientIQ.IntegrationTests
dotnet add reference ../../src/IngredientIQ.Domain ../../src/IngredientIQ.Application ../../src/IngredientIQ.Infrastructure ../../src/IngredientIQ.Api
cd ../..
```

### Step 4: Add Required NuGet Packages

```bash
# --- API Project Packages ---
cd src/IngredientIQ.Api

# API Documentation & Swagger
dotnet add package Swashbuckle.AspNetCore --version 6.4.0
dotnet add package Swashbuckle.AspNetCore.Annotations --version 6.4.0

# Logging
dotnet add package Serilog.AspNetCore --version 7.0.0
dotnet add package Serilog.Sinks.Console --version 4.1.0
dotnet add package Serilog.Sinks.File --version 5.0.0

# Configuration
dotnet add package Microsoft.Extensions.Configuration --version 8.0.0
dotnet add package Microsoft.Extensions.Configuration.Json --version 8.0.0

cd ../..

# --- Application Project Packages ---
cd src/IngredientIQ.Application

# CQRS Pattern
dotnet add package MediatR --version 12.1.1
dotnet add package MediatR.Extensions.Microsoft.DependencyInjection --version 11.1.0

# Validation
dotnet add package FluentValidation --version 11.7.1
dotnet add package FluentValidation.DependencyInjectionExtensions --version 11.7.1

# Mapping
dotnet add package AutoMapper --version 13.0.1
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 12.0.1

cd ../..

# --- Infrastructure Project Packages ---
cd src/IngredientIQ.Infrastructure

# Entity Framework Core
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.PostgreSQL --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add package Npgsql --version 8.0.0

# Caching
dotnet add package StackExchange.Redis --version 2.6.122

# AWS SDK
dotnet add package AWSSDK.S3 --version 3.7.307.0

# Azure Cognitive Services
dotnet add package Azure.AI.Vision.ImageAnalysis --version 1.0.0-beta.1

# Email Service
dotnet add package SendGrid --version 10.0.0

# JWT & Identity
dotnet add package System.IdentityModel.Tokens.Jwt --version 7.0.0
dotnet add package Microsoft.AspNetCore.Identity --version 8.0.0

# String Similarity (for fuzzy matching)
dotnet add package FuzzySharp --version 1.4.1

cd ../..

# --- Shared Project Packages ---
cd src/IngredientIQ.Shared
# Usually minimal - shared by other projects
cd ../..

# --- Unit Tests Packages ---
cd tests/IngredientIQ.UnitTests

dotnet add package xunit --version 2.6.4
dotnet add package xunit.runner.visualstudio --version 2.5.4
dotnet add package Moq --version 4.20.70
dotnet add package FluentAssertions --version 6.12.0
dotnet add package Bogus --version 35.4.1

cd ../..

# --- Integration Tests Packages ---
cd tests/IngredientIQ.IntegrationTests

dotnet add package xunit --version 2.6.4
dotnet add package xunit.runner.visualstudio --version 2.5.4
dotnet add package Microsoft.AspNetCore.Mvc.Testing --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 8.0.0

cd ../..
```

### Step 5: Restore and Build

```bash
cd C:\Users\sinhaa19\source\repos\Kleen\ Label\ AI

# Restore NuGet packages
dotnet restore

# Build solution
dotnet build

# Run tests to verify setup
dotnet test
```

---

## Key Implementation Patterns & Best Practices

### SOLID Principles Implementation

1. **Single Responsibility (S)**
   - Each class has one reason to change
   - `IScoringService`, `IOcrService`, `IStorageService` each handle one concern

2. **Open/Closed (O)**
   - Classes open for extension, closed for modification
   - New scoring algorithms added by implementing `IScoringService`

3. **Liskov Substitution (L)**
   - All `IRepository` implementations are interchangeable
   - Can swap PostgreSQL repository for mock in tests

4. **Interface Segregation (I)**
   - Clients depend on specific interfaces
   - `IOcrService` only exposes OCR methods, not everything

5. **Dependency Inversion (D)**
   - Depend on abstractions, not concrete implementations
   - All services registered via DI container

### Design Patterns

- **CQRS**: Commands for mutations, Queries for reads
- **Repository Pattern**: Data access abstraction layer
- **Unit of Work**: Transaction management across repositories
- **Dependency Injection**: Loose coupling throughout
- **Strategy Pattern**: Pluggable OCR/storage implementations
- **Middleware Pipeline**: Cross-cutting concerns
- **Factory Pattern**: Creating complex objects

### Code Quality Practices

- ✅ Comprehensive XML documentation comments
- ✅ Meaningful variable & method names
- ✅ DRY (Don't Repeat Yourself)
- ✅ YAGNI (You Aren't Gonna Need It)
- ✅ Proper exception handling
- ✅ Logging at appropriate levels
- ✅ Unit test coverage ≥80%
- ✅ Async/await for I/O operations
- ✅ Parameterized queries (SQL injection prevention)

---

**End of Document**

This comprehensive guide provides the complete blueprint for implementing IngredientIQ MVP using .NET 8, applying SOLID principles, clean architecture, and industry best practices.

For questions or clarifications, please contact: Abhishek Sinha

