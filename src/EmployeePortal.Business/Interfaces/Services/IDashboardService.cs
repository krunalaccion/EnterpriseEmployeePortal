using EmployeePortal.Models.DTOs;

namespace EmployeePortal.Business.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
}

