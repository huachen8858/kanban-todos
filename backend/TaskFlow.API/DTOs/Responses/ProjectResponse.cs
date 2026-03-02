namespace TaskFlow.API.DTOs.Responses;

public record ProjectResponse(
    int Id,
    string Name,
    string? Description,
    int OwnerId,
    DateTime CreatedAt,
    DateTime UpdatedAt);
