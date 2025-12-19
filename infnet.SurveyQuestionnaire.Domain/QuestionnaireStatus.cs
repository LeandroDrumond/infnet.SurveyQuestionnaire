namespace infnet.SurveyQuestionnaire.Domain;

/// <summary>
/// Status de um questionário
/// </summary>
public enum QuestionnaireStatus
{
    /// <summary>
    /// Rascunho - questionário em criação
    /// </summary>
  Draft = 0,
    
    /// <summary>
    /// Publicado - questionário disponível para coleta de respostas
    /// </summary>
    Published = 1,
    
    /// <summary>
    /// Fechado - questionário não aceita mais respostas
  /// </summary>
    Closed = 2
}
