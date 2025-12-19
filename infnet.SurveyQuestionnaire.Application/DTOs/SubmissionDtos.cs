namespace infnet.SurveyQuestionnaire.Application.DTOs.Submissions;

// ==================== Requests ====================

/// <summary>
/// DTO para criação de submission (resposta de questionário)
/// </summary>
public record CreateSubmissionRequest
{
    public Guid QuestionnaireId { get; init; }
    public List<SubmissionAnswerRequest> Answers { get; init; } = [];
}

/// <summary>
/// DTO para uma resposta individual
/// </summary>
public record SubmissionAnswerRequest
{
    public Guid QuestionId { get; init; }
    public string Answer { get; init; } = string.Empty;
    public Guid? SelectedOptionId { get; init; }
}

// ==================== Responses ====================

/// <summary>
/// DTO de resposta para submission
/// </summary>
public record SubmissionResponse
{
    public Guid Id { get; init; }
    public Guid QuestionnaireId { get; init; }
    public Guid RespondentUserId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime SubmittedAt { get; init; }
    public string? FailureReason { get; init; }
    public List<SubmissionItemResponse>? Items { get; init; }
}

/// <summary>
/// DTO de resposta para item de submission
/// </summary>
public record SubmissionItemResponse
{
    public Guid Id { get; init; }
    public Guid QuestionId { get; init; }
    public string QuestionText { get; init; } = string.Empty;
    public string Answer { get; init; } = string.Empty;
    public Guid? SelectedOptionId { get; init; }
    public string? SelectedOptionText { get; init; }
}

/// <summary>
/// DTO de resposta simplificada (lista)
/// </summary>
public record SubmissionListResponse
{
    public Guid Id { get; init; }
  public Guid QuestionnaireId { get; init; }
    public string QuestionnaireTitle { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime SubmittedAt { get; init; }
}

// ==================== Service Bus Message ====================

/// <summary>
/// Mensagem enviada para o Service Bus para processamento assíncrono
/// </summary>
public record SubmissionMessage
{
    public Guid SubmissionId { get; init; }
    public Guid QuestionnaireId { get; init; }
    public Guid RespondentUserId { get; init; }
    public DateTime SubmittedAt { get; init; }
  public List<SubmissionAnswerMessage> Answers { get; init; } = [];
}

/// <summary>
/// Resposta individual na mensagem do Service Bus
/// </summary>
public record SubmissionAnswerMessage
{
    public Guid QuestionId { get; init; }
    public string Answer { get; init; } = string.Empty;
    public Guid? SelectedOptionId { get; init; }
}
