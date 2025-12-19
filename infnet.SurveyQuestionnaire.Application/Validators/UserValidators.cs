using FluentValidation;
using infnet.SurveyQuestionnaire.Application.DTOs.Users;

namespace infnet.SurveyQuestionnaire.Application.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
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
      .WithMessage("Invalid email format")
           .Must(BeAValidEmailDomain)
      .WithMessage("Email domain is not valid");
    }

    private bool BeAValidEmailDomain(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var parts = email.Split('@');
        if (parts.Length != 2)
        {
            return false;
        }

        var domain = parts[1];
        return domain.Contains('.') && domain.Length >= 3;
    }
}

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
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
    .WithMessage("Invalid email format")
    .Must(BeAValidEmailDomain)
 .WithMessage("Email domain is not valid");
    }

    private bool BeAValidEmailDomain(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var parts = email.Split('@');
        if (parts.Length != 2)
        {
            return false;
        }

        var domain = parts[1];
        return domain.Contains('.') && domain.Length >= 3;
    }
}
