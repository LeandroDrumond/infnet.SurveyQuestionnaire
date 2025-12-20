using infnet.SurveyQuestionnaire.Application.DTOs.Submissions;
using infnet.SurveyQuestionnaire.Application.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace infnet.SurveyQuestionnaire.Functions;

/// <summary>
/// Azure Function que processa submissions do Service Bus
/// </summary>
public class SubmissionProcessorFunction
{
    private readonly ILogger<SubmissionProcessorFunction> _logger;
    private readonly ISubmissionProcessor _submissionProcessor;
    private static readonly JsonSerializerOptions _jsonOptions = new() {
        PropertyNameCaseInsensitive = true
    };

    public SubmissionProcessorFunction(
        ILogger<SubmissionProcessorFunction> logger,
    ISubmissionProcessor submissionProcessor)
    {
        _logger = logger;
        _submissionProcessor = submissionProcessor;
    }

    /// <summary>
    /// Processa mensagens da fila de submissions
    /// </summary>
    [Function("SubmissionProcessor")]
  public async Task Run(
        [ServiceBusTrigger("submission-queue", Connection = "ServiceBusConnection")]
        string messageBody,
        FunctionContext context)
    {
        var executionId = context.InvocationId;

        try
        {
            _logger.LogInformation(
       "[{ExecutionId}] Processing submission message. MessageBody: {MessageBody}",
    executionId,
       messageBody);

            var message = JsonSerializer.Deserialize<SubmissionMessage>(messageBody, _jsonOptions);

            if (message is null)
            {
   _logger.LogError("[{ExecutionId}] Failed to deserialize message", executionId);
           throw new InvalidOperationException("Invalid message format");
            }

            _logger.LogInformation(
     "[{ExecutionId}] Deserialized message. SubmissionId: {SubmissionId}, QuestionnaireId: {QuestionnaireId}, RespondentUserId: {RespondentUserId}",
     executionId,
                message.SubmissionId,
        message.QuestionnaireId,
      message.RespondentUserId);

            await _submissionProcessor.ProcessSubmissionAsync(message);

     _logger.LogInformation(
         "[{ExecutionId}] Successfully processed submission. SubmissionId: {SubmissionId}",
       executionId,
                message.SubmissionId);
        }
   catch (Exception ex)
        {
          _logger.LogError(
           ex,
  "[{ExecutionId}] Error processing submission message: {ErrorMessage}",
  executionId,
ex.Message);

            throw;
        }
    }
}
