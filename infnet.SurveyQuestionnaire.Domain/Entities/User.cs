using infnet.SurveyQuestionnaire.Domain.Common;
using infnet.SurveyQuestionnaire.Domain.Exceptions;
using infnet.SurveyQuestionnaire.Domain.Users;
using infnet.SurveyQuestionnaire.Domain.Users.ValueObjects;

namespace infnet.SurveyQuestionnaire.Domain.Entities;

public sealed class User : Entity
{
    private const int _nameMinLength = 3;
    private const int _nameMaxLength = 100;

    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; }
    public UserType UserType { get; private set; }
    public bool IsActive { get; private set; }

    // EF Constructor
    private User() {
        Email = null!;
    }

    private User(string name, Email email, UserType userType) : base() {
        SetName(name);
        Email = email;
        UserType = userType;
        IsActive = true;
    }

    /// <summary>
    /// Cria um usuário administrador
    /// </summary>
    public static User CreateAdministrator(string name, string email) {
        var userEmail = Email.Create(email);
        return new User(name, userEmail, UserType.Administrator);
    }

    /// <summary>
    /// Cria um usuário público
    /// </summary>
    public static User CreatePublicUser(string name, string email) {
        var userEmail = Email.Create(email);
        return new User(name, userEmail, UserType.Public);
    }

    public void Update(string name, string email) {
        SetName(name);
        Email = Email.Create(email);
        SetUpdatedAt();
    }

    public void Deactivate() {
        if (!IsActive) {
            throw new UserAlreadyInactiveException(Id);
        }

        IsActive = false;
        SetUpdatedAt();
    }

    public void Activate() {
        if (IsActive) {
            throw new UserAlreadyActiveException(Id);
        }

        IsActive = true;
        SetUpdatedAt();
    }

    /// <summary>
    /// Verifica se o usuário é administrador
    /// </summary>
    public bool IsAdministrator() => UserType == UserType.Administrator;

    /// <summary>
    /// Verifica se o usuário é público
    /// </summary>
    public bool IsPublicUser() => UserType == UserType.Public;

    /// <summary>
    /// Valida e define o nome do usuário
    /// </summary>
    private void SetName(string name) {
        if (string.IsNullOrWhiteSpace(name)) {
            throw new ArgumentException("Name cannot be empty", nameof(name));
        }

        var trimmedName = name.Trim();

        if (trimmedName.Length < _nameMinLength) {
            throw new ArgumentException($"Name must be at least {_nameMinLength} characters", nameof(name));
        }

        if (trimmedName.Length > _nameMaxLength) {
            throw new ArgumentException($"Name cannot exceed {_nameMaxLength} characters", nameof(name));
        }

        Name = trimmedName;
    }
}
