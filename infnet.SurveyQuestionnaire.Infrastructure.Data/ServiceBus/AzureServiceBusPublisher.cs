using Azure.Messaging.ServiceBus;
using infnet.SurveyQuestionnaire.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace infnet.SurveyQuestionnaire.Infrastructure.Data.ServiceBus;

/// <summary>
/// Implementação real do Service Bus Publisher usando Azure Service Bus
/// </summary>
public class AzureServiceBusPublisher : IServiceBusPublisher, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly string _defaultQueueName;
    private readonly Dictionary<string, ServiceBusSender> _senders = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public AzureServiceBusPublisher(IConfiguration configuration)
    {
        var connectionString = configuration["AzureServiceBus:ConnectionString"]
  ?? throw new InvalidOperationException("Azure Service Bus connection string not configured");

        _defaultQueueName = configuration["AzureServiceBus:DefaultQueueName"] ?? "submission-queue";

     _client = new ServiceBusClient(connectionString);
    }

    /// <summary>
    /// Publica uma mensagem em uma fila do Azure Service Bus
    /// </summary>
    public async Task PublishAsync<T>(T message, string? queueName = null, CancellationToken cancellationToken = default) where T : class
    {
   var queue = queueName ?? _defaultQueueName;
        var sender = await GetOrCreateSenderAsync(queue);

     try
        {
            // Serializa a mensagem para JSON
            var messageBody = JsonSerializer.Serialize(message, new JsonSerializerOptions 
            { 
   PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });

     // Cria mensagem do Service Bus
       var serviceBusMessage = new ServiceBusMessage(messageBody)
            {
        ContentType = "application/json",
   MessageId = Guid.NewGuid().ToString(),
             // Adiciona propriedades customizadas para facilitar rastreamento
   Subject = typeof(T).Name
            };

 // Envia mensagem
     await sender.SendMessageAsync(serviceBusMessage, cancellationToken);
        }
     catch (Exception ex)
        {
       throw new InvalidOperationException($"Error publishing message to queue '{queue}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Publica múltiplas mensagens em uma fila do Azure Service Bus
    /// </summary>
    public async Task PublishBatchAsync<T>(IEnumerable<T> messages, string? queueName = null, CancellationToken cancellationToken = default) where T : class
    {
        var messagesList = messages.ToList();
        if (!messagesList.Any())
    return;

 var queue = queueName ?? _defaultQueueName;
        var sender = await GetOrCreateSenderAsync(queue);

 try
    {
        // Processa mensagens em batches
 var currentBatch = new List<ServiceBusMessage>();
         
        foreach (var message in messagesList)
      {
          // Serializa mensagem
       var messageBody = JsonSerializer.Serialize(message, new JsonSerializerOptions 
   { 
       PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
    });

 var serviceBusMessage = new ServiceBusMessage(messageBody)
     {
         ContentType = "application/json",
MessageId = Guid.NewGuid().ToString(),
       Subject = typeof(T).Name
       };

         currentBatch.Add(serviceBusMessage);
   }

       // Envia todas as mensagens
if (currentBatch.Any())
            {
      using var messageBatch = await sender.CreateMessageBatchAsync(cancellationToken);
     
      foreach (var msg in currentBatch)
     {
         if (!messageBatch.TryAddMessage(msg))
           {
   // Se não couber, envia o batch atual
       if (messageBatch.Count > 0)
            {
         await sender.SendMessagesAsync(messageBatch, cancellationToken);
  }
      
          // Cria novo batch e adiciona mensagem
   using var newBatch = await sender.CreateMessageBatchAsync(cancellationToken);
        if (!newBatch.TryAddMessage(msg))
     {
        throw new InvalidOperationException($"Message is too large for queue '{queue}'");
   }
    await sender.SendMessagesAsync(newBatch, cancellationToken);
      }
     }

      // Envia batch final
      if (messageBatch.Count > 0)
        {
      await sender.SendMessagesAsync(messageBatch, cancellationToken);
             }
       }
  }
 catch (Exception ex)
        {
       throw new InvalidOperationException($"Error publishing batch messages to queue '{queue}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtém ou cria um sender para uma fila específica (com cache)
    /// </summary>
    private async Task<ServiceBusSender> GetOrCreateSenderAsync(string queueName)
    {
      // Thread-safe: verifica se já existe sender em cache
        if (_senders.TryGetValue(queueName, out var existingSender))
 {
            return existingSender;
    }

        await _semaphore.WaitAsync();
        try
        {
            // Double-check após obter lock
       if (_senders.TryGetValue(queueName, out var cachedSender))
      {
  return cachedSender;
            }

            // Cria novo sender
      var sender = _client.CreateSender(queueName);
    _senders[queueName] = sender;
 return sender;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Libera recursos (senders e client)
    /// </summary>
    public async ValueTask DisposeAsync()
    {
  // Fecha todos os senders
  foreach (var sender in _senders.Values)
        {
    await sender.DisposeAsync();
        }
        _senders.Clear();

   // Fecha o client
        await _client.DisposeAsync();

    _semaphore.Dispose();

        GC.SuppressFinalize(this);
 }
}
