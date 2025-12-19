using infnet.SurveyQuestionnaire.Application.DTOs.Users;
using infnet.SurveyQuestionnaire.Application.Interfaces;
using infnet.SurveyQuestionnaire.Domain.Common;
using infnet.SurveyQuestionnaire.Domain.Entities;
using infnet.SurveyQuestionnaire.Domain.Exceptions;
using infnet.SurveyQuestionnaire.Domain.Repositories;
using infnet.SurveyQuestionnaire.Domain.Users;

namespace infnet.SurveyQuestionnaire.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Cria um usuário (público ou administrador)
    /// </summary>
    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
    {
        // Verificar se email já existe (método direto, sem Specification)
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            throw new UserAlreadyExistsException(request.Email);
        }

        // Criar usuário baseado no tipo
        User user = request.UserType.ToLowerInvariant() switch
        {
            "administrator" => User.CreateAdministrator(request.Name, request.Email),
            "public" => User.CreatePublicUser(request.Name, request.Email),
            _ => throw new ArgumentException($"Invalid user type: {request.UserType}")
        };

        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(user);
    }

    public async Task<UserResponse> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        return user == null ? throw new UserNotFoundException(id) : MapToResponse(user);
    }

    public async Task<UserResponse?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        return user == null ? null : MapToResponse(user);
    }

    public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(MapToResponse);
    }

    public async Task<UserResponse> UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id) ?? throw new UserNotFoundException(id);

        // Verificar se o novo email já existe (exceto para o próprio usuário)
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);

        if (existingUser != null && existingUser.Id != id)
        {
            throw new UserAlreadyExistsException(request.Email);
        }

        user.Update(request.Name, request.Email);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(user);
    }

    public async Task DeactivateUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            throw new UserNotFoundException(id);
        }

        user.Deactivate();

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ActivateUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            throw new UserNotFoundException(id);
        }

        user.Activate();

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    private static UserResponse MapToResponse(User user)
    {
        return new UserResponse(
            user.Id,
            user.Name,
            user.Email.Value,
            user.UserType.ToString(),
            user.CreatedAt,
            user.UpdatedAt,
            user.IsActive
        );
    }
}
