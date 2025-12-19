namespace infnet.SurveyQuestionnaire.Domain.Users;

/// <summary>
/// Tipos de usuário no sistema
/// </summary>
public enum UserType
{
    /// <summary>
    /// Usuário público - pode responder questionários
    /// </summary>
    Public = 0,

    /// <summary>
    /// Administrador - pode criar e gerenciar questionários
    /// </summary>
    Administrator = 1
}
