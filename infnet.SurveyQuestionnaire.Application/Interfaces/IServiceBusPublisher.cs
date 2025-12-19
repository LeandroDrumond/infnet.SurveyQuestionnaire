namespace infnet.SurveyQuestionnaire.Application.Interfaces;

/// <summary>
/// Interface para publicar mensagens no Azure Service Bus
/// </summary>
public interface IServiceBusPublisher
{
    /// <summary>
    /// Publica uma mensagem em uma fila
    /// </summary>
    /// <typeparam name="T">Tipo da mensagem</typeparam>
    /// <param name="message">Mensagem a ser publicada</param>
    /// <param name="queueName">Nome da fila (opcional, usa padrão se não informado)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task PublishAsync<T>(T message, string? queueName = null, CancellationToken cancellationToken = default) where T : class;
}
