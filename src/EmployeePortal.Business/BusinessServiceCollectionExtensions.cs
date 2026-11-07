using AutoMapper;
using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Business.Mapping;
using EmployeePortal.Business.Security;
using EmployeePortal.Business.Services;
using EmployeePortal.Business.Validation;
using EmployeePortal.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeePortal.Business;

public static class BusinessServiceCollectionExtensions
{
    public static IServiceCollection AddPortalBusiness(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // Direct registration of AutoMapper 9.0.0 without deprecated extension packages
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PortalMappingProfile>();
        });
        services.AddSingleton<IMapper>(mapperConfig.CreateMapper());

        services.AddSingleton<ICryptoHelper, LegacyCryptoHelper>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IInputValidationService, InputValidationService>();

        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IAuthAndUserService, AuthAndUserService>();
        services.AddScoped<IPayrollService, PayrollService>();
        services.AddScoped<IAttendanceAndLeaveService, AttendanceAndLeaveService>();
        services.AddScoped<IDocumentAndReportService, DocumentAndReportService>();
        services.AddScoped<IAdminAuditNotificationService, AdminAuditNotificationService>();

        return services;
    }
}

