using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskFlowDbContext _context;

    public TaskRepository(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskItem>> GetByProjectAsync(int projectId)
        => await _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync();

    public async Task<TaskItem?> GetByIdAsync(int id)
        => await _context.Tasks.FindAsync(id);

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem> UpdateAsync(TaskItem task)
    {
        task.UpdatedAt = DateTime.UtcNow;
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task DeleteAsync(TaskItem task)
    {
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }
}
