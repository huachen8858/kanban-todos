using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories;

public class TaskCommentRepository : ITaskCommentRepository
{
    private readonly TaskFlowDbContext _context;

    public TaskCommentRepository(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskCommentEntity>> GetByTaskAsync(int taskId)
        => await _context.TaskComments
            .Where(c => c.TaskId == taskId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

    public async Task<TaskCommentEntity?> GetByIdAsync(int id)
        => await _context.TaskComments.FindAsync(id);

    public async Task<TaskCommentEntity> CreateAsync(TaskCommentEntity comment)
    {
        _context.TaskComments.Add(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task DeleteAsync(TaskCommentEntity comment)
    {
        _context.TaskComments.Remove(comment);
        await _context.SaveChangesAsync();
    }
}
