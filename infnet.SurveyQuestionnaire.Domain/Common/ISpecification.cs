using System.Linq.Expressions;

namespace infnet.SurveyQuestionnaire.Domain.Common;

/// <summary>
/// Interface base para Specification Pattern
/// Permite encapsular lógica de consulta complexa e reutilizável
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public interface ISpecification<T> where T : class
{
    /// <summary>
    /// Critério de filtro (WHERE clause)
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
 /// Lista de includes para eager loading
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// Lista de includes de strings para nested includes
 /// Exemplo: "Questionnaires.Questions.Options"
    /// </summary>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// Ordenação ascendente
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// Ordenação descendente
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// Paginação: número de registros a pular
    /// </summary>
    int Skip { get; }

    /// <summary>
    /// Paginação: número de registros a retornar
    /// </summary>
    int Take { get; }

    /// <summary>
    /// Indica se a paginação está habilitada
    /// </summary>
    bool IsPagingEnabled { get; }

    /// <summary>
    /// Lista de ordenações adicionais (ThenBy)
    /// </summary>
  List<(Expression<Func<T, object>> KeySelector, bool Descending)> ThenByExpressions { get; }
}
