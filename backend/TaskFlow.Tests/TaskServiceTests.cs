using Moq;
using TaskFlow.Application.Exceptions;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Services;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Tests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepo = new();
    private readonly Mock<IProjectRepository> _projectRepo = new();
    private readonly TaskService _sut;

    public TaskServiceTests()
    {
        _sut = new TaskService(_taskRepo.Object, _projectRepo.Object);
    }

    [Fact]
    public async Task GetByProject_ProjectNotFound_ThrowsNotFoundException()
    {
        _projectRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ProjectEntity?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByProjectAsync(1, userId: 1));
    }

    [Fact]
    public async Task GetByProject_UserNotOwner_ThrowsForbiddenException()
    {
        _projectRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new ProjectEntity { Id = 1, OwnerId = 42, Name = "P" });

        await Assert.ThrowsAsync<ForbiddenException>(() => _sut.GetByProjectAsync(1, userId: 99));
    }

    [Fact]
    public async Task GetByProject_Valid_ReturnsTasks()
    {
        var project = new ProjectEntity { Id = 1, OwnerId = 1, Name = "P" };
        var tasks = new List<TaskItem>
        {
            new() { Id = 1, Title = "T1", ProjectId = 1 },
            new() { Id = 2, Title = "T2", ProjectId = 1 },
        };
        _projectRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(project);
        _taskRepo.Setup(r => r.GetByProjectAsync(1)).ReturnsAsync(tasks);

        var result = await _sut.GetByProjectAsync(1, userId: 1);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Create_UserNotOwner_ThrowsForbiddenException()
    {
        _projectRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new ProjectEntity { Id = 1, OwnerId = 42, Name = "P" });

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _sut.CreateAsync(1, "Task", null, "Medium", null, null, userId: 99));
    }

    [Fact]
    public async Task Create_Valid_ReturnsCreatedTask()
    {
        var project = new ProjectEntity { Id = 1, OwnerId = 1, Name = "P" };
        _projectRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(project);
        _taskRepo.Setup(r => r.CreateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync((TaskItem t) => t);

        var result = await _sut.CreateAsync(1, "New Task", null, "High", null, null, userId: 1);

        Assert.Equal("New Task", result.Title);
        Assert.Equal(TaskFlow.Domain.Enums.TaskPriority.High, result.Priority);
    }

    [Fact]
    public async Task Delete_WhenUserNotOwner_ThrowsForbiddenException()
    {
        var task = new TaskItem { Id = 1, Title = "T", ProjectId = 1 };
        var project = new ProjectEntity { Id = 1, OwnerId = 42, Name = "P" };
        _taskRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
        _projectRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(project);

        await Assert.ThrowsAsync<ForbiddenException>(() => _sut.DeleteAsync(1, userId: 99));
    }
}
