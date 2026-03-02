using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces;

public interface ITaskCommentRepository
{
    Task<IEnumerable<TaskCommentEntity>> GetByTaskAsync(int taskId);
    Task<TaskCommentEntity?> GetByIdAsync(int id);
    Task<TaskCommentEntity> CreateAsync(TaskCommentEntity comment);
    Task DeleteAsync(TaskCommentEntity comment);
}
