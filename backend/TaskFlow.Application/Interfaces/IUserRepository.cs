using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetByIdAsync(int id);
    Task<UserEntity?> GetByEmailAsync(string email);
    Task<UserEntity> CreateAsync(UserEntity user);
}
