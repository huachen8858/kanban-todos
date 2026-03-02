using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TaskFlowDbContext _context;

    public UserRepository(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<UserEntity?> GetByIdAsync(int id)
        => await _context.Users.FindAsync(id);

    public async Task<UserEntity?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<UserEntity> CreateAsync(UserEntity user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}
