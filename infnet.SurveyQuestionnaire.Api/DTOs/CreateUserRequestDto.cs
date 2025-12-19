using System.ComponentModel.DataAnnotations;

namespace infnet.SurveyQuestionnaire.Api.DTOs;

/// <summary>
/// DTO para requisição de criação de usuário na API
/// </summary>
public class CreateUserRequestDto
{
    /// <summary>
    /// Nome completo do usuário
    /// </summary>
    /// <example>John Doe</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuário (deve ser único no sistema)
    /// </summary>
    /// <example>john.doe@example.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Tipo do usuário (Public ou Administrator)
    /// </summary>
    /// <example>Public</example>
    public string UserType { get; set; } = "Public";
}

/// <summary>
/// DTO para requisição de atualização de usuário na API
/// </summary>
public class UpdateUserRequestDto
{
    /// <summary>
    /// Nome completo do usuário
    /// </summary>
    /// <example>John Doe Updated</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuário (deve ser único no sistema)
    /// </summary>
    /// <example>john.updated@example.com</example>
    public string Email { get; set; } = string.Empty;
}
