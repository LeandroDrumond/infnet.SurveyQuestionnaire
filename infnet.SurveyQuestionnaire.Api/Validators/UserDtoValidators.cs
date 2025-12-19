using FluentValidation;
using infnet.SurveyQuestionnaire.Api.DTOs;

namespace infnet.SurveyQuestionnaire.Api.Validators;

public class CreateUserRequestDtoValidator : AbstractValidator<CreateUserRequestDto>
{
    public CreateUserRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(3, 100)
            .WithMessage("Name must be between 3 and 100 characters")
            .Matches(@"^[a-zA-ZÀ-ÿ\s'-]+$")
            .WithMessage("Name can only contain letters, spaces, hyphens and apostrophes");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .MaximumLength(200)
            .WithMessage("Email cannot exceed 200 characters")
            .EmailAddress()
            .WithMessage("Invalid email format");

        RuleFor(x => x.UserType)
            .NotEmpty()
            .WithMessage("UserType is required")
            .Must(type => type == "Public" || type == "Administrator")
            .WithMessage("UserType must be 'Public' or 'Administrator'");
    }
}

public class UpdateUserRequestDtoValidator : AbstractValidator<UpdateUserRequestDto>
{
    public UpdateUserRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(3, 100)
            .WithMessage("Name must be between 3 and 100 characters")
            .Matches(@"^[a-zA-ZÀ-ÿ\s'-]+$")
            .WithMessage("Name can only contain letters, spaces, hyphens and apostrophes");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .MaximumLength(200)
            .WithMessage("Email cannot exceed 200 characters")
            .EmailAddress()
            .WithMessage("Invalid email format");
    }
}
