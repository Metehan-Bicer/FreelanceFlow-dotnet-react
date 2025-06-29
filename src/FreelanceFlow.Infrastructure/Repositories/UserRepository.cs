using Microsoft.EntityFrameworkCore;
using FreelanceFlow.Domain.Entities;
using FreelanceFlow.Domain.Interfaces.Repositories;
using FreelanceFlow.Persistence.Context;

namespace FreelanceFlow.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(FreelanceFlowDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
    }

    public async Task<bool> IsUsernameExistsAsync(string username)
    {
        return await _context.Set<User>()
            .AnyAsync(u => u.Username == username && !u.IsDeleted);
    }

    public async Task<bool> IsEmailExistsAsync(string email)
    {
        return await _context.Set<User>()
            .AnyAsync(u => u.Email == email && !u.IsDeleted);
    }
}