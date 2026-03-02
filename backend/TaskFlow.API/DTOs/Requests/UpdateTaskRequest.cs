namespace TaskFlow.API.DTOs.Requests;

public record UpdateTaskRequest(
    string Title,
    string? Description,
    string Priority,
    DateOnly? DueDate,
    int? AssigneeId);
