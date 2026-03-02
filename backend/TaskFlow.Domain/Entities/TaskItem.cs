using TaskStatus = TaskFlow.Domain.Enums.TaskStatus;
using TaskPriority = TaskFlow.Domain.Enums.TaskPriority;

namespace TaskFlow.Domain.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateOnly? DueDate { get; set; }
    public int ProjectId { get; set; }
    public int? AssigneeId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ProjectEntity Project { get; set; } = null!;
    public UserEntity? Assignee { get; set; }
    public ICollection<TaskCommentEntity> Comments { get; set; } = [];
}
