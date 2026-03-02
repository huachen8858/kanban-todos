using Moq;
using TaskFlow.Application.Exceptions;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Services;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Tests;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _projectRepo = new();
    private readonly ProjectService _sut;

    public ProjectServiceTests()
    {
        _sut = new ProjectService(_projectRepo.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsProjectsForUser()
    {
        var projects = new List<ProjectEntity>
        {
            new() { Id = 1, Name = "A", OwnerId = 5 },
            new() { Id = 2, Name = "B", OwnerId = 5 },
        };
        _projectRepo.Setup(r => r.GetByOwnerAsync(5)).ReturnsAsync(projects);

        var result = await _sut.GetAllAsync(userId: 5);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetById_NotOwner_ThrowsForbiddenException()
    {
        _projectRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new ProjectEntity { Id = 1, OwnerId = 10, Name = "P" });

        await Assert.ThrowsAsync<ForbiddenException>(() => _sut.GetByIdAsync(1, userId: 99));
    }

    [Fact]
    public async Task GetById_ProjectNotFound_ThrowsNotFoundException()
    {
        _projectRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ProjectEntity?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByIdAsync(99, userId: 1));
    }

    [Fact]
    public async Task Create_Valid_SetsOwnerIdAndReturns()
    {
        _projectRepo.Setup(r => r.CreateAsync(It.IsAny<ProjectEntity>()))
            .ReturnsAsync((ProjectEntity p) => p);

        var result = await _sut.CreateAsync("My Project", "Desc", userId: 7);

        Assert.Equal("My Project", result.Name);
        Assert.Equal(7, result.OwnerId);
    }
}
