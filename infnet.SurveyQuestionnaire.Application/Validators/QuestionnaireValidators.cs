using FluentValidation;
using infnet.SurveyQuestionnaire.Application.DTOs.Questionnaires;

namespace infnet.SurveyQuestionnaire.Application.Validators;

public class CreateQuestionnaireRequestValidator : AbstractValidator<CreateQuestionnaireRequest>
{
    public CreateQuestionnaireRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
 .WithMessage("Title is required")
    .Length(3, 200)
     .WithMessage("Title must be between 3 and 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
       .WithMessage("Description is required")
          .MaximumLength(1000)
         .WithMessage("Description cannot exceed 1000 characters");
    }
}

public class UpdateQuestionnaireRequestValidator : AbstractValidator<UpdateQuestionnaireRequest>
{
    public UpdateQuestionnaireRequestValidator()
    {
        RuleFor(x => x.Title)
     .NotEmpty()
            .WithMessage("Title is required")
    .Length(3, 200)
         .WithMessage("Title must be between 3 and 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
  .WithMessage("Description is required")
            .MaximumLength(1000)
          .WithMessage("Description cannot exceed 1000 characters");
    }
}

public class PublishQuestionnaireRequestValidator : AbstractValidator<PublishQuestionnaireRequest>
{
    public PublishQuestionnaireRequestValidator()
    {
        RuleFor(x => x.CollectionStart)
       .NotEmpty()
       .WithMessage("Collection start date is required")
          .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
   .WithMessage("Collection start date must be today or in the future");

    RuleFor(x => x.CollectionEnd)
     .NotEmpty()
    .WithMessage("Collection end date is required")
            .GreaterThan(x => x.CollectionStart)
     .WithMessage("Collection end date must be after collection start date");
    }
}

public class AddQuestionRequestValidator : AbstractValidator<AddQuestionRequest>
{
    public AddQuestionRequestValidator()
    {
  RuleFor(x => x.Text)
   .NotEmpty()
     .WithMessage("Question text is required")
          .Length(3, 500)
.WithMessage("Question text must be between 3 and 500 characters");

   // Se for múltipla escolha, deve ter pelo menos 2 opções
        RuleFor(x => x.Options)
            .NotNull()
.When(x => x.IsMultipleChoice)
     .WithMessage("Multiple choice questions must have options");

        RuleFor(x => x.Options)
          .Must(options => options != null && options.Count >= 2)
            .When(x => x.IsMultipleChoice)
  .WithMessage("Multiple choice questions must have at least 2 options");

    RuleForEach(x => x.Options)
            .SetValidator(new AddOptionRequestValidator())
        .When(x => x.Options != null);
    }
}

public class UpdateQuestionRequestValidator : AbstractValidator<UpdateQuestionRequest>
{
    public UpdateQuestionRequestValidator()
    {
        RuleFor(x => x.Text)
   .NotEmpty()
            .WithMessage("Question text is required")
   .Length(3, 500)
            .WithMessage("Question text must be between 3 and 500 characters");
    }
}

public class AddOptionRequestValidator : AbstractValidator<AddOptionRequest>
{
    public AddOptionRequestValidator()
    {
 RuleFor(x => x.Text)
            .NotEmpty()
  .WithMessage("Option text is required")
            .Length(1, 200)
      .WithMessage("Option text must be between 1 and 200 characters");

        RuleFor(x => x.Order)
  .GreaterThan(0)
    .WithMessage("Order must be greater than 0");
    }
}
