using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces;

public interface IProjectRepository
{
    Task<IEnumerable<ProjectEntity>> GetByOwnerAsync(int ownerId);
    Task<ProjectEntity?> GetByIdAsync(int id);
    Task<ProjectEntity> CreateAsync(ProjectEntity project);
    Task<ProjectEntity> UpdateAsync(ProjectEntity project);
    Task DeleteAsync(ProjectEntity project);
}
