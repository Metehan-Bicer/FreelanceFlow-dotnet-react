using Microsoft.EntityFrameworkCore;
using FreelanceFlow.Domain.Entities;
using FreelanceFlow.Domain.Interfaces.Repositories;
using FreelanceFlow.Domain.Enums;
using FreelanceFlow.Persistence.Context;

namespace FreelanceFlow.Infrastructure.Repositories;

public class ProjectRepository : BaseRepository<Project>, IProjectRepository
{
    public ProjectRepository(FreelanceFlowDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Project>> GetProjectsByClientAsync(Guid clientId)
    {
        return await _dbSet
            .Include(p => p.Client)
            .Include(p => p.Tasks)
            .Where(p => p.ClientId == clientId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetProjectsByStatusAsync(ProjectStatus status)
    {
        return await _dbSet
            .Include(p => p.Client)
            .Where(p => p.Status == status && !p.IsDeleted)
            .OrderBy(p => p.DeadlineDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetActiveProjectsAsync()
    {
        return await _dbSet
            .Include(p => p.Client)
            .Include(p => p.Tasks)
            .Where(p => p.Status == ProjectStatus.InProgress && !p.IsDeleted)
            .OrderBy(p => p.DeadlineDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetOverdueProjectsAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet
            .Include(p => p.Client)
            .Where(p => p.DeadlineDate.HasValue && 
                       p.DeadlineDate.Value.Date < today && 
                       p.Status != ProjectStatus.Completed && 
                       p.Status != ProjectStatus.Cancelled && 
                       !p.IsDeleted)
            .OrderBy(p => p.DeadlineDate)
            .ToListAsync();
    }

    public async Task<Project?> GetProjectWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.Client)
            .Include(p => p.Tasks)
            .Include(p => p.TimeEntries)
            .Include(p => p.Invoices)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<decimal> GetTotalBudgetByClientAsync(Guid clientId)
    {
        return await _dbSet
            .Where(p => p.ClientId == clientId && !p.IsDeleted)
            .SumAsync(p => p.Budget);
    }

    public async Task<IEnumerable<Project>> GetAllWithClientsAsync()
    {
        return await _dbSet
            .Include(p => p.Client)
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Project?> GetByIdWithClientAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.Client)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<IEnumerable<Project>> GetByClientIdAsync(Guid clientId)
    {
        return await _dbSet
            .Include(p => p.Client)
            .Where(p => p.ClientId == clientId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}