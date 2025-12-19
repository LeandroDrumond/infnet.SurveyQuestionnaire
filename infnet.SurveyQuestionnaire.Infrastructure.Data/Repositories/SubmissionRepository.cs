using infnet.SurveyQuestionnaire.Domain.Entities;
using infnet.SurveyQuestionnaire.Domain.Repositories;
using infnet.SurveyQuestionnaire.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace infnet.SurveyQuestionnaire.Infrastructure.Data.Repositories;

public class SubmissionRepository : ISubmissionRepository
{
    private readonly SurveyQuestionnaireDbContext _context;

    public SubmissionRepository(SurveyQuestionnaireDbContext context)
    {
   _context = context;
    }

    public async Task<Submission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Submissions.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Submission?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Submissions.Include(s => s.Items)
                                        .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Submission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
      return await _context.Submissions.OrderByDescending(s => s.SubmittedAt)
                                       .ToListAsync(cancellationToken);
    }

  public async Task<IEnumerable<Submission>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
  {
        return await _context.Submissions.Where(s => s.RespondentUserId == userId)
                                         .OrderByDescending(s => s.SubmittedAt)
                                         .ToListAsync(cancellationToken);
  }

  public async Task<IEnumerable<Submission>> GetByQuestionnaireIdAsync(Guid questionnaireId, CancellationToken cancellationToken = default)
  {
        return await _context.Submissions.Where(s => s.QuestionnaireId == questionnaireId)
                                         .OrderByDescending(s => s.SubmittedAt)
                                         .ToListAsync(cancellationToken);
  }

    public async Task<bool> HasUserSubmittedAsync(Guid questionnaireId, Guid userId, CancellationToken cancellationToken = default)
    {
      return await _context.Submissions.AnyAsync(s => s.QuestionnaireId == questionnaireId && s.RespondentUserId == userId, cancellationToken);
    }

    public async Task<int> CountByQuestionnaireIdAsync(Guid questionnaireId, CancellationToken cancellationToken = default)
    {
        return await _context.Submissions.CountAsync(s => s.QuestionnaireId == questionnaireId, cancellationToken);
    }

    public void Add(Submission submission)
    {
        _context.Submissions.Add(submission);
    }

    public void Update(Submission submission)
    {
        _context.Submissions.Update(submission);
    }

    public void Remove(Submission submission)
    {
        _context.Submissions.Remove(submission);
    }
}
