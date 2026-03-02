namespace TaskFlow.Domain.Entities;

public class ProjectEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OwnerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public UserEntity Owner { get; set; } = null!;
    public ICollection<TaskItem> Tasks { get; set; } = [];
}
