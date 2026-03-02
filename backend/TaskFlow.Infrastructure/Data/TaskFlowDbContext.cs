using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskStatus = TaskFlow.Domain.Enums.TaskStatus;
using TaskPriority = TaskFlow.Domain.Enums.TaskPriority;

namespace TaskFlow.Infrastructure.Data;

public class TaskFlowDbContext : DbContext
{
    public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<ProjectEntity> Projects => Set<ProjectEntity>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<TaskCommentEntity> TaskComments => Set<TaskCommentEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).HasMaxLength(255).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Name).HasMaxLength(100).IsRequired();
            entity.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
        });

        modelBuilder.Entity<ProjectEntity>(entity =>
        {
            entity.ToTable("projects");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).HasMaxLength(200).IsRequired();
            entity.HasOne(p => p.Owner)
                  .WithMany()
                  .HasForeignKey(p => p.OwnerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("tasks");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).HasMaxLength(300).IsRequired();
            entity.Property(t => t.Status)
                  .HasConversion<string>()
                  .HasDefaultValue(TaskStatus.Todo);
            entity.Property(t => t.Priority)
                  .HasConversion<string>()
                  .HasDefaultValue(TaskPriority.Medium);
            entity.HasOne(t => t.Project)
                  .WithMany(p => p.Tasks)
                  .HasForeignKey(t => t.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(t => t.Assignee)
                  .WithMany()
                  .HasForeignKey(t => t.AssigneeId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<TaskCommentEntity>(entity =>
        {
            entity.ToTable("task_comments");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Content).IsRequired();
            entity.HasOne(c => c.Task)
                  .WithMany(t => t.Comments)
                  .HasForeignKey(c => c.TaskId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(c => c.Author)
                  .WithMany()
                  .HasForeignKey(c => c.AuthorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
