using infnet.SurveyQuestionnaire.Application.DTOs.Submissions;
using infnet.SurveyQuestionnaire.Application.Interfaces;
using infnet.SurveyQuestionnaire.Domain;
using infnet.SurveyQuestionnaire.Domain.Common;
using infnet.SurveyQuestionnaire.Domain.Entities;
using infnet.SurveyQuestionnaire.Domain.Exceptions;
using infnet.SurveyQuestionnaire.Domain.Repositories;

namespace infnet.SurveyQuestionnaire.Application.Services;

/// <summary>
/// Serviço de submissions (respostas de questionários)
/// </summary>
public class SubmissionService : ISubmissionService
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IQuestionnaireRepository _questionnaireRepository;
    private readonly IUserRepository _userRepository;
    private readonly IServiceBusPublisher _serviceBusPublisher;
    private readonly IUnitOfWork _unitOfWork;

    public SubmissionService(
 ISubmissionRepository submissionRepository,
        IQuestionnaireRepository questionnaireRepository,
     IUserRepository userRepository,
  IServiceBusPublisher serviceBusPublisher,
     IUnitOfWork unitOfWork)
    {
        _submissionRepository = submissionRepository;
    _questionnaireRepository = questionnaireRepository;
   _userRepository = userRepository;
        _serviceBusPublisher = serviceBusPublisher;
   _unitOfWork = unitOfWork;
 }

 // ==================== Public Methods ====================

    public async Task<SubmissionResponse> CreateSubmissionAsync(CreateSubmissionRequest request, Guid userId)
    {
  // 1. Validar usuário
 var user = await _userRepository.GetByIdAsync(userId)
     ?? throw new UnauthorizedAccessException($"User with ID '{userId}' not found");

        if (!user.IsPublicUser())
      throw new OnlyPublicUsersCanSubmitException(userId);

        // 2. Buscar questionário com questões
      var questionnaire = await _questionnaireRepository.GetByIdWithQuestionsAsync(request.QuestionnaireId)
         ?? throw new KeyNotFoundException($"Questionnaire with ID '{request.QuestionnaireId}' not found");

   // 3. Validar se questionário está disponível
        ValidateQuestionnaireAvailability(questionnaire);

 // 4. Verificar duplicação
  var hasSubmitted = await _submissionRepository.HasUserSubmittedAsync(questionnaire.Id, userId);
     if (hasSubmitted)
      throw new DuplicateSubmissionException(questionnaire.Id, userId);

 // 5. Validar respostas
    ValidateAnswers(questionnaire, request.Answers);

        // 6. Criar submission (aggregate)
   var submission = Submission.Create(questionnaire.Id, userId);

        // 7. Adicionar no repositório (ainda não salva no banco)
        _submissionRepository.Add(submission);
        await _unitOfWork.SaveChangesAsync();

        // 8. Criar mensagem para Service Bus
        var message = new SubmissionMessage
     {
            SubmissionId = submission.Id,
       QuestionnaireId = questionnaire.Id,
     RespondentUserId = userId,
 SubmittedAt = submission.SubmittedAt,
            Answers = request.Answers.Select(a => new SubmissionAnswerMessage
            {
      QuestionId = a.QuestionId,
            Answer = a.Answer,
   SelectedOptionId = a.SelectedOptionId
            }).ToList()
     };

// 9. Enviar para Service Bus (assíncrono)
    await _serviceBusPublisher.PublishAsync(message, "submission-queue");

        // 10. Retornar response
        return MapToResponse(submission);
    }

    public async Task<SubmissionResponse> GetSubmissionByIdAsync(Guid id, Guid userId)
    {
     var submission = await _submissionRepository.GetByIdAsync(id)
     ?? throw new SubmissionNotFoundException(id);

 // Validar autorização (apenas o respondente ou admin pode ver)
  await ValidateSubmissionAccess(submission, userId);

        return MapToResponse(submission);
    }

  public async Task<SubmissionResponse> GetSubmissionWithItemsAsync(Guid id, Guid userId)
    {
        var submission = await _submissionRepository.GetByIdWithItemsAsync(id)
  ?? throw new SubmissionNotFoundException(id);

        // Validar autorização
  await ValidateSubmissionAccess(submission, userId);

  return MapToDetailedResponse(submission);
}

    public async Task<IEnumerable<SubmissionListResponse>> GetMySubmissionsAsync(Guid userId)
    {
     var submissions = await _submissionRepository.GetByUserIdAsync(userId);
        return await MapToListResponseAsync(submissions);
    }

    public async Task<IEnumerable<SubmissionListResponse>> GetQuestionnaireSubmissionsAsync(Guid questionnaireId, Guid userId)
    {
    // Validar autorização (apenas admin ou criador do questionário)
    var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId)
         ?? throw new KeyNotFoundException($"Questionnaire with ID '{questionnaireId}' not found");

     await ValidateQuestionnaireOwnership(questionnaire, userId);

  var submissions = await _submissionRepository.GetByQuestionnaireIdAsync(questionnaireId);
        return await MapToListResponseAsync(submissions);
 }

    public async Task<int> CountQuestionnaireSubmissionsAsync(Guid questionnaireId, Guid userId)
    {
 // Validar autorização
 var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId)
      ?? throw new KeyNotFoundException($"Questionnaire with ID '{questionnaireId}' not found");

  await ValidateQuestionnaireOwnership(questionnaire, userId);

  return await _submissionRepository.CountByQuestionnaireIdAsync(questionnaireId);
    }

    // ==================== Processing (Azure Function) ====================

  public async Task ProcessSubmissionAsync(SubmissionMessage message)
    {
   try
        {
   // 1. Buscar submission
       var submission = await _submissionRepository.GetByIdAsync(message.SubmissionId)
     ?? throw new SubmissionNotFoundException(message.SubmissionId);

            // 2. Marcar como em processamento
     submission.StartProcessing();
     _submissionRepository.Update(submission);
      await _unitOfWork.SaveChangesAsync();

  // 3. Buscar questionário com questões
    var questionnaire = await _questionnaireRepository.GetByIdWithQuestionsAsync(message.QuestionnaireId)
?? throw new KeyNotFoundException($"Questionnaire with ID '{message.QuestionnaireId}' not found");

            // 4. Validar novamente (segurança)
      ValidateQuestionnaireAvailability(questionnaire);
       ValidateAnswers(questionnaire, message.Answers.Select(a => new SubmissionAnswerRequest
      {
         QuestionId = a.QuestionId,
            Answer = a.Answer,
SelectedOptionId = a.SelectedOptionId
   }).ToList());

       // 5. Adicionar items
  foreach (var answer in message.Answers)
      {
submission.AddItem(answer.QuestionId, answer.Answer, answer.SelectedOptionId);
   }

     // 6. Marcar como completa
       submission.Complete();

 // 7. Salvar
      _submissionRepository.Update(submission);
  await _unitOfWork.SaveChangesAsync();
    }
        catch (Exception)
    {
  // Marcar como falha
       var submission = await _submissionRepository.GetByIdAsync(message.SubmissionId);
            if (submission != null)
       {
       submission.Fail("Error processing submission");
       _submissionRepository.Update(submission);
 await _unitOfWork.SaveChangesAsync();
  }

     throw; // Re-throw para Service Bus fazer retry
  }
    }

    // ==================== Private Validation Methods ====================

    private static void ValidateQuestionnaireAvailability(Questionnaire questionnaire)
    {
   if (questionnaire.Status != QuestionnaireStatus.Published)
     throw new QuestionnaireNotAvailableException(questionnaire.Id, "Questionnaire is not published");

 var now = DateTime.UtcNow;
        if (questionnaire.CollectionStart > now)
      throw new QuestionnaireNotAvailableException(questionnaire.Id, "Collection period has not started yet");

     if (questionnaire.CollectionEnd < now)
     throw new QuestionnaireNotAvailableException(questionnaire.Id, "Collection period has ended");
    }

    private static void ValidateAnswers(Questionnaire questionnaire, List<SubmissionAnswerRequest> answers)
  {
      // Validar que todas as questões obrigatórias foram respondidas
      var requiredQuestions = questionnaire.Questions.Where(q => q.IsRequired).ToList();
   foreach (var question in requiredQuestions)
     {
     if (!answers.Any(a => a.QuestionId == question.Id))
      throw new RequiredQuestionNotAnsweredException(question.Id);
    }

        // Validar que respostas de múltipla escolha têm optionId válido
    foreach (var answer in answers)
   {
            var question = questionnaire.Questions.FirstOrDefault(q => q.Id == answer.QuestionId)
      ?? throw new InvalidSubmissionException($"Question '{answer.QuestionId}' not found in questionnaire");

    if (question.IsMultipleChoice && answer.SelectedOptionId == null)
        throw new InvalidSubmissionException($"Question '{question.Id}' requires a selected option");

 if (question.IsMultipleChoice && answer.SelectedOptionId.HasValue)
      {
     if (!question.Options.Any(o => o.Id == answer.SelectedOptionId.Value))
               throw new InvalidSubmissionException($"Selected option '{answer.SelectedOptionId}' is not valid for question '{question.Id}'");
     }
 }
    }

    private async Task ValidateSubmissionAccess(Submission submission, Guid userId)
   {
        var user = await _userRepository.GetByIdAsync(userId)
     ?? throw new UnauthorizedAccessException($"User with ID '{userId}' not found");

        // Apenas o respondente ou admin pode ver
  if (submission.RespondentUserId != userId && !user.IsAdministrator())
            throw new UnauthorizedAccessException("You are not authorized to view this submission");
    }

    private async Task ValidateQuestionnaireOwnership(Questionnaire questionnaire, Guid userId)
  {
     var user = await _userRepository.GetByIdAsync(userId)
   ?? throw new UnauthorizedAccessException($"User with ID '{userId}' not found");

        // Apenas o criador ou admin pode ver submissions do questionário
 if (questionnaire.CreatedByUserId != userId && !user.IsAdministrator())
    throw new UnauthorizedAccessException("You are not authorized to view submissions of this questionnaire");
    }

    // ==================== Mapping Methods ====================

    private static SubmissionResponse MapToResponse(Submission submission)
    {
        return new SubmissionResponse
        {
   Id = submission.Id,
        QuestionnaireId = submission.QuestionnaireId,
    RespondentUserId = submission.RespondentUserId,
       Status = submission.Status.ToString(),
SubmittedAt = submission.SubmittedAt,
            FailureReason = submission.FailureReason,
  Items = null
   };
    }

    private static SubmissionResponse MapToDetailedResponse(Submission submission)
    {
        return new SubmissionResponse
  {
   Id = submission.Id,
            QuestionnaireId = submission.QuestionnaireId,
    RespondentUserId = submission.RespondentUserId,
      Status = submission.Status.ToString(),
    SubmittedAt = submission.SubmittedAt,
       FailureReason = submission.FailureReason,
   Items = submission.Items.Select(item => new SubmissionItemResponse
      {
       Id = item.Id,
    QuestionId = item.QuestionId,
     QuestionText = string.Empty, // Preenchido abaixo se necessário
        Answer = item.Answer,
         SelectedOptionId = item.SelectedOptionId,
      SelectedOptionText = null
   }).ToList()
     };
  }

    private async Task<List<SubmissionListResponse>> MapToListResponseAsync(IEnumerable<Submission> submissions)
    {
        var result = new List<SubmissionListResponse>();

        foreach (var submission in submissions)
  {
     var questionnaire = await _questionnaireRepository.GetByIdAsync(submission.QuestionnaireId);
      
  result.Add(new SubmissionListResponse
            {
      Id = submission.Id,
QuestionnaireId = submission.QuestionnaireId,
           QuestionnaireTitle = questionnaire?.Title ?? "Unknown",
        Status = submission.Status.ToString(),
   SubmittedAt = submission.SubmittedAt
      });
  }

   return result;
    }
}
