using Azure.Messaging.ServiceBus;
using infnet.SurveyQuestionnaire.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace infnet.SurveyQuestionnaire.Infrastructure.Data.ServiceBus;

public class AzureServiceBusPublisher : IServiceBusPublisher, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly string _defaultQueueName;
    private readonly Dictionary<string, ServiceBusSender> _senders = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public AzureServiceBusPublisher(IConfiguration configuration)
    {
        
        var connectionString = configuration["AzureServiceBus:ConnectionString"]
                                ?? throw new InvalidOperationException("Azure Service Bus connection string not configured.");

        _defaultQueueName = configuration["AzureServiceBus:DefaultQueueName"] ?? "submission-queue";
        _client = new ServiceBusClient(connectionString);
    }

    public async Task PublishAsync<T>(T message, string? queueName = null, CancellationToken cancellationToken = default) where T : class
    {
    var queue = queueName ?? _defaultQueueName;
        var sender = await GetOrCreateSenderAsync(queue);

        try
        {
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

         await sender.SendMessageAsync(serviceBusMessage, cancellationToken);
        }
   catch (Exception ex)
        {
            throw new InvalidOperationException($"Error publishing message to queue '{queue}': {ex.Message}", ex);
   }
    }

    private async Task<ServiceBusSender> GetOrCreateSenderAsync(string queueName)
    {
        if (_senders.TryGetValue(queueName, out var existingSender))
            return existingSender;

        await _semaphore.WaitAsync();
   try
  {
            if (_senders.TryGetValue(queueName, out var cachedSender))
       {
        return cachedSender;
            }

    var sender = _client.CreateSender(queueName);
      _senders[queueName] = sender;
      return sender;
   }
        finally
   {
            _semaphore.Release();
        }
    }

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
