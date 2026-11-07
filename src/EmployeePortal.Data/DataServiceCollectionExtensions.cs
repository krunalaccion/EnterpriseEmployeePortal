using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Data.Context;
using EmployeePortal.Data.Repositories;
using EmployeePortal.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeePortal.Data;

public static class DataServiceCollectionExtensions
{
    public static IServiceCollection AddPortalData(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        services.AddDbContext<PortalDbContext>((serviceProvider, options) =>
        {
            var runtimeConfig = serviceProvider.GetRequiredService<IConfiguration>();
            var provider = runtimeConfig[$"{DatabaseOptions.SectionName}:Provider"] ?? "SqlServer";
            var inMemoryDbName = runtimeConfig[$"{DatabaseOptions.SectionName}:InMemoryDatabaseName"] ?? "EnterpriseEmployeePortalDb";
            var connectionString = runtimeConfig.GetConnectionString("DefaultConnection")
                ?? "Server=(localdb)\\mssqllocaldb;Database=EnterpriseEmployeePortalDb;Trusted_Connection=True;TrustServerCertificate=True;";

            if (string.Equals(provider, "InMemory", StringComparison.OrdinalIgnoreCase))
            {
                options.UseInMemoryDatabase(inMemoryDbName);
            }
            else
            {
                options.UseSqlServer(connectionString);
            }
        });

        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IPayrollRepository, PayrollRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        services.AddScoped<ILeaveRepository, LeaveRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<INotificationAndSettingRepository, NotificationAndSettingRepository>();

        return services;
    }
}

