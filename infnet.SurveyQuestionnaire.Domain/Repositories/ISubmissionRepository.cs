using infnet.SurveyQuestionnaire.Domain.Common;
using infnet.SurveyQuestionnaire.Domain.Entities;

namespace infnet.SurveyQuestionnaire.Domain.Repositories;

/// <summary>
/// Interface do repositório de submissions (respostas de questionários)
/// </summary>
public interface ISubmissionRepository
{
    // ==================== Queries Básicas ====================
    
    /// <summary>
    /// Busca uma submission por ID
    /// </summary>
    Task<Submission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca uma submission por ID incluindo os items
    /// </summary>
 Task<Submission?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca todas as submissions
    /// </summary>
    Task<IEnumerable<Submission>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca submissions de um usuário específico
    /// </summary>
 Task<IEnumerable<Submission>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

  /// <summary>
    /// Busca submissions de um questionário específico
    /// </summary>
    Task<IEnumerable<Submission>> GetByQuestionnaireIdAsync(Guid questionnaireId, CancellationToken cancellationToken = default);

    // ==================== Verificações ====================
    
    /// <summary>
    /// Verifica se usuário já respondeu um questionário
    /// </summary>
    Task<bool> HasUserSubmittedAsync(Guid questionnaireId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Conta quantas submissions um questionário tem
    /// </summary>
    Task<int> CountByQuestionnaireIdAsync(Guid questionnaireId, CancellationToken cancellationToken = default);

    // ==================== Queries com Specification ====================
    
    /// <summary>
    /// Busca submission com specification
    /// </summary>
    Task<Submission?> GetBySpecAsync(ISpecification<Submission> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca múltiplas submissions com specification
 /// </summary>
    Task<IEnumerable<Submission>> GetAllBySpecAsync(ISpecification<Submission> specification, CancellationToken cancellationToken = default);

 /// <summary>
    /// Conta submissions com specification
    /// </summary>
    Task<int> CountAsync(ISpecification<Submission> specification, CancellationToken cancellationToken = default);

    // ==================== Comandos ====================
 
    /// <summary>
    /// Adiciona uma nova submission
    /// </summary>
  void Add(Submission submission);

    /// <summary>
    /// Atualiza uma submission existente
    /// </summary>
    void Update(Submission submission);

  /// <summary>
    /// Remove uma submission
    /// </summary>
    void Remove(Submission submission);
}
