using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Application.Exceptions;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<AuthServiceResult> RegisterAsync(string email, string name, string password)
    {
        var existing = await _userRepository.GetByEmailAsync(email);
        if (existing != null)
            throw new InvalidOperationException("A user with this email already exists.");

        var user = new UserEntity
        {
            Email = email,
            Name = name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        var created = await _userRepository.CreateAsync(user);
        var token = GenerateToken(created);
        return new AuthServiceResult(token, created.Id, created.Email, created.Name);
    }

    public async Task<AuthServiceResult> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var token = GenerateToken(user);
        return new AuthServiceResult(token, user.Id, user.Email, user.Name);
    }

    private string GenerateToken(UserEntity user)
    {
        var secret = _configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("Jwt:Secret is not configured.");
        var issuer = _configuration["Jwt:Issuer"] ?? "TaskFlow";
        var audience = _configuration["Jwt:Audience"] ?? "TaskFlowUsers";
        var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "1440");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
