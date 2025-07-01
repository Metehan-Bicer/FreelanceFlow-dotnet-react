using FreelanceFlow.Domain.Entities;

namespace FreelanceFlow.Domain.Interfaces.Repositories;

public interface IClientRepository : IBaseRepository<Client>
{
    Task<IEnumerable<Client>> GetActiveClientsAsync();
    Task<Client?> GetByEmailAsync(string email);
    Task<IEnumerable<Client>> SearchByNameAsync(string name);
    Task<IEnumerable<Client>> GetClientsWithProjectsAsync();
    Task<IEnumerable<Project>> GetActiveProjectsByClientIdAsync(Guid clientId);
}