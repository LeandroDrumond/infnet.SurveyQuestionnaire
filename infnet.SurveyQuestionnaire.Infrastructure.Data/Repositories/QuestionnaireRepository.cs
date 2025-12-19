using infnet.SurveyQuestionnaire.Domain;
using infnet.SurveyQuestionnaire.Domain.Common;
using infnet.SurveyQuestionnaire.Domain.Entities;
using infnet.SurveyQuestionnaire.Domain.Repositories;
using infnet.SurveyQuestionnaire.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace infnet.SurveyQuestionnaire.Infrastructure.Data.Repositories;

public class QuestionnaireRepository : IQuestionnaireRepository
{
    private readonly SurveyQuestionnaireDbContext _context;

    public QuestionnaireRepository(SurveyQuestionnaireDbContext context)
  {
  _context = context;
    }

    // Métodos básicos (mantidos para compatibilidade)
    public async Task<Questionnaire?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
   return await _context.Questionnaires.FindAsync([id], cancellationToken);
    }

    public async Task<Questionnaire?> GetByIdWithQuestionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Questionnaires
  .Include(q => q.Questions)
     .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
 }

    public async Task<IEnumerable<Questionnaire>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Questionnaires
.OrderByDescending(q => q.CreatedAt)
  .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Questionnaire>> GetByCreatorIdAsync(Guid createdByUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Questionnaires
    .Where(q => q.CreatedByUserId == createdByUserId)
        .OrderByDescending(q => q.CreatedAt)
     .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Questionnaire>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
      return await _context.Questionnaires
.Where(q => q.Status == QuestionnaireStatus.Published)
   .OrderByDescending(q => q.CreatedAt)
        .ToListAsync(cancellationToken);
    }

    // Métodos com Specification (para queries complexas)
    public async Task<Questionnaire?> GetBySpecAsync(ISpecification<Questionnaire> specification, CancellationToken cancellationToken = default)
    {
        return await SpecificationEvaluator
  .GetQuery(_context.Questionnaires.AsQueryable(), specification)
      .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Questionnaire>> GetAllBySpecAsync(ISpecification<Questionnaire> specification, CancellationToken cancellationToken = default)
    {
  return await SpecificationEvaluator
            .GetQuery(_context.Questionnaires.AsQueryable(), specification)
      .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<Questionnaire> specification, CancellationToken cancellationToken = default)
    {
    return await SpecificationEvaluator
            .GetQueryForCount(_context.Questionnaires.AsQueryable(), specification)
          .CountAsync(cancellationToken);
    }

    // Comandos
    public void Add(Questionnaire questionnaire)
    {
   _context.Questionnaires.Add(questionnaire);
    }

    public void Update(Questionnaire questionnaire)
    {
        _context.Questionnaires.Update(questionnaire);
  }

    public void Remove(Questionnaire questionnaire)
    {
      _context.Questionnaires.Remove(questionnaire);
    }
}
