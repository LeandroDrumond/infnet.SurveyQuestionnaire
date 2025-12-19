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
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuário (deve ser único no sistema)
    /// </summary>
    /// <example>john.doe@example.com</example>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Tipo do usuário (Public ou Administrator)
    /// </summary>
    /// <example>Public</example>
    [Required(ErrorMessage = "UserType is required")]
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
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuário (deve ser único no sistema)
    /// </summary>
    /// <example>john.updated@example.com</example>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
    public string Email { get; set; } = string.Empty;
}
