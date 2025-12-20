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

    public SubmissionService(ISubmissionRepository submissionRepository, IQuestionnaireRepository questionnaireRepository, IUserRepository userRepository,
                             IServiceBusPublisher serviceBusPublisher, IUnitOfWork unitOfWork) {
        _submissionRepository = submissionRepository;
        _questionnaireRepository = questionnaireRepository;
        _userRepository = userRepository;
        _serviceBusPublisher = serviceBusPublisher;
        _unitOfWork = unitOfWork;
    }

    public async Task<SubmissionResponse> CreateSubmissionAsync(CreateSubmissionRequest request, Guid userId) 
    {

        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new UnauthorizedAccessException($"User with ID '{userId}' not found");

        if (!user.IsPublicUser())
            throw new OnlyPublicUsersCanSubmitException(userId);

        var questionnaire = await _questionnaireRepository.GetByIdWithQuestionsAsync(request.QuestionnaireId)
                            ?? throw new KeyNotFoundException($"Questionnaire with ID '{request.QuestionnaireId}' not found");

        ValidateQuestionnaireAvailability(questionnaire);


        var hasSubmitted = await _submissionRepository.HasUserSubmittedAsync(questionnaire.Id, userId);

        if (hasSubmitted)
            throw new DuplicateSubmissionException(questionnaire.Id, userId);

        ValidateAnswers(questionnaire, request.Answers);

        var submission = Submission.Create(questionnaire.Id, userId);

        _submissionRepository.Add(submission);
        await _unitOfWork.SaveChangesAsync();

        var message = new SubmissionMessage 
        {
            SubmissionId = submission.Id,
            QuestionnaireId = questionnaire.Id,
            RespondentUserId = userId,
            SubmittedAt = submission.SubmittedAt,
            Answers = [.. request.Answers.Select(a => new SubmissionAnswerMessage
            {
                QuestionId = a.QuestionId,
                Answer = a.Answer,
                SelectedOptionId = a.SelectedOptionId
            })]
        };

        await _serviceBusPublisher.PublishAsync(message, "submission-queue");

        return MapToResponse(submission);
    }

    public async Task<SubmissionResponse> GetSubmissionByIdAsync(Guid id, Guid userId) 
    {
        var submission = await _submissionRepository.GetByIdAsync(id) ?? throw new SubmissionNotFoundException(id);

        await ValidateSubmissionAccess(submission, userId);

        return MapToResponse(submission);
    }

    public async Task<SubmissionResponse> GetSubmissionWithItemsAsync(Guid id, Guid userId) 
    {
        var submission = await _submissionRepository.GetByIdWithItemsAsync(id) ?? throw new SubmissionNotFoundException(id);


        await ValidateSubmissionAccess(submission, userId);

        return MapToDetailedResponse(submission);
    }

    public async Task<IEnumerable<SubmissionListResponse>> GetQuestionnaireSubmissionsAsync(Guid questionnaireId, Guid userId) 
    {

        var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId)
                            ?? throw new KeyNotFoundException($"Questionnaire with ID '{questionnaireId}' not found");

        await ValidateQuestionnaireOwnership(questionnaire, userId);

        var submissions = await _submissionRepository.GetByQuestionnaireIdAsync(questionnaireId);
        return await MapToListResponseAsync(submissions);
    }

    public async Task<int> CountQuestionnaireSubmissionsAsync(Guid questionnaireId, Guid userId) 
    {

        var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId)
                            ?? throw new KeyNotFoundException($"Questionnaire with ID '{questionnaireId}' not found");

        await ValidateQuestionnaireOwnership(questionnaire, userId);

        return await _submissionRepository.CountByQuestionnaireIdAsync(questionnaireId);
    }


    public async Task ProcessSubmissionAsync(SubmissionMessage message) 
    {
        try 
        {
            var submission = await LoadSubmissionForProcessingAsync(message.SubmissionId);

            if (ShouldSkipProcessing(submission))
                return;

            if (await TryCompletePartialSubmissionAsync(submission))
                return;

            await ResetSubmissionIfNecessaryAsync(submission);
            await StartAndProcessSubmissionAsync(submission, message.Answers);
            await CompleteSubmissionAsync(submission);
        }
        catch (Exception) {
            await HandleProcessingFailureAsync(message.SubmissionId);
            throw;
        }
    }

    private async Task<Submission> LoadSubmissionForProcessingAsync(Guid submissionId) 
    {
        return await _submissionRepository.GetByIdWithItemsAsync(submissionId)
                ?? throw new SubmissionNotFoundException(submissionId);
    }

    private static bool ShouldSkipProcessing(Submission submission) 
    {
        return submission.Status == SubmissionStatus.Completed;
    }

    private async Task<bool> TryCompletePartialSubmissionAsync(Submission submission) 
    {
        if (submission.Status != SubmissionStatus.Processing || !submission.Items.Any())
            return false;

        submission.Complete();
        _submissionRepository.Update(submission);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private async Task ResetSubmissionIfNecessaryAsync(Submission submission) 
    {
        if (submission.Status == SubmissionStatus.Processing || submission.Status == SubmissionStatus.Failed) 
        {
            submission.ResetToPending();
            await _unitOfWork.SaveChangesAsync();
        }
    }

    private async Task StartAndProcessSubmissionAsync(Submission submission, List<SubmissionAnswerMessage> answers) 
    {
       
        foreach (var answer in answers) 
        {
            submission.AddItem(answer.QuestionId, answer.Answer, answer.SelectedOptionId);
        }

        submission.StartProcessing();

        _submissionRepository.Update(submission);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task CompleteSubmissionAsync(Submission submission) 
    {
        submission.Complete();
        _submissionRepository.Update(submission);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task HandleProcessingFailureAsync(Guid submissionId) 
    {
        var submission = await _submissionRepository.GetByIdAsync(submissionId);

        if (submission != null && submission.Status != SubmissionStatus.Failed) 
        {
            submission.Fail("Error processing submission");
            _submissionRepository.Update(submission);
            await _unitOfWork.SaveChangesAsync();
        }
    }

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
        var requiredQuestionIds = questionnaire.Questions
            .Where(q => q.IsRequired)
            .Select(q => q.Id)
            .ToList();

        foreach (var requiredQuestionId in requiredQuestionIds) 
        {
            if (!answers.Any(a => a.QuestionId == requiredQuestionId))
                throw new RequiredQuestionNotAnsweredException(requiredQuestionId);
        }

        foreach (var answer in answers) 
        {
            var question = questionnaire.Questions.FirstOrDefault(q => q.Id == answer.QuestionId)
                ?? throw new InvalidSubmissionException($"Question '{answer.QuestionId}' not found in questionnaire");

            if (question.IsMultipleChoice && answer.SelectedOptionId == null)
                throw new InvalidSubmissionException($"Question '{question.Id}' requires a selected option");

            if (question.IsMultipleChoice && answer.SelectedOptionId.HasValue && !question.Options.Any(o => o.Id == answer.SelectedOptionId.Value))
                throw new InvalidSubmissionException($"Selected option '{answer.SelectedOptionId}' is not valid for question '{question.Id}'");

        }
    }

    private async Task ValidateSubmissionAccess(Submission submission, Guid userId) 
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new UnauthorizedAccessException($"User with ID '{userId}' not found");


        if (submission.RespondentUserId != userId && !user.IsAdministrator())
            throw new UnauthorizedAccessException("You are not authorized to view this submission");
    }

    private async Task ValidateQuestionnaireOwnership(Questionnaire questionnaire, Guid userId) 
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new UnauthorizedAccessException($"User with ID '{userId}' not found");

        if (questionnaire.CreatedByUserId != userId && !user.IsAdministrator())
            throw new UnauthorizedAccessException("You are not authorized to view submissions of this questionnaire");
    }

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
            Items = [.. submission.Items.Select(item => new SubmissionItemResponse
            {
                Id = item.Id,
                QuestionId = item.QuestionId,
                QuestionText = string.Empty,
                Answer = item.Answer,
                SelectedOptionId = item.SelectedOptionId,
                SelectedOptionText = null
            })]
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
