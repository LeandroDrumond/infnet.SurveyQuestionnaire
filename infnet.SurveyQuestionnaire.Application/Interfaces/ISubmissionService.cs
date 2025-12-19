using infnet.SurveyQuestionnaire.Application.DTOs.Submissions;

namespace infnet.SurveyQuestionnaire.Application.Interfaces;

/// <summary>
/// Interface do serviço de submissions (respostas de questionários)
/// </summary>
public interface ISubmissionService
{
    // ==================== Submission Operations ====================

    /// <summary>
    /// Cria uma nova submission e envia para processamento assíncrono
    /// </summary>
/// <param name="request">Dados da submission</param>
    /// <param name="userId">ID do usuário respondente (deve ser usuário público)</param>
/// <returns>Submission criada com status Pending</returns>
    Task<SubmissionResponse> CreateSubmissionAsync(CreateSubmissionRequest request, Guid userId);

    /// <summary>
    /// Busca uma submission por ID
    /// </summary>
    /// <param name="id">ID da submission</param>
    /// <param name="userId">ID do usuário (para validar autorização)</param>
    /// <returns>Submission encontrada</returns>
    Task<SubmissionResponse> GetSubmissionByIdAsync(Guid id, Guid userId);

    /// <summary>
    /// Busca uma submission por ID com todos os items
    /// </summary>
    /// <param name="id">ID da submission</param>
    /// <param name="userId">ID do usuário (para validar autorização)</param>
    /// <returns>Submission com items</returns>
  Task<SubmissionResponse> GetSubmissionWithItemsAsync(Guid id, Guid userId);

    /// <summary>
    /// Lista todas as submissions de um questionário (apenas para admin ou criador)
    /// </summary>
    /// <param name="questionnaireId">ID do questionário</param>
    /// <param name="userId">ID do usuário (admin ou criador)</param>
    /// <returns>Lista de submissions do questionário</returns>
  Task<IEnumerable<SubmissionListResponse>> GetQuestionnaireSubmissionsAsync(Guid questionnaireId, Guid userId);

    /// <summary>
    /// Conta quantas submissions um questionário tem (apenas para admin ou criador)
    /// </summary>
    /// <param name="questionnaireId">ID do questionário</param>
    /// <param name="userId">ID do usuário (admin ou criador)</param>
    /// <returns>Número de submissions</returns>
    Task<int> CountQuestionnaireSubmissionsAsync(Guid questionnaireId, Guid userId);

    // ==================== Processing Operations (usado pela Azure Function) ====================

    /// <summary>
    /// Processa uma submission (usado pela Azure Function)
    /// </summary>
    /// <param name="message">Mensagem do Service Bus</param>
    Task ProcessSubmissionAsync(SubmissionMessage message);
}
