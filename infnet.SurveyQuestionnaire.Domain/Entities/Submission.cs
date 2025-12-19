using infnet.SurveyQuestionnaire.Domain.Common;
using infnet.SurveyQuestionnaire.Domain.Exceptions;

namespace infnet.SurveyQuestionnaire.Domain.Entities;

/// <summary>
/// Representa uma submissão (resposta) de um questionário
/// </summary>
public sealed class Submission : Entity
{
    private readonly List<SubmissionItem> _items = [];

    /// <summary>
    /// ID do questionário respondido
    /// </summary>
    public Guid QuestionnaireId { get; private set; }

    /// <summary>
    /// ID do usuário que respondeu (deve ser usuário público)
    /// </summary>
    public Guid RespondentUserId { get; private set; }

    /// <summary>
    /// Data e hora da submissão
    /// </summary>
    public DateTime SubmittedAt { get; private set; }

    /// <summary>
    /// Status do processamento
    /// </summary>
    public SubmissionStatus Status { get; private set; }

    /// <summary>
    /// Mensagem de erro (caso Status = Failed)
    /// </summary>
    public string? FailureReason { get; private set; }

    /// <summary>
    /// Respostas das questões
    /// </summary>
    public IReadOnlyCollection<SubmissionItem> Items => _items.AsReadOnly();

    private Submission()
    {
    }

  private Submission(Guid questionnaireId, Guid respondentUserId) : base()
  {
        if (questionnaireId == Guid.Empty)
            throw new ArgumentException("Questionnaire ID cannot be empty", nameof(questionnaireId));

        if (respondentUserId == Guid.Empty)
          throw new ArgumentException("Respondent user ID cannot be empty", nameof(respondentUserId));

         QuestionnaireId = questionnaireId;
        RespondentUserId = respondentUserId;
        SubmittedAt = DateTime.UtcNow;
        Status = SubmissionStatus.Pending;
  }

    /// <summary>
    /// Cria uma nova submissão
    /// </summary>
    /// <param name="questionnaireId">ID do questionário</param>
    /// <param name="respondentUserId">ID do usuário público</param>
    public static Submission Create(Guid questionnaireId, Guid respondentUserId)
    {
        return new Submission(questionnaireId, respondentUserId);
    }

    /// <summary>
    /// Adiciona uma resposta à submissão
    /// </summary>
    /// <param name="questionId">ID da questão</param>
    /// <param name="answer">Resposta em texto</param>
    /// <param name="selectedOptionId">ID da opção selecionada (opcional)</param>
    public void AddItem(Guid questionId, string answer, Guid? selectedOptionId = null)
    {
        if (Status != SubmissionStatus.Pending)
            throw new InvalidSubmissionException($"Cannot add items to submission with status {Status}");

        if (_items.Any(i => i.QuestionId == questionId))
            throw new InvalidSubmissionException($"Question '{questionId}' has already been answered in this submission");

        var item = SubmissionItem.Create(questionId, answer, selectedOptionId);
        
        _items.Add(item);
        SetUpdatedAt();
    }

    /// <summary>
    /// Marca a submissão como em processamento
    /// </summary>
    public void StartProcessing()
    {
        if (Status != SubmissionStatus.Pending)
            throw new InvalidSubmissionException($"Cannot start processing submission with status {Status}");

        Status = SubmissionStatus.Processing;
        SetUpdatedAt();
    }

    /// <summary>
    /// Marca a submissão como completada
    /// </summary>
 public void Complete()
    {
        if (Status != SubmissionStatus.Processing && Status != SubmissionStatus.Pending)
            throw new InvalidSubmissionException($"Cannot complete submission with status {Status}");

      if (_items.Count == 0)
            throw new InvalidSubmissionException("Cannot complete submission without any answers");

    Status = SubmissionStatus.Completed;
        SetUpdatedAt();
    }

    /// <summary>
    /// Marca a submissão como falha
    /// </summary>
    /// <param name="reason">Motivo da falha</param>
    public void Fail(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Failure reason cannot be empty", nameof(reason));

        Status = SubmissionStatus.Failed;
        FailureReason = reason.Trim();
        SetUpdatedAt();
    }

    /// <summary>
    /// Verifica se a submissão tem resposta para uma questão específica
    /// </summary>
    public bool HasAnswerForQuestion(Guid questionId)
    {
        return _items.Any(i => i.QuestionId == questionId);
    }

    /// <summary>
    /// Obtém a resposta de uma questão específica
    /// </summary>
    public SubmissionItem? GetAnswerForQuestion(Guid questionId)
    {
        return _items.FirstOrDefault(i => i.QuestionId == questionId);
    }
}
