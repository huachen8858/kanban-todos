using TaskFlow.Application.Exceptions;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<ProjectEntity>> GetAllAsync(int userId)
        => await _projectRepository.GetByOwnerAsync(userId);

    public async Task<ProjectEntity> GetByIdAsync(int id, int userId)
    {
        var project = await _projectRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(ProjectEntity), id);

        if (project.OwnerId != userId)
            throw new ForbiddenException();

        return project;
    }

    public async Task<ProjectEntity> CreateAsync(string name, string? description, int userId)
    {
        var project = new ProjectEntity
        {
            Name = name,
            Description = description,
            OwnerId = userId
        };
        return await _projectRepository.CreateAsync(project);
    }

    public async Task<ProjectEntity> UpdateAsync(int id, string name, string? description, int userId)
    {
        var project = await _projectRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(ProjectEntity), id);

        if (project.OwnerId != userId)
            throw new ForbiddenException();

        project.Name = name;
        project.Description = description;
        return await _projectRepository.UpdateAsync(project);
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var project = await _projectRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(ProjectEntity), id);

        if (project.OwnerId != userId)
            throw new ForbiddenException();

        await _projectRepository.DeleteAsync(project);
    }
}
