using EmployeePortal.Business.Security;
using EmployeePortal.Models.Entities;
using EmployeePortal.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Data.Context;

public static class PortalDbSeeder
{
    private static readonly SemaphoreSlim SeedLock = new(1, 1);

    public static async Task SeedAsync(PortalDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await SeedLock.WaitAsync(cancellationToken);
        try
        {
            if (await dbContext.Departments.AnyAsync(cancellationToken))
            {
                return;
            }

            await SeedInternalAsync(dbContext, cancellationToken);
        }
        finally
        {
            SeedLock.Release();
        }
    }

    private static async Task SeedInternalAsync(PortalDbContext dbContext, CancellationToken cancellationToken)
    {

        var crypto = new LegacyCryptoHelper();

        var departments = new List<Department>
        {
            new() { Id = 1, Name = "Platform Engineering", Code = "ENG-PLAT", CostCenter = "CC-1010", Location = "Austin, TX", AnnualBudget = 4250000m, IsActive = true },
            new() { Id = 2, Name = "People & Culture (HR)", Code = "HR-CORP", CostCenter = "CC-2020", Location = "New York, NY", AnnualBudget = 1350000m, IsActive = true },
            new() { Id = 3, Name = "Corporate Finance & Payroll", Code = "FIN-PAY", CostCenter = "CC-3030", Location = "Chicago, IL", AnnualBudget = 1980000m, IsActive = true },
            new() { Id = 4, Name = "Cybersecurity & Compliance", Code = "SEC-OPS", CostCenter = "CC-4040", Location = "Reston, VA", AnnualBudget = 2750000m, IsActive = true },
            new() { Id = 5, Name = "Enterprise Operations", Code = "OPS-GLB", CostCenter = "CC-5050", Location = "Seattle, WA", AnnualBudget = 1620000m, IsActive = true }
        };
        await dbContext.Departments.AddRangeAsync(departments, cancellationToken);

        var employees = new List<Employee>
        {
            new()
            {
                Id = 1,
                EmployeeNumber = "EMP-1001",
                FirstName = "Elena",
                LastName = "Vance",
                Email = "elena.vance@dummy-hr-portal.local",
                Phone = "555-0101",
                JobTitle = "VP of Engineering",
                Status = EmploymentStatus.Active,
                HireDate = new DateTime(2020, 3, 15),
                BaseSalary = 195000m,
                EncryptedTaxId = crypto.EncryptTaxIdWithStaticDes("999-01-1001"),
                BankRoutingNumber = "021000021",
                BankAccountNumber = "880011223301",
                HrNotesHtml = "<p>Executive sponsor for <strong>Platform Modernization</strong>. Cleared for Tier-1 systems.</p>",
                DepartmentId = 1
            },
            new()
            {
                Id = 2,
                EmployeeNumber = "EMP-1002",
                FirstName = "Marcus",
                LastName = "Sterling",
                Email = "marcus.sterling@dummy-hr-portal.local",
                Phone = "555-0102",
                JobTitle = "Director of Human Resources",
                Status = EmploymentStatus.Active,
                HireDate = new DateTime(2019, 7, 1),
                BaseSalary = 162000m,
                EncryptedTaxId = crypto.EncryptTaxIdWithStaticDes("999-01-1002"),
                BankRoutingNumber = "021000021",
                BankAccountNumber = "880011223302",
                HrNotesHtml = "<p>Primary administrator for workforce policies and annual compensation review.</p>",
                DepartmentId = 2
            },
            new()
            {
                Id = 3,
                EmployeeNumber = "EMP-1003",
                FirstName = "Priya",
                LastName = "Natarajan",
                Email = "priya.natarajan@dummy-hr-portal.local",
                Phone = "555-0103",
                JobTitle = "Principal Security Architect",
                Status = EmploymentStatus.Active,
                HireDate = new DateTime(2021, 1, 11),
                BaseSalary = 178000m,
                EncryptedTaxId = crypto.EncryptTaxIdWithStaticDes("999-01-1003"),
                BankRoutingNumber = "111000025",
                BankAccountNumber = "880011223303",
                HrNotesHtml = "<span class='badge'>Red Team Lead</span> Assigned to internal audit &amp; compliance.",
                DepartmentId = 4,
                ManagerId = 1
            },
            new()
            {
                Id = 4,
                EmployeeNumber = "EMP-1004",
                FirstName = "David",
                LastName = "Kowalski",
                Email = "david.kowalski@dummy-hr-portal.local",
                Phone = "555-0104",
                JobTitle = "Senior Payroll Specialist",
                Status = EmploymentStatus.Active,
                HireDate = new DateTime(2022, 5, 23),
                BaseSalary = 118000m,
                EncryptedTaxId = crypto.EncryptTaxIdWithStaticDes("999-01-1004"),
                BankRoutingNumber = "071000013",
                BankAccountNumber = "880011223304",
                HrNotesHtml = "<p>Authorized ACH batch operator for US payroll runs.</p>",
                DepartmentId = 3,
                ManagerId = 2
            },
            new()
            {
                Id = 5,
                EmployeeNumber = "EMP-1005",
                FirstName = "Sophia",
                LastName = "Alvarez",
                Email = "sophia.alvarez@dummy-hr-portal.local",
                Phone = "555-0105",
                JobTitle = "Senior Cloud Software Engineer",
                Status = EmploymentStatus.OnLeave,
                HireDate = new DateTime(2023, 2, 6),
                BaseSalary = 148000m,
                EncryptedTaxId = crypto.EncryptTaxIdWithStaticDes("999-01-1005"),
                BankRoutingNumber = "121000248",
                BankAccountNumber = "880011223305",
                HrNotesHtml = "<p>High performer (Exceeds Expectations Q3/Q4).</p>",
                DepartmentId = 1,
                ManagerId = 1
            },
            new()
            {
                Id = 6,
                EmployeeNumber = "EMP-1006",
                FirstName = "Liam",
                LastName = "O'Connor",
                Email = "liam.oconnor@dummy-hr-portal.local",
                Phone = "555-0106",
                JobTitle = "Operations Coordinator",
                Status = EmploymentStatus.Probation,
                HireDate = new DateTime(2025, 9, 1),
                BaseSalary = 86000m,
                EncryptedTaxId = crypto.EncryptTaxIdWithStaticDes("999-01-1006"),
                BankRoutingNumber = "122000247",
                BankAccountNumber = "880011223306",
                HrNotesHtml = "<p>90-day onboarding review scheduled next month.</p>",
                DepartmentId = 5,
                ManagerId = 2
            }
        };
        await dbContext.Employees.AddRangeAsync(employees, cancellationToken);

        var roles = new List<Role>
        {
            new() { Id = 1, Name = "Admin", Description = "Full system and security administration privileges", PermissionsCsv = "All,Admin,Payroll,HR,Users", IsSystemRole = true },
            new() { Id = 2, Name = "HRManager", Description = "Manage workforce records, departments, leave and documents", PermissionsCsv = "HR,Employees,Leave,Documents,Reports", IsSystemRole = true },
            new() { Id = 3, Name = "PayrollSpecialist", Description = "Process payroll runs and export ACH bank files", PermissionsCsv = "Payroll,Reports", IsSystemRole = true },
            new() { Id = 4, Name = "Employee", Description = "Standard self-service portal access", PermissionsCsv = "SelfService,Leave,Attendance", IsSystemRole = true }
        };
        await dbContext.Roles.AddRangeAsync(roles, cancellationToken);

        // Dummy accounts using unsalted MD5 hashes of dummy test passwords
        var users = new List<ApplicationUser>
        {
            new()
            {
                Id = 1,
                Username = "admin",
                Email = "elena.vance@dummy-hr-portal.local",
                PasswordHash = crypto.ComputeUnsaltedMd5Hash("Admin@123!"),
                PasswordHashAlgorithm = "MD5-Legacy",
                IsActive = true,
                EmployeeId = 1,
                LastLoginUtc = DateTime.UtcNow.AddHours(-2)
            },
            new()
            {
                Id = 2,
                Username = "hr.marcus",
                Email = "marcus.sterling@dummy-hr-portal.local",
                PasswordHash = crypto.ComputeUnsaltedMd5Hash("HrManager@123!"),
                PasswordHashAlgorithm = "MD5-Legacy",
                IsActive = true,
                EmployeeId = 2,
                LastLoginUtc = DateTime.UtcNow.AddHours(-5)
            },
            new()
            {
                Id = 3,
                Username = "payroll.david",
                Email = "david.kowalski@dummy-hr-portal.local",
                PasswordHash = crypto.ComputeUnsaltedMd5Hash("Payroll@123!"),
                PasswordHashAlgorithm = "MD5-Legacy",
                IsActive = true,
                EmployeeId = 4,
                LastLoginUtc = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                Id = 4,
                Username = "sophia.dev",
                Email = "sophia.alvarez@dummy-hr-portal.local",
                PasswordHash = crypto.ComputeUnsaltedMd5Hash("Employee@123!"),
                PasswordHashAlgorithm = "MD5-Legacy",
                IsActive = true,
                EmployeeId = 5,
                LastLoginUtc = DateTime.UtcNow.AddDays(-3)
            }
        };
        await dbContext.Users.AddRangeAsync(users, cancellationToken);

        var userRoles = new List<UserRole>
        {
            new() { Id = 1, UserId = 1, RoleId = 1 },
            new() { Id = 2, UserId = 2, RoleId = 2 },
            new() { Id = 3, UserId = 3, RoleId = 3 },
            new() { Id = 4, UserId = 4, RoleId = 4 }
        };
        await dbContext.UserRoles.AddRangeAsync(userRoles, cancellationToken);

        var payrolls = new List<PayrollRecord>
        {
            new() { Id = 1, EmployeeId = 1, PayPeriodStart = new DateTime(2026, 10, 1), PayPeriodEnd = new DateTime(2026, 10, 15), GrossSalary = 7500m, FederalTaxWithheld = 1350m, StateTaxWithheld = 375m, BenefitsDeduction = 185m, BonusAmount = 500m, NetPay = 6090m, PaymentStatus = "Processed", DirectDepositReference = "ACH-20261015-EMP1001" },
            new() { Id = 2, EmployeeId = 2, PayPeriodStart = new DateTime(2026, 10, 1), PayPeriodEnd = new DateTime(2026, 10, 15), GrossSalary = 6230.77m, FederalTaxWithheld = 1121.54m, StateTaxWithheld = 311.54m, BenefitsDeduction = 185m, BonusAmount = 0m, NetPay = 4612.69m, PaymentStatus = "Processed", DirectDepositReference = "ACH-20261015-EMP1002" },
            new() { Id = 3, EmployeeId = 3, PayPeriodStart = new DateTime(2026, 10, 1), PayPeriodEnd = new DateTime(2026, 10, 15), GrossSalary = 6846.15m, FederalTaxWithheld = 1232.31m, StateTaxWithheld = 342.31m, BenefitsDeduction = 185m, BonusAmount = 250m, NetPay = 5336.53m, PaymentStatus = "Processed", DirectDepositReference = "ACH-20261015-EMP1003" },
            new() { Id = 4, EmployeeId = 4, PayPeriodStart = new DateTime(2026, 10, 1), PayPeriodEnd = new DateTime(2026, 10, 15), GrossSalary = 4538.46m, FederalTaxWithheld = 816.92m, StateTaxWithheld = 226.92m, BenefitsDeduction = 185m, BonusAmount = 0m, NetPay = 3309.62m, PaymentStatus = "Processed", DirectDepositReference = "ACH-20261015-EMP1004" }
        };
        await dbContext.PayrollRecords.AddRangeAsync(payrolls, cancellationToken);

        var attendance = new List<AttendanceRecord>
        {
            new() { Id = 1, EmployeeId = 1, WorkDate = DateTime.UtcNow.Date.AddDays(-1), ClockInUtc = DateTime.UtcNow.Date.AddDays(-1).AddHours(8), ClockOutUtc = DateTime.UtcNow.Date.AddDays(-1).AddHours(17), RegularHours = 8m, OvertimeHours = 1m, AttendanceStatus = "Present", SourceIpAddress = "10.20.1.14", ShiftNotes = "Architecture review board" },
            new() { Id = 2, EmployeeId = 2, WorkDate = DateTime.UtcNow.Date.AddDays(-1), ClockInUtc = DateTime.UtcNow.Date.AddDays(-1).AddHours(9), ClockOutUtc = DateTime.UtcNow.Date.AddDays(-1).AddHours(17), RegularHours = 8m, OvertimeHours = 0m, AttendanceStatus = "Present", SourceIpAddress = "10.20.2.41", ShiftNotes = "Benefits enrollment sync" },
            new() { Id = 3, EmployeeId = 3, WorkDate = DateTime.UtcNow.Date.AddDays(-1), ClockInUtc = DateTime.UtcNow.Date.AddDays(-1).AddHours(8), ClockOutUtc = DateTime.UtcNow.Date.AddDays(-1).AddHours(18), RegularHours = 8m, OvertimeHours = 2m, AttendanceStatus = "Present", SourceIpAddress = "10.20.4.19", ShiftNotes = "Security scanner validation" }
        };
        await dbContext.AttendanceRecords.AddRangeAsync(attendance, cancellationToken);

        var leaves = new List<LeaveRequest>
        {
            new() { Id = 1, EmployeeId = 5, LeaveType = LeaveType.AnnualVacation, StartDate = DateTime.UtcNow.Date.AddDays(3), EndDate = DateTime.UtcNow.Date.AddDays(7), TotalDays = 5m, Reason = "Family autumn trip", Status = LeaveApprovalStatus.Pending },
            new() { Id = 2, EmployeeId = 3, LeaveType = LeaveType.StudyLeave, StartDate = DateTime.UtcNow.Date.AddDays(14), EndDate = DateTime.UtcNow.Date.AddDays(16), TotalDays = 3m, Reason = "Security conference attendance", Status = LeaveApprovalStatus.Approved, ReviewerComments = "Approved by VP Engineering", ReviewedAtUtc = DateTime.UtcNow.AddDays(-2) },
            new() { Id = 3, EmployeeId = 6, LeaveType = LeaveType.SickLeave, StartDate = DateTime.UtcNow.Date.AddDays(-4), EndDate = DateTime.UtcNow.Date.AddDays(-3), TotalDays = 2m, Reason = "Medical appointment", Status = LeaveApprovalStatus.Approved, ReviewerComments = "Verified", ReviewedAtUtc = DateTime.UtcNow.AddDays(-3) }
        };
        await dbContext.LeaveRequests.AddRangeAsync(leaves, cancellationToken);

        var documents = new List<EmployeeDocument>
        {
            new() { Id = 1, EmployeeId = 1, Title = "Executive Employment Agreement 2026", Category = DocumentCategory.EmploymentContract, FileName = "emp1001_contract.txt", ContentType = "text/plain", FileSizeBytes = 1024, StorageRelativePath = "emp1001_contract.txt", IsConfidential = true, UploadedByUsername = "admin" },
            new() { Id = 2, EmployeeId = 4, Title = "W-4 Federal Tax Withholding Certificate", Category = DocumentCategory.IdentityAndTax, FileName = "emp1004_w4_tax.txt", ContentType = "text/plain", FileSizeBytes = 768, StorageRelativePath = "emp1004_w4_tax.txt", IsConfidential = true, UploadedByUsername = "payroll.david" },
            new() { Id = 3, EmployeeId = 2, Title = "Enterprise Remote Work Policy v4", Category = DocumentCategory.GeneralPolicy, FileName = "remote_work_policy_v4.txt", ContentType = "text/plain", FileSizeBytes = 2048, StorageRelativePath = "remote_work_policy_v4.txt", IsConfidential = false, UploadedByUsername = "hr.marcus" }
        };
        await dbContext.EmployeeDocuments.AddRangeAsync(documents, cancellationToken);

        var reportTemplates = new List<ReportTemplate>
        {
            new() { Id = 1, Name = "High Compensation Band (> $150k)", Category = "Compensation", Description = "Lists employees with base salary exceeding $150,000", DefaultFilterExpression = "BaseSalary > 150000", DefaultSortColumn = "BaseSalary descending", IsPublic = true },
            new() { Id = 2, Name = "Platform Engineering Roster", Category = "Headcount", Description = "Active employees in Department #1", DefaultFilterExpression = "DepartmentId == 1", DefaultSortColumn = "LastName ascending", IsPublic = true },
            new() { Id = 3, Name = "Probation & Leave Status Audit", Category = "Compliance", Description = "Non-active standard status employees", DefaultFilterExpression = "BaseSalary >= 80000", DefaultSortColumn = "HireDate descending", IsPublic = true }
        };
        await dbContext.ReportTemplates.AddRangeAsync(reportTemplates, cancellationToken);

        var notifications = new List<Notification>
        {
            new() { Id = 1, Title = "Q4 Open Enrollment Window Now Active", MessageHtml = "<div>Please review your <strong>2026 Healthcare &amp; Benefits</strong> selections in the Document Vault by Friday.</div>", Category = "Benefits", IsBroadcast = true, IsRead = false },
            new() { Id = 2, Title = "Payroll Direct Deposit Cutoff Reminder", MessageHtml = "<div>All bonus adjustments for the current pay cycle must be submitted before <em>17:00 UTC</em>.</div>", Category = "Payroll", IsBroadcast = true, IsRead = false }
        };
        await dbContext.Notifications.AddRangeAsync(notifications, cancellationToken);

        var settings = new List<SystemSetting>
        {
            new() { Id = 1, SettingKey = "Portal.EnvironmentBanner", SettingValue = "Internal HR & Workforce Sandbox", GroupName = "General", Description = "Header environment label", IsSensitive = false },
            new() { Id = 2, SettingKey = "Payroll.AchBatchServiceKey", SettingValue = "DUMMY-ACH-KEY-99481-SCANNER-TEST", GroupName = "Payroll", Description = "Dummy ACH integration key for scanner benchmarking", IsSensitive = true },
            new() { Id = 3, SettingKey = "Security.AllowLegacyMd5Auth", SettingValue = "true", GroupName = "Security", Description = "Enables legacy unsalted MD5 password verification", IsSensitive = false }
        };
        await dbContext.SystemSettings.AddRangeAsync(settings, cancellationToken);

        var auditLogs = new List<AuditLog>
        {
            new() { Id = 1, TimestampUtc = DateTime.UtcNow.AddHours(-6), ActorUsername = "system", ActionName = "DatabaseSeedCompleted", ModuleName = "Administration", TargetEntityId = "ALL", DetailsJson = "{\"seededEmployees\":6,\"seededDepartments\":5}", Severity = AuditSeverity.Information },
            new() { Id = 2, TimestampUtc = DateTime.UtcNow.AddHours(-3), ActorUsername = "payroll.david", ActionName = "ExportAchBatch", ModuleName = "Payroll", TargetEntityId = "BATCH-2026-10", DetailsJson = "{\"records\":4,\"totalNet\":19348.84}", Severity = AuditSeverity.Warning },
            new() { Id = 3, TimestampUtc = DateTime.UtcNow.AddHours(-1), ActorUsername = "admin", ActionName = "SecurityConfigModified", ModuleName = "Administration", TargetEntityId = "Security.AllowLegacyMd5Auth", DetailsJson = "{\"oldValue\":\"false\",\"newValue\":\"true\"}", Severity = AuditSeverity.SecurityAlert }
        };
        await dbContext.AuditLogs.AddRangeAsync(auditLogs, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

