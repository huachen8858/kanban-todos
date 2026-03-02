using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces;

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetByProjectAsync(int projectId);
    Task<TaskItem?> GetByIdAsync(int id);
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<TaskItem> UpdateAsync(TaskItem task);
    Task DeleteAsync(TaskItem task);
}
