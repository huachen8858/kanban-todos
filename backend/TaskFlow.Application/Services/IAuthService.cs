namespace TaskFlow.Application.Services;

public interface IAuthService
{
    Task<AuthServiceResult> RegisterAsync(string email, string name, string password);
    Task<AuthServiceResult> LoginAsync(string email, string password);
}

public record AuthServiceResult(string Token, int UserId, string Email, string Name);
