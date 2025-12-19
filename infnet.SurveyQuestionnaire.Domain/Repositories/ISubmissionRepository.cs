using infnet.SurveyQuestionnaire.Domain.Entities;

namespace infnet.SurveyQuestionnaire.Domain.Repositories;

public interface ISubmissionRepository
{
    Task<Submission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Submission?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Submission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Submission>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Submission>> GetByQuestionnaireIdAsync(Guid questionnaireId, CancellationToken cancellationToken = default);
    Task<bool> HasUserSubmittedAsync(Guid questionnaireId, Guid userId, CancellationToken cancellationToken = default);
    Task<int> CountByQuestionnaireIdAsync(Guid questionnaireId, CancellationToken cancellationToken = default);
    void Add(Submission submission);
    void Update(Submission submission);
    void Remove(Submission submission);
}
