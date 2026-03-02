namespace TaskFlow.API.DTOs.Responses;

public record AuthResponse(string Token, int UserId, string Email, string Name);
