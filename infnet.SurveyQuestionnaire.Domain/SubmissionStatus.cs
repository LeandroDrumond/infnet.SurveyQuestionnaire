namespace infnet.SurveyQuestionnaire.Domain;

/// <summary>
/// Status da submissão (resposta do questionário)
/// </summary>
public enum SubmissionStatus
{
    /// <summary>
    /// Enviado para fila, aguardando processamento
    /// </summary>
    Pending = 0,
 
    /// <summary>
    /// Sendo processado pela Azure Function
    /// </summary>
    Processing = 1,
    
    /// <summary>
    /// Processado e salvo com sucesso
    /// </summary>
    Completed = 2,
    
    /// <summary>
    /// Falha no processamento
    /// </summary>
    Failed = 3
}
