using FluentValidation;
using TaskFlow.API.DTOs.Requests;

namespace TaskFlow.API.Validators;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required.");
    }
}
