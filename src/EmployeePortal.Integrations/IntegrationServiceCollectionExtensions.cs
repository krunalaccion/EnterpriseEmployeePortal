using EmployeePortal.Integrations.Imaging;
using EmployeePortal.Integrations.Messaging;
using EmployeePortal.Integrations.Payroll;
using EmployeePortal.Integrations.Serialization;
using EmployeePortal.Integrations.Storage;
using EmployeePortal.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeePortal.Integrations;

public static class IntegrationServiceCollectionExtensions
{
    public static IServiceCollection AddPortalIntegrations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<IntegrationSettings>(configuration.GetSection(IntegrationSettings.SectionName));

        services.AddSingleton<IDocumentStorageProvider, LocalFileStorageProvider>();
        services.AddSingleton<IStateSerializer, LegacyJsonSerializer>();
        services.AddScoped<IPayrollExportBridge, XmlPayrollExportBridge>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddSingleton<IAvatarProcessor, BadgeAvatarProcessor>();

        return services;
    }
}

