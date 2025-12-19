using infnet.SurveyQuestionnaire.Domain.Common;
using infnet.SurveyQuestionnaire.Domain.Entities;

namespace infnet.SurveyQuestionnaire.Domain.Repositories;

/// <summary>
/// Repositório para o agregado Questionnaire
/// </summary>
public interface IQuestionnaireRepository
{
    // Queries básicas
    Task<Questionnaire?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Questionnaire?> GetByIdWithQuestionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Questionnaire>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Questionnaire>> GetByCreatorIdAsync(Guid createdByUserId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Questionnaire>> GetPublishedAsync(CancellationToken cancellationToken = default);
    
    // Queries com Specification (para queries complexas)
    Task<Questionnaire?> GetBySpecAsync(ISpecification<Questionnaire> specification, CancellationToken cancellationToken = default);
    Task<IEnumerable<Questionnaire>> GetAllBySpecAsync(ISpecification<Questionnaire> specification, CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecification<Questionnaire> specification, CancellationToken cancellationToken = default);

    // Comandos
    void Add(Questionnaire questionnaire);
    void Update(Questionnaire questionnaire);
    void Remove(Questionnaire questionnaire);
}
