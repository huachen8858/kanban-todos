using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services;

public interface IProjectService
{
    Task<IEnumerable<ProjectEntity>> GetAllAsync(int userId);
    Task<ProjectEntity> GetByIdAsync(int id, int userId);
    Task<ProjectEntity> CreateAsync(string name, string? description, int userId);
    Task<ProjectEntity> UpdateAsync(int id, string name, string? description, int userId);
    Task DeleteAsync(int id, int userId);
}
