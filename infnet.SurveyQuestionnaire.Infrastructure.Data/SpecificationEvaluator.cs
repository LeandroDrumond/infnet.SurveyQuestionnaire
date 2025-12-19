using infnet.SurveyQuestionnaire.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace infnet.SurveyQuestionnaire.Infrastructure.Data;

/// <summary>
/// Avalia e aplica specifications em queries do Entity Framework Core
/// Converte ISpecification em IQueryable aplicando filtros, includes, ordenação e paginação
/// </summary>
public static class SpecificationEvaluator
{
  /// <summary>
    /// Aplica a specification em uma query do EF Core
    /// </summary>
    /// <typeparam name="T">Tipo da entidade</typeparam>
    /// <param name="inputQuery">Query inicial do EF Core</param>
    /// <param name="specification">Specification a ser aplicada</param>
    /// <returns>Query com a specification aplicada</returns>
    public static IQueryable<T> GetQuery<T>(
        IQueryable<T> inputQuery,
      ISpecification<T> specification) where T : class
    {
        var query = inputQuery;

        // 1. Aplicar filtro (WHERE clause)
      if (specification.Criteria != null)
        {
      query = query.Where(specification.Criteria);
        }

        // 2. Aplicar Includes (eager loading) - Expressões
        query = specification.Includes.Aggregate(
            query,
  (current, include) => current.Include(include));

    // 3. Aplicar Includes (eager loading) - Strings (para nested includes)
        query = specification.IncludeStrings.Aggregate(
            query,
      (current, include) => current.Include(include));

     // 4. Aplicar ordenação principal
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
       query = query.OrderByDescending(specification.OrderByDescending);
        }

    // 5. Aplicar ordenações secundárias (ThenBy)
   if (specification.ThenByExpressions.Any())
        {
   IOrderedQueryable<T>? orderedQuery = query as IOrderedQueryable<T>;
            
 if (orderedQuery != null)
   {
                foreach (var thenBy in specification.ThenByExpressions)
        {
    orderedQuery = thenBy.Descending
          ? orderedQuery.ThenByDescending(thenBy.KeySelector)
      : orderedQuery.ThenBy(thenBy.KeySelector);
        }
    
       query = orderedQuery;
            }
  }

      // 6. Aplicar paginação (deve ser o último)
        if (specification.IsPagingEnabled)
    {
        query = query.Skip(specification.Skip)
      .Take(specification.Take);
        }

        return query;
    }

 /// <summary>
    /// Conta o número de registros que atendem a specification (ignora paginação)
    /// </summary>
    public static IQueryable<T> GetQueryForCount<T>(
IQueryable<T> inputQuery,
        ISpecification<T> specification) where T : class
    {
        var query = inputQuery;

        // Aplica apenas o critério de filtro (ignora includes, ordenação e paginação)
if (specification.Criteria != null)
        {
       query = query.Where(specification.Criteria);
        }

        return query;
    }
}
