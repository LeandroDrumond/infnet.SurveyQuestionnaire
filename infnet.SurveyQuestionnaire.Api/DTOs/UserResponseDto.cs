namespace infnet.SurveyQuestionnaire.Api.DTOs;

/// <summary>
/// DTO para resposta de usuário na API
/// </summary>
public class UserResponseDto
{
    /// <summary>
    /// Identificador único do usuário (always returned)
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome completo do usuário (always returned)
    /// </summary>
    /// <example>John Doe</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuário (always returned)
    /// </summary>
    /// <example>john.doe@example.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Tipo do usuário: Public ou Administrator (always returned)
    /// </summary>
    /// <example>Administrator</example>
    public string UserType { get; set; } = string.Empty;

    /// <summary>
    /// Data e hora de criação do usuário em UTC (always returned)
    /// </summary>
    /// <example>2025-01-15T10:30:00Z</example>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data e hora da última atualização do usuário em UTC (optional, may be null if never updated)
    /// </summary>
    /// <example>2025-01-15T15:45:00Z</example>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indica se o usuário está ativo no sistema (always returned)
    /// </summary>
    /// <example>true</example>
    public bool IsActive { get; set; }
}
