using infnet.SurveyQuestionnaire.Domain.Common;
using infnet.SurveyQuestionnaire.Domain.Entities;
using infnet.SurveyQuestionnaire.Domain.Repositories;
using infnet.SurveyQuestionnaire.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace infnet.SurveyQuestionnaire.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SurveyQuestionnaireDbContext _context;

    public UserRepository(SurveyQuestionnaireDbContext context)
    {
        _context = context;
    }

    // Métodos básicos (mantidos para compatibilidade)
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FindAsync([id], cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Value == normalizedEmail, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .OrderBy(u => u.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await _context.Users.AnyAsync(u => u.Email.Value == normalizedEmail, cancellationToken);
    }

    // Métodos com Specification (para queries complexas)
    public async Task<User?> GetBySpecAsync(ISpecification<User> specification, CancellationToken cancellationToken = default)
    {
        return await SpecificationEvaluator
            .GetQuery(_context.Users.AsQueryable(), specification)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllBySpecAsync(ISpecification<User> specification, CancellationToken cancellationToken = default)
    {
        return await SpecificationEvaluator
            .GetQuery(_context.Users.AsQueryable(), specification)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<User> specification, CancellationToken cancellationToken = default)
    {
        return await SpecificationEvaluator
            .GetQueryForCount(_context.Users.AsQueryable(), specification)
            .CountAsync(cancellationToken);
    }

    // Comandos
    public void Add(User user)
    {
        _context.Users.Add(user);
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
    }

    public void Remove(User user)
    {
        _context.Users.Remove(user);
    }
}
