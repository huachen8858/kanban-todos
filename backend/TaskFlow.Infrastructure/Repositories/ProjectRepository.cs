using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly TaskFlowDbContext _context;

    public ProjectRepository(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProjectEntity>> GetByOwnerAsync(int ownerId)
        => await _context.Projects
            .Where(p => p.OwnerId == ownerId)
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync();

    public async Task<ProjectEntity?> GetByIdAsync(int id)
        => await _context.Projects.FindAsync(id);

    public async Task<ProjectEntity> CreateAsync(ProjectEntity project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<ProjectEntity> UpdateAsync(ProjectEntity project)
    {
        project.UpdatedAt = DateTime.UtcNow;
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task DeleteAsync(ProjectEntity project)
    {
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
    }
}
