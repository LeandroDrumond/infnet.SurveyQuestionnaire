using infnet.SurveyQuestionnaire.Domain.Common;
using infnet.SurveyQuestionnaire.Infrastructure.Data.Context;

namespace infnet.SurveyQuestionnaire.Infrastructure.Data;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly SurveyQuestionnaireDbContext _context;

    public UnitOfWork(SurveyQuestionnaireDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
