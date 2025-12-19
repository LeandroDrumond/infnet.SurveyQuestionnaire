using infnet.SurveyQuestionnaire.Domain;
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

    public async Task<Questionnaire?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Questionnaires.FindAsync([id], cancellationToken);
    }

    public async Task<Questionnaire?> GetByIdWithQuestionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Questionnaires
                    .AsNoTracking()
                    .Include(q => q.Questions)
                    .ThenInclude(q => q.Options)
                    .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Questionnaire>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Questionnaires.OrderByDescending(q => q.CreatedAt)
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

    public void Add(Questionnaire questionnaire)
    {
        _context.Questionnaires.Add(questionnaire);
    }

    public void Update(Questionnaire questionnaire)
    {
      
        var existingQuestionnaire = _context.Questionnaires
                                    .AsNoTracking()
                                    .Include(q => q.Questions)
                                    .ThenInclude(q => q.Options)
                                    .FirstOrDefault(q => q.Id == questionnaire.Id) ?? throw new InvalidOperationException($"Questionnaire {questionnaire.Id} not found.");
           
            _context.Questionnaires.Update(questionnaire);
            _context.Entry(questionnaire).State = EntityState.Modified;

        var existingQuestionIds = existingQuestionnaire.Questions.Select(q => q.Id).ToHashSet();
        var existingOptionIds = existingQuestionnaire.Questions.SelectMany(q => q.Options).Select(o => o.Id).ToHashSet();

                 foreach (var question in questionnaire.Questions)
                 {
                         if (existingQuestionIds.Contains(question.Id))
                         {
                        
                          _context.Entry(question).State = EntityState.Modified;
                         }               
                        else
                        {
        
                          _context.Entry(question).State = EntityState.Added;
                        }

            
                    foreach (var option in question.Options)
                    {
                        if (existingOptionIds.Contains(option.Id))
                        {
  
                             _context.Entry(option).State = EntityState.Modified;
                        }
                        else
                        {
                            _context.Entry(option).State = EntityState.Added;
                        }
                    }
                 }
    }

    public void Remove(Questionnaire questionnaire)
    {
        _context.Questionnaires.Remove(questionnaire);
    }
}
