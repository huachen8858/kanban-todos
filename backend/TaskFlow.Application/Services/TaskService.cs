using TaskFlow.Application.Exceptions;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskStatus = TaskFlow.Domain.Enums.TaskStatus;
using TaskPriority = TaskFlow.Domain.Enums.TaskPriority;

namespace TaskFlow.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;

    public TaskService(ITaskRepository taskRepository, IProjectRepository projectRepository)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<TaskItem>> GetByProjectAsync(int projectId, int userId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId)
            ?? throw new NotFoundException(nameof(ProjectEntity), projectId);

        if (project.OwnerId != userId)
            throw new ForbiddenException();

        return await _taskRepository.GetByProjectAsync(projectId);
    }

    public async Task<TaskItem> GetByIdAsync(int id, int userId)
    {
        var task = await _taskRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(TaskItem), id);

        var project = await _projectRepository.GetByIdAsync(task.ProjectId)
            ?? throw new NotFoundException(nameof(ProjectEntity), task.ProjectId);

        if (project.OwnerId != userId)
            throw new ForbiddenException();

        return task;
    }

    public async Task<TaskItem> CreateAsync(int projectId, string title, string? description,
        string priority, DateOnly? dueDate, int? assigneeId, int userId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId)
            ?? throw new NotFoundException(nameof(ProjectEntity), projectId);

        if (project.OwnerId != userId)
            throw new ForbiddenException();

        var task = new TaskItem
        {
            Title = title,
            Description = description,
            Priority = Enum.Parse<TaskPriority>(priority),
            DueDate = dueDate,
            ProjectId = projectId,
            AssigneeId = assigneeId
        };

        return await _taskRepository.CreateAsync(task);
    }

    public async Task<TaskItem> UpdateAsync(int id, string title, string? description,
        string priority, DateOnly? dueDate, int? assigneeId, int userId)
    {
        var task = await GetByIdAsync(id, userId);

        task.Title = title;
        task.Description = description;
        task.Priority = Enum.Parse<TaskPriority>(priority);
        task.DueDate = dueDate;
        task.AssigneeId = assigneeId;

        return await _taskRepository.UpdateAsync(task);
    }

    public async Task<TaskItem> UpdateStatusAsync(int id, string status, int userId)
    {
        var task = await GetByIdAsync(id, userId);
        task.Status = Enum.Parse<TaskStatus>(status);
        return await _taskRepository.UpdateAsync(task);
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var task = await GetByIdAsync(id, userId);
        await _taskRepository.DeleteAsync(task);
    }
}
