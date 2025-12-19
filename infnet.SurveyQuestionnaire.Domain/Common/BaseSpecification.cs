using System.Linq.Expressions;

namespace infnet.SurveyQuestionnaire.Domain.Common;

/// <summary>
/// Classe base para implementar Specifications
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public abstract class BaseSpecification<T> : ISpecification<T> where T : class
{
    protected BaseSpecification()
    {
    }

    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    public Expression<Func<T, bool>>? Criteria { get; private set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }
    public int Skip { get; private set; }
    public int Take { get; private set; }
    public bool IsPagingEnabled { get; private set; }
    public List<(Expression<Func<T, object>> KeySelector, bool Descending)> ThenByExpressions { get; } = new();

    /// <summary>
    /// Adiciona critério de filtro (WHERE)
    /// </summary>
    protected void AddCriteria(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// Adiciona include para eager loading
    /// </summary>
    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

  /// <summary>
    /// Adiciona include usando string (para nested includes)
    /// Exemplo: AddInclude("Questionnaires.Questions.Options")
    /// </summary>
    protected void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    /// <summary>
    /// Define ordenação ascendente
    /// </summary>
  protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
 OrderBy = orderByExpression;
    }

    /// <summary>
    /// Define ordenação descendente
    /// </summary>
    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }

    /// <summary>
    /// Adiciona ordenação secundária (ThenBy)
    /// </summary>
    protected void ApplyThenBy(Expression<Func<T, object>> thenByExpression, bool descending = false)
    {
        ThenByExpressions.Add((thenByExpression, descending));
    }

    /// <summary>
    /// Aplica paginação
    /// </summary>
  /// <param name="skip">Número de registros a pular</param>
    /// <param name="take">Número de registros a retornar</param>
    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    /// <summary>
    /// Combina este specification com outro usando AND
    /// </summary>
    public ISpecification<T> And(ISpecification<T> specification)
  {
        if (Criteria == null)
      {
 Criteria = specification.Criteria;
        }
        else if (specification.Criteria != null)
        {
            var parameter = Expression.Parameter(typeof(T));
    var combined = Expression.Lambda<Func<T, bool>>(
             Expression.AndAlso(
           Expression.Invoke(Criteria, parameter),
         Expression.Invoke(specification.Criteria, parameter)
       ),
parameter
);
  Criteria = combined;
        }

        return this;
  }

    /// <summary>
 /// Combina este specification com outro usando OR
    /// </summary>
    public ISpecification<T> Or(ISpecification<T> specification)
    {
        if (Criteria == null)
        {
 Criteria = specification.Criteria;
        }
        else if (specification.Criteria != null)
        {
      var parameter = Expression.Parameter(typeof(T));
    var combined = Expression.Lambda<Func<T, bool>>(
         Expression.OrElse(
        Expression.Invoke(Criteria, parameter),
         Expression.Invoke(specification.Criteria, parameter)
 ),
 parameter
    );
  Criteria = combined;
        }

return this;
    }
}
