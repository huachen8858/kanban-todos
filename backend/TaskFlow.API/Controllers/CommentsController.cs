using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.DTOs.Requests;
using TaskFlow.API.DTOs.Responses;
using TaskFlow.Application.Services;
using TaskFlow.Domain.Entities;

namespace TaskFlow.API.Controllers;

[ApiController]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ITaskCommentService _commentService;
    private readonly IValidator<CreateCommentRequest> _createValidator;

    public CommentsController(
        ITaskCommentService commentService,
        IValidator<CreateCommentRequest> createValidator)
    {
        _commentService = commentService;
        _createValidator = createValidator;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpGet("api/v1/tasks/{taskId:int}/comments")]
    public async Task<IActionResult> GetByTask(int taskId)
    {
        var comments = await _commentService.GetByTaskAsync(taskId, CurrentUserId);
        return Ok(ApiResponse<IEnumerable<TaskCommentResponse>>.Ok(comments.Select(ToResponse)));
    }

    [HttpPost("api/v1/tasks/{taskId:int}/comments")]
    public async Task<IActionResult> Create(int taskId, [FromBody] CreateCommentRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);
        var comment = await _commentService.CreateAsync(taskId, request.Content, CurrentUserId);
        return StatusCode(201, ApiResponse<TaskCommentResponse>.Ok(ToResponse(comment), "Comment added."));
    }

    [HttpDelete("api/v1/comments/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _commentService.DeleteAsync(id, CurrentUserId);
        return NoContent();
    }

    private static TaskCommentResponse ToResponse(TaskCommentEntity c)
        => new(c.Id, c.Content, c.TaskId, c.AuthorId, c.CreatedAt);
}
