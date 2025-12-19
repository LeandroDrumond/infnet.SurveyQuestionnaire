using infnet.SurveyQuestionnaire.Domain.Common;
using infnet.SurveyQuestionnaire.Domain.Entities;

namespace infnet.SurveyQuestionnaire.Domain.Specifications;

/// <summary>
/// Specification para buscar questionários publicados
/// </summary>
public class PublishedQuestionnairesSpecification : BaseSpecification<Questionnaire>
{
    public PublishedQuestionnairesSpecification() 
        : base(q => q.Status == QuestionnaireStatus.Published)
    {
        ApplyOrderByDescending(q => q.CreatedAt);
    }
}

/// <summary>
/// Specification para buscar questionários de um criador específico
/// </summary>
public class QuestionnairesByCreatorSpecification : BaseSpecification<Questionnaire>
{
    public QuestionnairesByCreatorSpecification(Guid creatorId)
        : base(q => q.CreatedByUserId == creatorId)
    {
        ApplyOrderByDescending(q => q.CreatedAt);
    }
}

/// <summary>
/// Specification para buscar questionários com questões
/// </summary>
public class QuestionnaireWithQuestionsSpecification : BaseSpecification<Questionnaire>
{
    public QuestionnaireWithQuestionsSpecification(Guid questionnaireId)
      : base(q => q.Id == questionnaireId)
    {
AddInclude(q => q.Questions);
    }
}

/// <summary>
/// Specification para buscar questionários completos (com questões e opções)
/// </summary>
public class QuestionnaireWithFullDetailsSpecification : BaseSpecification<Questionnaire>
{
    public QuestionnaireWithFullDetailsSpecification(Guid questionnaireId)
    : base(q => q.Id == questionnaireId)
    {
        // Nested include usando string
        AddInclude("Questions.Options");
    }
}

/// <summary>
/// Specification para buscar questionários ativos (dentro do período de coleta)
/// </summary>
public class ActiveQuestionnairesSpecification : BaseSpecification<Questionnaire>
{
    public ActiveQuestionnairesSpecification()
      : base(q => 
            q.Status == QuestionnaireStatus.Published &&
   q.CollectionStart.HasValue &&
      q.CollectionEnd.HasValue &&
 DateTime.UtcNow >= q.CollectionStart.Value &&
     DateTime.UtcNow <= q.CollectionEnd.Value)
    {
        ApplyOrderBy(q => q.CollectionEnd); // Ordena por data de término mais próxima
    }
}

/// <summary>
/// Specification para buscar questionários por status
/// </summary>
public class QuestionnairesByStatusSpecification : BaseSpecification<Questionnaire>
{
    public QuestionnairesByStatusSpecification(QuestionnaireStatus status)
     : base(q => q.Status == status)
    {
   ApplyOrderByDescending(q => q.CreatedAt);
    }
}

/// <summary>
/// Specification complexa: questionários publicados de um criador com paginação
/// </summary>
public class PublishedQuestionnairesByCreatorPaginatedSpecification : BaseSpecification<Questionnaire>
{
    public PublishedQuestionnairesByCreatorPaginatedSpecification(
        Guid creatorId, 
        int pageNumber, 
     int pageSize)
 : base(q => 
        q.CreatedByUserId == creatorId && 
        q.Status == QuestionnaireStatus.Published)
    {
        ApplyOrderByDescending(q => q.CreatedAt);
  ApplyThenBy(q => q.Title); // Ordenação secundária por título
 ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
}

/// <summary>
/// Specification para buscar questionários por período de criação
/// </summary>
public class QuestionnairesByDateRangeSpecification : BaseSpecification<Questionnaire>
{
    public QuestionnairesByDateRangeSpecification(DateTime startDate, DateTime endDate)
        : base(q => q.CreatedAt >= startDate && q.CreatedAt <= endDate)
    {
 ApplyOrderByDescending(q => q.CreatedAt);
    }
}

/// <summary>
/// Specification para buscar questionários com título contendo texto
/// </summary>
public class QuestionnairesByTitleSpecification : BaseSpecification<Questionnaire>
{
    public QuestionnairesByTitleSpecification(string searchTerm)
        : base(q => q.Title.Contains(searchTerm))
    {
        ApplyOrderBy(q => q.Title);
    }
}

/// <summary>
/// Specification combinada: questionários publicados E ativos (exemplo de combinação)
/// </summary>
public class PublishedAndActiveQuestionnairesSpecification : BaseSpecification<Questionnaire>
{
    public PublishedAndActiveQuestionnairesSpecification()
    {
        // Usa o método And para combinar specifications
        var publishedSpec = new PublishedQuestionnairesSpecification();
      var activeSpec = new ActiveQuestionnairesSpecification();
        
    And(publishedSpec);
        And(activeSpec);
    }
}
