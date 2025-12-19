using infnet.SurveyQuestionnaire.Domain.Entities;

namespace infnet.SurveyQuestionnaire.Domain.Repositories;

/// <summary>
/// Interface do repositório de usuários
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    void Add(User user);
    void Update(User user);
    void Remove(User user);
}
