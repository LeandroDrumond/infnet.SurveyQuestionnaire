using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace infnet.SurveyQuestionnaire.Functions;

public class Function1
{
    private readonly ILogger<Function1> _logger;

    public Function1(ILogger<Function1> logger)
    {
        _logger = logger;
    }

    [Function("Function1")]
    public void Run(
      [ServiceBusTrigger("myqueue", Connection = "ServiceBusConnection")] string message)
    {
        _logger.LogInformation("Service Bus queue trigger function processed message: {Message}", message);
    }
}