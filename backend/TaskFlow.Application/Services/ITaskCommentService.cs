using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services;

public interface ITaskCommentService
{
    Task<IEnumerable<TaskCommentEntity>> GetByTaskAsync(int taskId, int userId);
    Task<TaskCommentEntity> CreateAsync(int taskId, string content, int userId);
    Task DeleteAsync(int id, int userId);
}
