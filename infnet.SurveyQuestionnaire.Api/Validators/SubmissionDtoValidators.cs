using FluentValidation;
using infnet.SurveyQuestionnaire.Api.DTOs;

namespace infnet.SurveyQuestionnaire.Api.Validators;

/// <summary>
/// Validador para CreateSubmissionRequestDto
/// </summary>
public class CreateSubmissionRequestDtoValidator : AbstractValidator<CreateSubmissionRequestDto>
{
  public CreateSubmissionRequestDtoValidator()
  {
        RuleFor(x => x.QuestionnaireId)
            .NotEmpty()
     .WithMessage("Questionnaire ID is required");

     RuleFor(x => x.Answers)
      .NotNull()
      .WithMessage("Answers are required")
     .NotEmpty()
         .WithMessage("At least one answer is required")
      .Must(answers => answers.Count <= 100)
      .WithMessage("Cannot submit more than 100 answers at once");

        RuleForEach(x => x.Answers)
      .ChildRules(answer =>
      {
        answer.RuleFor(a => a.QuestionId)
            .NotEmpty()
            .WithMessage("Question ID is required");

            answer.RuleFor(a => a.Answer)
            .NotEmpty()
             .WithMessage("Answer is required")
             .MaximumLength(5000)
            .WithMessage("Answer cannot exceed 5000 characters");
      });
    }
}

/// <summary>
/// Validador para SubmissionAnswerDto (não mais necessário, mas mantido para referência)
/// </summary>
public class SubmissionAnswerDtoValidator : AbstractValidator<SubmissionAnswerDto>
{
    public SubmissionAnswerDtoValidator()
    {
        RuleFor(x => x.QuestionId)
            .NotEmpty()
            .WithMessage("Question ID is required");

        RuleFor(x => x.Answer)
        .NotEmpty()
        .WithMessage("Answer is required")
        .MaximumLength(5000)
        .WithMessage("Answer cannot exceed 5000 characters");
    }
}
