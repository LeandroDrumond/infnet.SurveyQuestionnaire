using infnet.SurveyQuestionnaire.Domain.Entities;

namespace infnet.SurveyQuestionnaire.Domain.Repositories;

/// <summary>
/// Repositório para o agregado Questionnaire
/// </summary>
public interface IQuestionnaireRepository
{
    Task<Questionnaire?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Questionnaire?> GetByIdWithQuestionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Questionnaire>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Questionnaire>> GetByCreatorIdAsync(Guid createdByUserId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Questionnaire>> GetPublishedAsync(CancellationToken cancellationToken = default);

    void Add(Questionnaire questionnaire);
    void Update(Questionnaire questionnaire);
    void Remove(Questionnaire questionnaire);
}
