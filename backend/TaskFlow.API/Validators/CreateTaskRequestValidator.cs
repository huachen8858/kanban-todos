using FluentValidation;
using TaskFlow.API.DTOs.Requests;

namespace TaskFlow.API.Validators;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    private static readonly string[] ValidPriorities = ["Low", "Medium", "High"];

    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .MaximumLength(300).WithMessage("Task title must not exceed 300 characters.");

        RuleFor(x => x.Priority)
            .Must(p => ValidPriorities.Contains(p))
            .WithMessage("Priority must be Low, Medium, or High.");
    }
}
