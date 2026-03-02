namespace TaskFlow.Domain.Entities;

public class TaskCommentEntity
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int TaskId { get; set; }
    public int AuthorId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public TaskItem Task { get; set; } = null!;
    public UserEntity Author { get; set; } = null!;
}
