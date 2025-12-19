namespace infnet.SurveyQuestionnaire.Application.DTOs.Users;

/// <summary>
/// Request para criar usuário (público ou administrador)
/// </summary>
public record CreateUserRequest(
    string Name,
    string Email,
    string UserType
);

/// <summary>
/// Request para atualizar usuário
/// </summary>
public record UpdateUserRequest(
    string Name,
    string Email
);

/// <summary>
/// Response de usuário
/// </summary>
public record UserResponse(
    Guid Id,
    string Name,
    string Email,
    string UserType,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool IsActive
);
