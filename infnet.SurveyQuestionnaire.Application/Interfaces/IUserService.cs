using infnet.SurveyQuestionnaire.Application.DTOs.Users;

namespace infnet.SurveyQuestionnaire.Application.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Cria um usuário (público ou administrador)
    /// </summary>
    Task<UserResponse> CreateUserAsync(CreateUserRequest request);

    Task<UserResponse> GetUserByIdAsync(Guid id);
    Task<UserResponse?> GetUserByEmailAsync(string email);
    Task<IEnumerable<UserResponse>> GetAllUsersAsync();
    Task<UserResponse> UpdateUserAsync(Guid id, UpdateUserRequest request);
    Task DeactivateUserAsync(Guid id);
    Task ActivateUserAsync(Guid id);
}
