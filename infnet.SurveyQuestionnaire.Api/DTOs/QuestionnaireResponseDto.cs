using System.ComponentModel.DataAnnotations;

namespace infnet.SurveyQuestionnaire.Api.DTOs;

/// <summary>
/// DTO de resposta completo do questionário (API Layer)
/// </summary>
public class QuestionnaireResponseDto
{
    /// <summary>
    /// Identificador único do questionário (always returned)
    /// </summary>
    public Guid Id { get; init; }
 
    /// <summary>
    /// Título do questionário (always returned)
    /// </summary>
 public string Title { get; init; } = string.Empty;
    
    /// <summary>
    /// Descrição do questionário (always returned)
    /// </summary>
public string Description { get; init; } = string.Empty;
 
    /// <summary>
    /// Status do questionário: Draft, Published ou Closed (always returned)
    /// </summary>
    public string Status { get; init; } = string.Empty;
    
    /// <summary>
    /// Data de início do período de coleta (optional, null if not published)
    /// </summary>
    public DateTime? CollectionStart { get; init; }
    
    /// <summary>
    /// Data de fim do período de coleta (optional, null if not published)
    /// </summary>
    public DateTime? CollectionEnd { get; init; }
    
    /// <summary>
    /// ID do usuário que criou o questionário (always returned)
    /// </summary>
    public Guid CreatedByUserId { get; init; }
 
    /// <summary>
    /// Data e hora de criação em UTC (always returned)
    /// </summary>
    public DateTime CreatedAt { get; init; }
  
    /// <summary>
    /// Data e hora da última atualização em UTC (optional, may be null)
/// </summary>
    public DateTime? UpdatedAt { get; init; }
    
  /// <summary>
    /// Lista de questões do questionário (optional, null in basic response)
    /// </summary>
    public List<QuestionResponseDto>? Questions { get; init; }
}

/// <summary>
/// DTO de resposta resumido do questionário para listagens (API Layer)
/// </summary>
public class QuestionnaireListResponseDto
{
 /// <summary>
    /// Identificador único do questionário (always returned)
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Título do questionário (always returned)
    /// </summary>
    public string Title { get; init; } = string.Empty;
    
    /// <summary>
    /// Descrição do questionário (always returned)
    /// </summary>
 public string Description { get; init; } = string.Empty;
    
    /// <summary>
    /// Status do questionário: Draft, Published ou Closed (always returned)
    /// </summary>
    public string Status { get; init; } = string.Empty;
    
    /// <summary>
    /// Data de início do período de coleta (optional, null if not published)
    /// </summary>
    public DateTime? CollectionStart { get; init; }
    
    /// <summary>
    /// Data de fim do período de coleta (optional, null if not published)
    /// </summary>
    public DateTime? CollectionEnd { get; init; }
    
    /// <summary>
    /// Número total de questões no questionário (always returned)
    /// </summary>
    public int QuestionCount { get; init; }
    
    /// <summary>
    /// Data e hora de criação em UTC (always returned)
    /// </summary>
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// DTO de resposta da questão (API Layer)
/// </summary>
public class QuestionResponseDto
{
    /// <summary>
    /// Identificador único da questão (always returned)
    /// </summary>
    public Guid Id { get; init; }
    
/// <summary>
    /// Texto da questão (always returned)
    /// </summary>
    public string Text { get; init; } = string.Empty;
    
    /// <summary>
    /// Indica se a resposta é obrigatória (always returned)
    /// </summary>
    public bool IsRequired { get; init; }
    
    /// <summary>
    /// Indica se é uma questão de múltipla escolha (always returned)
    /// </summary>
    public bool IsMultipleChoice { get; init; }
    
    /// <summary>
    /// Data e hora de criação em UTC (always returned)
    /// </summary>
    public DateTime CreatedAt { get; init; }
    
    /// <summary>
    /// Lista de opções de resposta (optional, only for multiple choice questions)
    /// </summary>
    public List<OptionResponseDto>? Options { get; init; }
}

/// <summary>
/// DTO de resposta da opção (API Layer)
/// </summary>
public class OptionResponseDto
{
    /// <summary>
    /// Identificador único da opção (always returned)
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Texto da opção (always returned)
    /// </summary>
    public string Text { get; init; } = string.Empty;
    
    /// <summary>
    /// Ordem de exibição da opção (always returned)
    /// </summary>
    public int Order { get; init; }
}
