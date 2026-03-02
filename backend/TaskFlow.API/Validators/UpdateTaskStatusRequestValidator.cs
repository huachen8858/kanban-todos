using FluentValidation;
using TaskFlow.API.DTOs.Requests;

namespace TaskFlow.API.Validators;

public class UpdateTaskStatusRequestValidator : AbstractValidator<UpdateTaskStatusRequest>
{
    private static readonly string[] ValidStatuses = ["Todo", "InProgress", "Done"];

    public UpdateTaskStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => ValidStatuses.Contains(s))
            .WithMessage("Status must be Todo, InProgress, or Done.");
    }
}
