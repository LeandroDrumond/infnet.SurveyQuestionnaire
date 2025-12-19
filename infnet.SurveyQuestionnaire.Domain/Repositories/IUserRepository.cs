using infnet.SurveyQuestionnaire.Domain.Common;
using infnet.SurveyQuestionnaire.Domain.Entities;

namespace infnet.SurveyQuestionnaire.Domain.Repositories;

/// <summary>
/// Interface do repositório de usuários
/// </summary>
public interface IUserRepository
{
    // Queries básicas
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);

    // Verificações
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    // Queries com Specification (para queries complexas)
    Task<User?> GetBySpecAsync(ISpecification<User> specification, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAllBySpecAsync(ISpecification<User> specification, CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecification<User> specification, CancellationToken cancellationToken = default);

    // Comandos
    void Add(User user);
    void Update(User user);
    void Remove(User user);
}
