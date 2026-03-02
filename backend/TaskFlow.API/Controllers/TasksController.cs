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
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly IValidator<CreateTaskRequest> _createValidator;
    private readonly IValidator<UpdateTaskRequest> _updateValidator;
    private readonly IValidator<UpdateTaskStatusRequest> _statusValidator;

    public TasksController(
        ITaskService taskService,
        IValidator<CreateTaskRequest> createValidator,
        IValidator<UpdateTaskRequest> updateValidator,
        IValidator<UpdateTaskStatusRequest> statusValidator)
    {
        _taskService = taskService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _statusValidator = statusValidator;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpGet("api/v1/projects/{projectId:int}/tasks")]
    public async Task<IActionResult> GetByProject(int projectId)
    {
        var tasks = await _taskService.GetByProjectAsync(projectId, CurrentUserId);
        return Ok(ApiResponse<IEnumerable<TaskResponse>>.Ok(tasks.Select(ToResponse)));
    }

    [HttpGet("api/v1/tasks/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await _taskService.GetByIdAsync(id, CurrentUserId);
        return Ok(ApiResponse<TaskResponse>.Ok(ToResponse(task)));
    }

    [HttpPost("api/v1/projects/{projectId:int}/tasks")]
    public async Task<IActionResult> Create(int projectId, [FromBody] CreateTaskRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);
        var task = await _taskService.CreateAsync(
            projectId, request.Title, request.Description,
            request.Priority, request.DueDate, request.AssigneeId, CurrentUserId);
        return StatusCode(201, ApiResponse<TaskResponse>.Ok(ToResponse(task), "Task created."));
    }

    [HttpPut("api/v1/tasks/{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);
        var task = await _taskService.UpdateAsync(
            id, request.Title, request.Description,
            request.Priority, request.DueDate, request.AssigneeId, CurrentUserId);
        return Ok(ApiResponse<TaskResponse>.Ok(ToResponse(task), "Task updated."));
    }

    [HttpPatch("api/v1/tasks/{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTaskStatusRequest request)
    {
        await _statusValidator.ValidateAndThrowAsync(request);
        var task = await _taskService.UpdateStatusAsync(id, request.Status, CurrentUserId);
        return Ok(ApiResponse<TaskResponse>.Ok(ToResponse(task), "Task status updated."));
    }

    [HttpDelete("api/v1/tasks/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _taskService.DeleteAsync(id, CurrentUserId);
        return NoContent();
    }

    private static TaskResponse ToResponse(TaskItem t)
        => new(t.Id, t.Title, t.Description, t.Status.ToString(), t.Priority.ToString(),
               t.DueDate, t.ProjectId, t.AssigneeId, t.CreatedAt, t.UpdatedAt);
}
