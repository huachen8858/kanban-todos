namespace TaskFlow.API.DTOs.Responses;

public record TaskResponse(
    int Id,
    string Title,
    string? Description,
    string Status,
    string Priority,
    DateOnly? DueDate,
    int ProjectId,
    int? AssigneeId,
    DateTime CreatedAt,
    DateTime UpdatedAt);
