using TaskFlow.Application.Exceptions;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services;

public class TaskCommentService : ITaskCommentService
{
    private readonly ITaskCommentRepository _commentRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;

    public TaskCommentService(
        ITaskCommentRepository commentRepository,
        ITaskRepository taskRepository,
        IProjectRepository projectRepository)
    {
        _commentRepository = commentRepository;
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<TaskCommentEntity>> GetByTaskAsync(int taskId, int userId)
    {
        await VerifyTaskAccessAsync(taskId, userId);
        return await _commentRepository.GetByTaskAsync(taskId);
    }

    public async Task<TaskCommentEntity> CreateAsync(int taskId, string content, int userId)
    {
        await VerifyTaskAccessAsync(taskId, userId);

        var comment = new TaskCommentEntity
        {
            TaskId = taskId,
            Content = content,
            AuthorId = userId
        };

        return await _commentRepository.CreateAsync(comment);
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var comment = await _commentRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(TaskCommentEntity), id);

        if (comment.AuthorId != userId)
            throw new ForbiddenException();

        await _commentRepository.DeleteAsync(comment);
    }

    private async System.Threading.Tasks.Task VerifyTaskAccessAsync(int taskId, int userId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId)
            ?? throw new NotFoundException(nameof(TaskItem), taskId);

        var project = await _projectRepository.GetByIdAsync(task.ProjectId)
            ?? throw new NotFoundException(nameof(ProjectEntity), task.ProjectId);

        if (project.OwnerId != userId)
            throw new ForbiddenException();
    }
}
