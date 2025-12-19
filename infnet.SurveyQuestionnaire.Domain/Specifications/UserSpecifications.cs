using infnet.SurveyQuestionnaire.Domain.Common;
using infnet.SurveyQuestionnaire.Domain.Entities;
using infnet.SurveyQuestionnaire.Domain.Users;

namespace infnet.SurveyQuestionnaire.Domain.Specifications;

/// <summary>
/// Specification para buscar usuários ativos
/// </summary>
public class ActiveUsersSpecification : BaseSpecification<User>
{
    public ActiveUsersSpecification()
        : base(u => u.IsActive)
    {
        ApplyOrderBy(u => u.Name);
    }
}

/// <summary>
/// Specification para buscar usuários administradores
/// </summary>
public class AdministratorUsersSpecification : BaseSpecification<User>
{
    public AdministratorUsersSpecification()
        : base(u => u.UserType == UserType.Administrator)
    {
        ApplyOrderBy(u => u.Name);
    }
}

/// <summary>
/// Specification para buscar usuários por email domain
/// </summary>
public class UsersByEmailDomainSpecification : BaseSpecification<User>
{
    public UsersByEmailDomainSpecification(string domain)
      : base(u => u.Email.Value.EndsWith($"@{domain}"))
    {
        ApplyOrderBy(u => u.Email.Value);
    }
}

/// <summary>
/// Specification complexa: administradores ativos
/// </summary>
public class ActiveAdministratorsSpecification : BaseSpecification<User>
{
    public ActiveAdministratorsSpecification()
        : base(u => u.IsActive && u.UserType == UserType.Administrator)
    {
   ApplyOrderBy(u => u.Name);
    }
}

/// <summary>
/// Specification para buscar usuários criados em um período
/// </summary>
public class UsersByDateRangeSpecification : BaseSpecification<User>
{
    public UsersByDateRangeSpecification(DateTime startDate, DateTime endDate)
    : base(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate)
    {
 ApplyOrderByDescending(u => u.CreatedAt);
    }
}

/// <summary>
/// Specification para buscar usuários por nome (contém)
/// </summary>
public class UsersByNameSpecification : BaseSpecification<User>
{
    public UsersByNameSpecification(string searchTerm)
: base(u => u.Name.Contains(searchTerm))
    {
   ApplyOrderBy(u => u.Name);
    }
}

/// <summary>
/// Specification com paginação: usuários públicos ativos
/// </summary>
public class ActivePublicUsersPaginatedSpecification : BaseSpecification<User>
{
    public ActivePublicUsersPaginatedSpecification(int pageNumber, int pageSize)
 : base(u => u.IsActive && u.UserType == UserType.Public)
    {
        ApplyOrderBy(u => u.Name);
    ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
}
