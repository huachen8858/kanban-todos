namespace TaskFlow.API.DTOs.Responses;

public record TaskCommentResponse(
    int Id,
    string Content,
    int TaskId,
    int AuthorId,
    DateTime CreatedAt);
