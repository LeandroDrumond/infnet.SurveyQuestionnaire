namespace infnet.SurveyQuestionnaire.Api.Models;

/// <summary>
/// Modelo de resposta de erro padronizado seguindo RFC 7807 (Problem Details)
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Título do erro
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Código de status HTTP
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Dicionário de erros de validação (campo -> mensagens)
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    /// Mensagem detalhada do erro (opcional)
    /// </summary>
    public string? Detail { get; set; }

    /// <summary>
    /// URI que identifica o tipo do problema (opcional)
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// ID do trace para rastreamento (opcional)
    /// </summary>
    public string? TraceId { get; set; }
}
