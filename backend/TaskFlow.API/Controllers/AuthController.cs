using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.DTOs.Requests;
using TaskFlow.API.DTOs.Responses;
using TaskFlow.Application.Services;

namespace TaskFlow.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthController(
        IAuthService authService,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator)
    {
        _authService = authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        await _registerValidator.ValidateAndThrowAsync(request);
        var result = await _authService.RegisterAsync(request.Email, request.Name, request.Password);
        var response = new AuthResponse(result.Token, result.UserId, result.Email, result.Name);
        return StatusCode(201, ApiResponse<AuthResponse>.Ok(response, "User registered successfully."));
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        await _loginValidator.ValidateAndThrowAsync(request);
        var result = await _authService.LoginAsync(request.Email, request.Password);
        var response = new AuthResponse(result.Token, result.UserId, result.Email, result.Name);
        return Ok(ApiResponse<AuthResponse>.Ok(response, "Login successful."));
    }
}
