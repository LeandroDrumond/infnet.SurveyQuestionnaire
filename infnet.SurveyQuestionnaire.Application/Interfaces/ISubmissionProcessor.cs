using infnet.SurveyQuestionnaire.Application.DTOs.Submissions;

namespace infnet.SurveyQuestionnaire.Application.Interfaces;

/// <summary>
/// Interface para processar submissions
/// </summary>
public interface ISubmissionProcessor
{
    /// <summary>
    /// Processa uma mensagem de submission de forma assíncrona
    /// </summary>
    /// <param name="message">Mensagem com dados da submission</param>
    Task ProcessSubmissionAsync(SubmissionMessage message);
}
