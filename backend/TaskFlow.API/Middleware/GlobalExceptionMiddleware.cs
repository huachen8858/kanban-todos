using System.Text.Json;
using FluentValidation;
using TaskFlow.Application.Exceptions;

namespace TaskFlow.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => e.ErrorMessage).ToArray();
            await WriteErrorResponse(context, StatusCodes.Status400BadRequest, "Validation failed.", errors);
        }
        catch (NotFoundException ex)
        {
            await WriteErrorResponse(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (ForbiddenException ex)
        {
            await WriteErrorResponse(context, StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await WriteErrorResponse(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorResponse(context, StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorResponse(
        HttpContext context, int statusCode, string message, string[]? errors = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var body = new
        {
            success = false,
            data = (object?)null,
            message,
            errors = errors ?? []
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
