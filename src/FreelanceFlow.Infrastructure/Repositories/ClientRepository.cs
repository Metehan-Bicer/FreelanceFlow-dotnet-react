using Microsoft.EntityFrameworkCore;
using FreelanceFlow.Domain.Entities;
using FreelanceFlow.Domain.Interfaces.Repositories;
using FreelanceFlow.Persistence.Context;

namespace FreelanceFlow.Infrastructure.Repositories;

public class ClientRepository : BaseRepository<Client>, IClientRepository
{
    public ClientRepository(FreelanceFlowDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Client>> GetActiveClientsAsync()
    {
        return await _dbSet
            .Where(c => c.Status == Domain.Enums.ClientStatus.Active && !c.IsDeleted)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Client?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Email == email && !c.IsDeleted);
    }

    public async Task<IEnumerable<Client>> SearchByNameAsync(string name)
    {
        return await _dbSet
            .Where(c => c.Name.Contains(name) && !c.IsDeleted)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Client>> GetClientsWithProjectsAsync()
    {
        return await _dbSet
            .Include(c => c.Projects)
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}