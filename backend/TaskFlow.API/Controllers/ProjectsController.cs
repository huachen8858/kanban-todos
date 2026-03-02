using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.DTOs.Requests;
using TaskFlow.API.DTOs.Responses;
using TaskFlow.Application.Services;

namespace TaskFlow.API.Controllers;

[ApiController]
[Route("api/v1/projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IValidator<CreateProjectRequest> _createValidator;
    private readonly IValidator<UpdateProjectRequest> _updateValidator;

    public ProjectsController(
        IProjectService projectService,
        IValidator<CreateProjectRequest> createValidator,
        IValidator<UpdateProjectRequest> updateValidator)
    {
        _projectService = projectService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var projects = await _projectService.GetAllAsync(CurrentUserId);
        var response = projects.Select(p => ToResponse(p));
        return Ok(ApiResponse<IEnumerable<ProjectResponse>>.Ok(response));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var project = await _projectService.GetByIdAsync(id, CurrentUserId);
        return Ok(ApiResponse<ProjectResponse>.Ok(ToResponse(project)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);
        var project = await _projectService.CreateAsync(request.Name, request.Description, CurrentUserId);
        return StatusCode(201, ApiResponse<ProjectResponse>.Ok(ToResponse(project), "Project created."));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);
        var project = await _projectService.UpdateAsync(id, request.Name, request.Description, CurrentUserId);
        return Ok(ApiResponse<ProjectResponse>.Ok(ToResponse(project), "Project updated."));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _projectService.DeleteAsync(id, CurrentUserId);
        return NoContent();
    }

    private static ProjectResponse ToResponse(Domain.Entities.ProjectEntity p)
        => new(p.Id, p.Name, p.Description, p.OwnerId, p.CreatedAt, p.UpdatedAt);
}
