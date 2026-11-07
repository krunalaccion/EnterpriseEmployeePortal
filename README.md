# EnterpriseEmployeePortal (.NET 9 Security Benchmark Application)

> **SECURITY BENCHMARK NOTICE:**
> `EnterpriseEmployeePortal` is an intentionally vulnerable enterprise HR & Workforce Management test target created specifically for evaluating offline security scanners (`SecureScanAI`).
> - It uses **dummy test data and dummy credentials only**.
> - Do **not** deploy this application to production or expose it to untrusted networks.

---

## 1. Solution Architecture & Acyclic Dependency Graph

All projects target **.NET 9 (`net9.0`)**:

```text
EmployeePortal.Web ──────────────────────────────────────────────────┐
  │ (HTTP REST calls only via HttpClient)                            │
  ▼                                                                  ▼
EmployeePortal.API ───┬──► EmployeePortal.Data ──────┐               │
  (Composition Root)  │                              ▼               │
                      ├──► EmployeePortal.Business ──┼──► EmployeePortal.Models
                      │              │               │               ▲
                      │              ▼               │               │
                      └──► EmployeePortal.Integrations───────────────┘
```

| Project | Direct Project References | Responsibilities |
| :--- | :--- | :--- |
| **`EmployeePortal.Models`** | *(None)* | Entities, DTOs, Enums, Configuration option models. |
| **`EmployeePortal.Integrations`** | `Models` | File storage (`LocalFileStorageProvider`), `Newtonsoft.Json` state serializer, legacy ADO.NET SQL/XML payroll bridge (`Microsoft.Data.SqlClient 5.1.2`), SMTP client, `System.Drawing.Common` avatar helper. |
| **`EmployeePortal.Business`** | `Integrations`, `Models` | **Repository interfaces (`Interfaces/Repositories/I*Repository.cs`)**, service interfaces & implementations, `AutoMapper 9.0.0` mapping profile, `LegacyCryptoHelper`, `JwtTokenGenerator`, `InputValidationService`. |
| **`EmployeePortal.Data`** | `Business`, `Models` | `PortalDbContext`, EF Core entity configurations, `PortalDbSeeder`, and **repository implementations** implementing `Business` abstractions. |
| **`EmployeePortal.API`** | `Data`, `Business`, `Integrations`, `Models` | **Composition Root (`Program.cs`)** calling `AddPortalData()`, `AddPortalIntegrations()`, and `AddPortalBusiness()`; REST controllers, middlewares, CORS. |
| **`EmployeePortal.Web`** | `Models` | Blazor Interactive Server admin UI communicating with `EmployeePortal.API` exclusively via HTTP REST endpoints (`IPortalApiClient`). |
| **`EmployeePortal.UnitTests`** | `Data`, `Business`, `Integrations`, `Models` | xUnit unit tests for business services, crypto, serialization, and storage. |
| **`EmployeePortal.IntegrationTests`** | `API`, `Data`, `Business`, `Models` | `WebApplicationFactory<Program>` API integration tests and optional SQL Server relational security tests. |

---

## 2. Functional Modules (13 Modules)

1. **Dashboard** (`DashboardController`, `Dashboard.razor`)
2. **Employee Management** (`EmployeesController`, `EmployeeDirectory.razor`, `EmployeeDetail.razor`)
3. **Department Management** (`DepartmentsController`, `DepartmentDirectory.razor`)
4. **User and Role Management** (`UsersRolesController`, `UserRoleMatrix.razor`)
5. **Authentication and Authorization** (`AuthController`, `PortalJwtAuthenticationHandler`, `Login.razor`)
6. **Payroll** (`PayrollController`, `PayrollCenter.razor`)
7. **Attendance** (`AttendanceController`, `AttendanceLogPage.razor`)
8. **Leave Management** (`LeaveController`, `LeaveManagementPage.razor`)
9. **Document Management** (`DocumentsController`, `DocumentVaultPage.razor`)
10. **Reports and Analytics** (`ReportsController`, `ReportsAnalyticsPage.razor`)
11. **Audit Logs** (`AuditLogsController`, `AuditLogExplorerPage.razor`)
12. **Notifications** (`NotificationsController`, `NotificationCenterPage.razor`)
13. **Administration** (`AdministrationController`, `SystemAdminPage.razor`)

---

## 3. Database Configuration & Test Strategy

- **Primary Database (`SqlServer`)**: Configured in `appsettings.json` (`DatabaseOptions:Provider = "SqlServer"`).
- **Development & Integration Test Fallback (`InMemory`)**: Configured in `appsettings.Development.json` and `PortalApiWebApplicationFactory` (`DatabaseOptions:Provider = "InMemory"`) with automatic seed data (`PortalDbSeeder.SeedAsync`) so the application and default test suite run without requiring SQL Server or LocalDB.
- **Differences Between `InMemory` and `SqlServer`**:
  - Relational raw SQL methods (`FromSqlRaw` in `EmployeeRepository.SearchEmployeesRawSqlAsync`, `ReportRepository.ExecuteCustomReportRawSqlAsync`, and `SqlCommand` in `XmlPayrollExportBridge.ExportPayrollViaLegacyAdoSqlAsync`) require a relational SQL Server engine and are not supported by EF Core's `InMemory` provider.
  - Optional SQL Server relational tests in `SqlServerRelationalSecurityTests` use `[SqlServerFact]` and are **genuinely skipped** when the `PORTAL_SQLSERVER_TEST_CS` environment variable is absent.

---

## 4. Verified Vulnerable NuGet Dependencies (SCA Benchmark Cases)

| Package Name | Pinned Version | Affected Versions | Fixed Version | CVE / GHSA | Severity | CWE | Target Project | Technical & Applicability Notes |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **`Microsoft.Data.SqlClient`** | `5.1.2` | `>= 5.0.0, < 5.1.3` *(5.x line)* | **`5.1.3`** | **CVE-2024-0056** `GHSA-98g6-xh36-x2p7` | High (`8.7`) | `CWE-319` / `CWE-420` | `EmployeePortal.Integrations` | **Dependency Isolation:** `Microsoft.EntityFrameworkCore.SqlServer 9.0.22` requires `Microsoft.Data.SqlClient >= 5.1.9`. To avoid a `NU1605` package downgrade in `Data`, `Microsoft.Data.SqlClient 5.1.2` is isolated in `EmployeePortal.Integrations` (which does not reference EF Core). |
| **`System.Linq.Dynamic.Core`** | `1.2.18` | `>= 1.0.7.10, <= 1.2.25` | **`1.3.0`** | **CVE-2023-32571** `GHSA-w65q-jcmv-28gj` | Critical (`9.8`) | `CWE-94` / `CWE-697` | `EmployeePortal.Data` | Dynamic LINQ expression injection allowing arbitrary method invocation / RCE when untrusted filters are passed to `.Where()` / `.OrderBy()`. |
| **`Newtonsoft.Json`** | `12.0.3` | `< 13.0.1` | **`13.0.1`** | **CVE-2024-21907** `GHSA-5crp-9r3c-p9vr` | High (`7.5`) | `CWE-755` / `CWE-674` | `EmployeePortal.Integrations`, `EmployeePortal.API` | **Package SCA Case:** Uncontrolled recursion on deeply nested JSON causes `StackOverflowException` (DoS). Kept distinct from our application-level `CWE-502` `TypeNameHandling.All` code scenario. |
| **`System.Drawing.Common`** | `4.7.0` | `>= 4.0.0, < 4.7.2`; `>= 5.0.0, < 5.0.3` | **`4.7.2` / `5.0.3`** | **CVE-2021-24112** `GHSA-rxg9-xrhp-64gj` | Critical (`9.8`) | `CWE-94` *(NVD/GitHub Advisory)* | `EmployeePortal.Models` *(and transitive to `Web`)* | **Package SCA Case & Platform Condition:** Isolated in `EmployeePortal.Models` (since `Microsoft.Data.SqlClient 5.1.2` transitively pulls `System.Windows.Extensions 6.0.0 -> System.Drawing.Common >= 6.0.0` in `Integrations`). Runtime vulnerability affects **Linux and macOS environments only** (via `libgdiplus` metafile parsing). Windows is not vulnerable at runtime, and on `.NET 6+` (`net9.0`), `System.Drawing.Common` runtime GDI+ APIs are Windows-only. |
| **`System.IdentityModel.Tokens.Jwt`** & **`Microsoft.IdentityModel.JsonWebTokens`** | `6.33.0` | `>= 6.0.0, < 6.34.0` *(6.x line)* | **`6.34.0`** | **CVE-2024-21319** `GHSA-59j7-ghrg-fj52` | Moderate (`6.5`) | `CWE-20` / `CWE-400` | `EmployeePortal.Business` *(and transitive to `API`)* | **Package SCA Case & Applicability:** Disclosed against ASP.NET Core project templates with JWT/federated authentication and `Microsoft.IdentityModel.JsonWebTokens` where crafted tokens cause excessive memory allocation (DoS/OOM). Also resolves transitive `Microsoft.IdentityModel.Tokens 6.33.0`. |
| **`AutoMapper`** | `9.0.0` | `< 15.1.1` | **`15.1.1`** *(for `.NET 9`)* | **CVE-2026-32933** `GHSA-rvv3-g6hj-g44x` | High (`7.5`) | `CWE-674` | `EmployeePortal.Business` | Uncontrolled recursion when mapping circular/deep object graphs without `.MaxDepth()`. Registered directly without deprecated DI extension packages. |

---

## 5. Source-Code Security Test Scenarios (14 Categories)

| # | Category | CWE | Source File(s) | Scenario ID & Summary |
| :- | :--- | :--- | :--- | :--- |
| 1 | **SQL Injection** | `CWE-89` | `EmployeeRepository.cs`, `ReportRepository.cs`, `XmlPayrollExportBridge.cs` | `VULN-SQLI-01..03`: Unparameterized `FromSqlRaw` and ADO.NET `SqlCommand` string interpolation. |
| 2 | **Cross-Site Scripting (XSS)** | `CWE-79` | `EmployeeDetail.razor`, `NotificationCenterPage.razor` | `VULN-XSS-01..02`: Unencoded `@((MarkupString)html)` rendering of HR notes and broadcast announcements. |
| 3 | **Insecure Deserialization** | `CWE-502`, `CWE-611` | `LegacyJsonSerializer.cs`, `XmlPayrollExportBridge.cs` | `VULN-DESER-01..02`: `Newtonsoft.Json` `TypeNameHandling.All` and XML `DtdProcessing.Parse` with `XmlUrlResolver`. |
| 4 | **Hardcoded Dummy Credentials** | `CWE-798` | `appsettings.json`, `JwtTokenGenerator.cs`, `SmtpEmailService.cs` | `VULN-SECRET-01..02`: Hardcoded fallback JWT signing key, dummy SMTP API key, and dummy SQL credentials. |
| 5 | **Weak Cryptography** | `CWE-327`, `CWE-328` | `LegacyCryptoHelper.cs` | `VULN-CRYPTO-01`: Unsalted `MD5`/`SHA1` password hashing and `DES` encryption with static 8-byte key/IV. |
| 6 | **Missing Authorization** | `CWE-862` | `PayrollController.cs`, `AdministrationController.cs` | `VULN-AUTHZ-01..02`: `POST /api/payroll/export-bank-file` and `PUT /api/administration/settings` missing `[Authorize]`. |
| 7 | **IDOR** | `CWE-639` | `PayrollController.cs`, `DocumentsController.cs` | `VULN-IDOR-01..02`: `GET /api/payroll/payslips/{id}` and `GET /api/documents/{id}` lacking caller ownership checks. |
| 8 | **Insecure File Uploads** | `CWE-434` | `DocumentsController.cs` | `VULN-UPLOAD-01`: `POST /api/documents/upload` accepting arbitrary extensions without validation. |
| 9 | **Path Traversal** | `CWE-22` | `LocalFileStorageProvider.cs`, `DocumentsController.cs` | `VULN-PATH-01`: `GET /api/documents/download-raw?fileName=` joining raw user paths without canonical validation. |
| 10 | **Sensitive Info Exposure** | `CWE-200`, `CWE-209` | `ExceptionHandlingMiddleware.cs`, `EmployeeDtos.cs` | `VULN-INFO-01..02`: Full stack trace & host details in HTTP 500 JSON; `EncryptedTaxId` and bank numbers in directory DTOs. |
| 11 | **Insecure Logging** | `CWE-532` | `AuthAndUserService.cs`, `AuditRequestLoggingMiddleware.cs` | `VULN-LOG-01..02`: Logging plaintext failed login passwords and raw `Authorization` Bearer tokens. |
| 12 | **Weak Input Validation** | `CWE-20`, `CWE-1333`, `CWE-915` | `InputValidationService.cs`, `EmployeeDtos.cs` | `VULN-VAL-01..02`: Backtracking ReDoS regex without timeout and mass-assignment of `RoleOverride` / `BaseSalary`. |
| 13 | **Permissive CORS** | `CWE-942` | `EmployeePortal.API/Program.cs` | `VULN-CORS-01`: `SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials()`. |
| 14 | **Vulnerable NuGet Dependencies** | `CWE-1395` | `Data.csproj`, `Integrations.csproj`, `Business.csproj`, `API.csproj` | Verified CVEs/GHSAs listed in Section 4. |

