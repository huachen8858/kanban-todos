namespace TaskFlow.API.DTOs.Requests;

public record CreateTaskRequest(
    string Title,
    string? Description,
    string Priority,
    DateOnly? DueDate,
    int? AssigneeId);
