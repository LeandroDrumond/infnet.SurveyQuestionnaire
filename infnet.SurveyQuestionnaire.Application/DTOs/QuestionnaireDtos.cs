namespace infnet.SurveyQuestionnaire.Application.DTOs.Questionnaires;

/// <summary>
/// DTO para criação de questionário
/// </summary>
public record CreateQuestionnaireRequest
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

/// <summary>
/// DTO para atualização de questionário
/// </summary>
public record UpdateQuestionnaireRequest
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

/// <summary>
/// DTO para publicação de questionário
/// </summary>
public record PublishQuestionnaireRequest
{
    public DateTime CollectionStart { get; init; }
    public DateTime CollectionEnd { get; init; }
}

/// <summary>
/// DTO para adicionar questão ao questionário
/// </summary>
public record AddQuestionRequest
{
    public string Text { get; init; } = string.Empty;
    public bool IsRequired { get; init; }
    public bool IsMultipleChoice { get; init; }
    public List<AddOptionRequest>? Options { get; init; }
}

/// <summary>
/// DTO para atualizar questão
/// </summary>
public record UpdateQuestionRequest
{
    public string Text { get; init; } = string.Empty;
 public bool IsRequired { get; init; }
    public bool IsMultipleChoice { get; init; }
}

/// <summary>
/// DTO para adicionar opção a uma questão
/// </summary>
public record AddOptionRequest
{
    public string Text { get; init; } = string.Empty;
    public int Order { get; init; }
}

/// <summary>
/// DTO de resposta completo do questionário
/// </summary>
public record QuestionnaireResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime? CollectionStart { get; init; }
    public DateTime? CollectionEnd { get; init; }
    public Guid CreatedByUserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<QuestionResponse>? Questions { get; init; }
}

/// <summary>
/// DTO de resposta resumido do questionário (para listagens)
/// </summary>
public record QuestionnaireListResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime? CollectionStart { get; init; }
 public DateTime? CollectionEnd { get; init; }
    public int QuestionCount { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// DTO de resposta da questão
/// </summary>
public record QuestionResponse
{
    public Guid Id { get; init; }
    public string Text { get; init; } = string.Empty;
    public bool IsRequired { get; init; }
    public bool IsMultipleChoice { get; init; }
    public DateTime CreatedAt { get; init; }
 public List<OptionResponse>? Options { get; init; }
}

/// <summary>
/// DTO de resposta da opção
/// </summary>
public record OptionResponse
{
    public Guid Id { get; init; }
    public string Text { get; init; } = string.Empty;
    public int Order { get; init; }
}
