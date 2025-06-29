using FreelanceFlow.Domain.Entities;
using FreelanceFlow.Domain.Enums;

namespace FreelanceFlow.Domain.Interfaces.Repositories;

public interface IProjectRepository : IBaseRepository<Project>
{
    Task<IEnumerable<Project>> GetProjectsByClientAsync(Guid clientId);
    Task<IEnumerable<Project>> GetProjectsByStatusAsync(ProjectStatus status);
    Task<IEnumerable<Project>> GetActiveProjectsAsync();
    Task<IEnumerable<Project>> GetOverdueProjectsAsync();
    Task<Project?> GetProjectWithDetailsAsync(Guid id);
    Task<decimal> GetTotalBudgetByClientAsync(Guid clientId);
    Task<IEnumerable<Project>> GetAllWithClientsAsync();
    Task<Project?> GetByIdWithClientAsync(Guid id);
    Task<IEnumerable<Project>> GetByClientIdAsync(Guid clientId);
}