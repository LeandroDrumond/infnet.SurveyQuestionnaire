using FluentValidation;
using infnet.SurveyQuestionnaire.Api.DTOs;

namespace infnet.SurveyQuestionnaire.Api.Validators;

/// <summary>
/// Validador para criação de questionário
/// </summary>
public class CreateQuestionnaireRequestDtoValidator : AbstractValidator<CreateQuestionnaireRequestDto>
{
    public CreateQuestionnaireRequestDtoValidator()
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

/// <summary>
/// Validador para atualização de questionário
/// </summary>
public class UpdateQuestionnaireRequestDtoValidator : AbstractValidator<UpdateQuestionnaireRequestDto>
{
    public UpdateQuestionnaireRequestDtoValidator()
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

/// <summary>
/// Validador para publicação de questionário
/// </summary>
public class PublishQuestionnaireRequestDtoValidator : AbstractValidator<PublishQuestionnaireRequestDto>
{
 public PublishQuestionnaireRequestDtoValidator()
    {
   RuleFor(x => x.CollectionStart)
       .NotEmpty()
    .WithMessage("Collection start date is required")
   .Must(BeAValidDate)
            .WithMessage("Collection start date must be a valid date")
       .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Collection start date must be today or in the future");

        RuleFor(x => x.CollectionEnd)
     .NotEmpty()
            .WithMessage("Collection end date is required")
            .Must(BeAValidDate)
.WithMessage("Collection end date must be a valid date")
          .GreaterThan(x => x.CollectionStart)
     .WithMessage("Collection end date must be after collection start date");

        // Validação adicional: período mínimo de 1 dia
        RuleFor(x => x)
    .Must(x => (x.CollectionEnd - x.CollectionStart).TotalDays >= 1)
          .WithMessage("Collection period must be at least 1 day")
            .When(x => x.CollectionStart != default && x.CollectionEnd != default);

        // Validação adicional: período máximo de 1 ano
        RuleFor(x => x)
   .Must(x => (x.CollectionEnd - x.CollectionStart).TotalDays <= 365)
     .WithMessage("Collection period cannot exceed 1 year")
     .When(x => x.CollectionStart != default && x.CollectionEnd != default);
    }

    private bool BeAValidDate(DateTime date)
    {
        return date != default && date > DateTime.MinValue && date < DateTime.MaxValue;
    }
}

/// <summary>
/// Validador para adicionar questão
/// </summary>
public class AddQuestionRequestDtoValidator : AbstractValidator<AddQuestionRequestDto>
{
    public AddQuestionRequestDtoValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
  .WithMessage("Question text is required")
      .Length(3, 500)
            .WithMessage("Question text must be between 3 and 500 characters");

        // Se for múltipla escolha, deve ter opções
        RuleFor(x => x.Options)
            .NotNull()
       .WithMessage("Multiple choice questions must have options")
       .When(x => x.IsMultipleChoice);

        // Se for múltipla escolha, deve ter pelo menos 2 opções
    RuleFor(x => x.Options)
 .Must(options => options != null && options.Count >= 2)
            .WithMessage("Multiple choice questions must have at least 2 options")
   .When(x => x.IsMultipleChoice);

      // Validar cada opção individualmente
   RuleForEach(x => x.Options)
          .SetValidator(new AddOptionRequestDtoValidator())
     .When(x => x.Options != null);

        // Se não for múltipla escolha, não deve ter opções
    RuleFor(x => x.Options)
        .Must(options => options == null || options.Count == 0)
       .WithMessage("Non-multiple choice questions cannot have options")
.When(x => !x.IsMultipleChoice);
    }
}

/// <summary>
/// Validador para atualizar questão
/// </summary>
public class UpdateQuestionRequestDtoValidator : AbstractValidator<UpdateQuestionRequestDto>
{
    public UpdateQuestionRequestDtoValidator()
{
        RuleFor(x => x.Text)
     .NotEmpty()
   .WithMessage("Question text is required")
  .Length(3, 500)
         .WithMessage("Question text must be between 3 and 500 characters");
    }
}

/// <summary>
/// Validador para adicionar opção
/// </summary>
public class AddOptionRequestDtoValidator : AbstractValidator<AddOptionRequestDto>
{
    public AddOptionRequestDtoValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
      .WithMessage("Option text is required")
.Length(1, 200)
            .WithMessage("Option text must be between 1 and 200 characters");

        RuleFor(x => x.Order)
   .GreaterThan(0)
          .WithMessage("Order must be greater than 0")
            .LessThanOrEqualTo(100)
      .WithMessage("Order cannot exceed 100");
    }
}
