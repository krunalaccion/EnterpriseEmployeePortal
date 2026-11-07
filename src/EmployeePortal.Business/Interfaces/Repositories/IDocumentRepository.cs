using EmployeePortal.Models.Entities;

namespace EmployeePortal.Business.Interfaces.Repositories;

public interface IDocumentRepository : IRepository<EmployeeDocument>
{
    Task<IReadOnlyList<EmployeeDocument>> GetDocumentsWithEmployeeAsync(int? employeeId = null, CancellationToken cancellationToken = default);
}

