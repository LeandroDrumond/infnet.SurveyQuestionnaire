using System.ComponentModel.DataAnnotations;

namespace infnet.SurveyQuestionnaire.Api.DTOs;

/// <summary>
/// DTO para criação de submission (resposta de questionário)
/// </summary>
public record CreateSubmissionRequestDto
{
    /// <summary>
    /// ID do questionário sendo respondido
    /// </summary>
    public Guid QuestionnaireId { get; init; }

    /// <summary>
    /// Lista de respostas
    /// </summary>
    public List<SubmissionAnswerDto> Answers { get; init; } = [];
}

/// <summary>
/// DTO para uma resposta individual
/// </summary>
public record SubmissionAnswerDto
{
    /// <summary>
    /// ID da questão sendo respondida
    /// </summary>
    public Guid QuestionId { get; init; }

    /// <summary>
    /// Resposta em texto
    /// </summary>
    public string Answer { get; init; } = string.Empty;

    /// <summary>
    /// ID da opção selecionada (apenas para questões de múltipla escolha)
    /// </summary>
    public Guid? SelectedOptionId { get; init; }
}

/// <summary>
/// DTO de resposta para submission
/// </summary>
public record SubmissionResponseDto
{
    public Guid Id { get; init; }
    public Guid QuestionnaireId { get; init; }
    public Guid RespondentUserId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime SubmittedAt { get; init; }
    public string? FailureReason { get; init; }
    public List<SubmissionItemResponseDto>? Items { get; init; }
}

/// <summary>
/// DTO de resposta para item de submission
/// </summary>
public record SubmissionItemResponseDto
{
    public Guid Id { get; init; }
    public Guid QuestionId { get; init; }
    public string QuestionText { get; init; } = string.Empty;
    public string Answer { get; init; } = string.Empty;
    public Guid? SelectedOptionId { get; init; }
    public string? SelectedOptionText { get; init; }
}

/// <summary>
/// DTO de resposta simplificada para lista
/// </summary>
public record SubmissionListResponseDto
{
    public Guid Id { get; init; }
    public Guid QuestionnaireId { get; init; }
    public string QuestionnaireTitle { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime SubmittedAt { get; init; }
}
