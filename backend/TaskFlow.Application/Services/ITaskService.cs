using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskItem>> GetByProjectAsync(int projectId, int userId);
    Task<TaskItem> GetByIdAsync(int id, int userId);
    Task<TaskItem> CreateAsync(int projectId, string title, string? description, string priority, DateOnly? dueDate, int? assigneeId, int userId);
    Task<TaskItem> UpdateAsync(int id, string title, string? description, string priority, DateOnly? dueDate, int? assigneeId, int userId);
    Task<TaskItem> UpdateStatusAsync(int id, string status, int userId);
    Task DeleteAsync(int id, int userId);
}
