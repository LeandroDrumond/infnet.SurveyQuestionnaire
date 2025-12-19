using infnet.SurveyQuestionnaire.Domain.Common;

namespace infnet.SurveyQuestionnaire.Domain.Entities;

/// <summary>
/// Representa uma resposta individual de uma questão
/// </summary>
public sealed class SubmissionItem : Entity
{
    private const int _answerMaxLength = 5000;

    /// <summary>
    /// ID da questão respondida
    /// </summary>
    public Guid QuestionId { get; internal set; }

    /// <summary>
    /// Resposta em texto (pode ser texto livre ou o texto da opção selecionada)
    /// </summary>
    public string Answer { get; private set; } = string.Empty;

    /// <summary>
    /// ID da opção selecionada (apenas para questões de múltipla escolha)
    /// </summary>
    public Guid? SelectedOptionId { get; internal set; }

    /// <summary>
    /// Foreign Key para Submission (necessário para EF Core)
    /// </summary>
    public Guid SubmissionId { get; internal set; }

    // EF Core Constructor
    private SubmissionItem()
    {
    }

    private SubmissionItem(Guid questionId, string answer, Guid? selectedOptionId) : base()
    {
        QuestionId = questionId;
        Answer = ValidateAndTrimAnswer(answer);
        SelectedOptionId = selectedOptionId;
    }

    /// <summary>
    /// Cria um novo item de submissão (INTERNAL - só chamado por Submission)
    /// </summary>
    /// <param name="questionId">ID da questão</param>
    /// <param name="answer">Resposta em texto</param>
    /// <param name="selectedOptionId">ID da opção selecionada (opcional)</param>
    internal static SubmissionItem Create(Guid questionId, string answer, Guid? selectedOptionId = null)
    {
        if (questionId == Guid.Empty)
            throw new ArgumentException("Question ID cannot be empty", nameof(questionId));

        return new SubmissionItem(questionId, answer, selectedOptionId);
    }

    private static string ValidateAndTrimAnswer(string answer)
    {
     if (string.IsNullOrWhiteSpace(answer))
        throw new ArgumentException("Answer cannot be empty", nameof(answer));

        var trimmedAnswer = answer.Trim();

        if (trimmedAnswer.Length > _answerMaxLength)
            throw new ArgumentException( $"Answer cannot exceed {_answerMaxLength} characters",nameof(answer));

        return trimmedAnswer;
    }
}
