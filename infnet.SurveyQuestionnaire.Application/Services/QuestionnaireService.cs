using infnet.SurveyQuestionnaire.Application.DTOs.Questionnaires;
using infnet.SurveyQuestionnaire.Application.Interfaces;
using infnet.SurveyQuestionnaire.Domain.Common;
using infnet.SurveyQuestionnaire.Domain.Entities;
using infnet.SurveyQuestionnaire.Domain.Repositories;

namespace infnet.SurveyQuestionnaire.Application.Services;

public class QuestionnaireService : IQuestionnaireService
{
    private readonly IQuestionnaireRepository _questionnaireRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public QuestionnaireService(
      IQuestionnaireRepository questionnaireRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
{
        _questionnaireRepository = questionnaireRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<QuestionnaireResponse> CreateQuestionnaireAsync(CreateQuestionnaireRequest request, Guid createdByUserId)
    {
      var user = await _userRepository.GetByIdAsync(createdByUserId)
            ?? throw new UnauthorizedAccessException($"User with ID '{createdByUserId}' not found.");

        if (!user.IsAdministrator())
   {
 throw new UnauthorizedAccessException("Only administrators can create questionnaires.");
        }

        var questionnaire = Questionnaire.Create(
            request.Title,
            request.Description,
         createdByUserId
  );

     _questionnaireRepository.Add(questionnaire);
        await _unitOfWork.SaveChangesAsync();

 return MapToResponse(questionnaire);
    }

public async Task<QuestionnaireResponse> GetQuestionnaireByIdAsync(Guid id)
    {
        var questionnaire = await _questionnaireRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Questionnaire with ID '{id}' not found.");

        return MapToResponse(questionnaire);
    }

    public async Task<QuestionnaireResponse> GetQuestionnaireWithQuestionsAsync(Guid id)
    {
    var questionnaire = await _questionnaireRepository.GetByIdWithQuestionsAsync(id)
?? throw new KeyNotFoundException($"Questionnaire with ID '{id}' not found.");

   return MapToDetailedResponse(questionnaire);
    }

    public async Task<IEnumerable<QuestionnaireListResponse>> GetAllQuestionnairesAsync()
    {
        var questionnaires = await _questionnaireRepository.GetAllAsync();
  return questionnaires.Select(MapToListResponse);
    }

    public async Task<IEnumerable<QuestionnaireListResponse>> GetQuestionnairesByCreatorAsync(Guid createdByUserId)
    {
        var questionnaires = await _questionnaireRepository.GetByCreatorIdAsync(createdByUserId);
        return questionnaires.Select(MapToListResponse);
    }

    public async Task<IEnumerable<QuestionnaireListResponse>> GetPublishedQuestionnairesAsync()
    {
        var questionnaires = await _questionnaireRepository.GetPublishedAsync();
return questionnaires.Select(MapToListResponse);
    }

    public async Task<QuestionnaireResponse> UpdateQuestionnaireAsync(Guid id, UpdateQuestionnaireRequest request, Guid userId)
    {
        var questionnaire = await _questionnaireRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Questionnaire with ID '{id}' not found.");

    await ValidateQuestionnaireOwnership(questionnaire, userId);

        questionnaire.Update(request.Title, request.Description);
      
        // ✅ CORREÇÃO: Adicionar Update para o EF Core rastrear as mudanças
        _questionnaireRepository.Update(questionnaire);
        
   await _unitOfWork.SaveChangesAsync();

        return MapToResponse(questionnaire);
    }

    public async Task DeleteQuestionnaireAsync(Guid id, Guid userId)
    {
        var questionnaire = await _questionnaireRepository.GetByIdAsync(id)
  ?? throw new KeyNotFoundException($"Questionnaire with ID '{id}' not found.");

     await ValidateQuestionnaireOwnership(questionnaire, userId);

        _questionnaireRepository.Remove(questionnaire);
   await _unitOfWork.SaveChangesAsync();
    }

    public async Task<QuestionnaireResponse> PublishQuestionnaireAsync(Guid id, PublishQuestionnaireRequest request, Guid userId)
    {
        var questionnaire = await _questionnaireRepository.GetByIdWithQuestionsAsync(id)
   ?? throw new KeyNotFoundException($"Questionnaire with ID '{id}' not found.");

 await ValidateQuestionnaireOwnership(questionnaire, userId);

        questionnaire.Publish(request.CollectionStart, request.CollectionEnd);
        _questionnaireRepository.Update(questionnaire);
        await _unitOfWork.SaveChangesAsync();

   return MapToDetailedResponse(questionnaire);
    }

    public async Task<QuestionnaireResponse> CloseQuestionnaireAsync(Guid id, Guid userId)
    {
        var questionnaire = await _questionnaireRepository.GetByIdAsync(id)
?? throw new KeyNotFoundException($"Questionnaire with ID '{id}' not found.");

        await ValidateQuestionnaireOwnership(questionnaire, userId);

        questionnaire.Close();
     
        // ✅ CORREÇÃO: Adicionar Update para o EF Core rastrear as mudanças
        _questionnaireRepository.Update(questionnaire);
        
        await _unitOfWork.SaveChangesAsync();

     return MapToResponse(questionnaire);
    }

    public async Task<QuestionnaireResponse> AddQuestionAsync(Guid questionnaireId, AddQuestionRequest request, Guid userId)
    {
        var questionnaire = await _questionnaireRepository.GetByIdWithQuestionsAsync(questionnaireId)
     ?? throw new KeyNotFoundException($"Questionnaire with ID '{questionnaireId}' not found.");

      await ValidateQuestionnaireOwnership(questionnaire, userId);

   if (request.IsMultipleChoice && request.Options != null && request.Options.Any())
    {
     var options = request.Options
           .OrderBy(o => o.Order)
                .Select(o => (o.Text, o.Order));

            questionnaire.AddQuestionWithOptions(
    request.Text,
     request.IsRequired,
      options
       );
        }
    else
        {
       questionnaire.AddQuestion(
     request.Text,
  request.IsRequired,
     request.IsMultipleChoice
        );
   }

       _questionnaireRepository.Update(questionnaire);
        await _unitOfWork.SaveChangesAsync();

  return MapToDetailedResponse(questionnaire);
    }

    public async Task<QuestionResponse> UpdateQuestionAsync(Guid questionId, UpdateQuestionRequest request, Guid userId)
    {
        var questionnaires = await _questionnaireRepository.GetAllAsync();
     var questionnaire = questionnaires.FirstOrDefault(q => q.Questions.Any(qu => qu.Id == questionId))
   ?? throw new KeyNotFoundException($"Question with ID '{questionId}' not found.");

        await ValidateQuestionnaireOwnership(questionnaire, userId);

   questionnaire.UpdateQuestion(questionId, request.Text);
        
        // ✅ CORREÇÃO: Adicionar Update para o EF Core rastrear as mudanças
  _questionnaireRepository.Update(questionnaire);
        
        await _unitOfWork.SaveChangesAsync();

        var question = questionnaire.Questions.First(q => q.Id == questionId);
 return MapToQuestionResponse(question);
    }

  public async Task DeleteQuestionAsync(Guid questionId, Guid userId)
    {
        var questionnaires = await _questionnaireRepository.GetAllAsync();
        var questionnaire = questionnaires.FirstOrDefault(q => q.Questions.Any(qu => qu.Id == questionId))
      ?? throw new KeyNotFoundException($"Question with ID '{questionId}' not found.");

        await ValidateQuestionnaireOwnership(questionnaire, userId);

        questionnaire.RemoveQuestion(questionId);
        
        // ✅ CORREÇÃO: Adicionar Update para o EF Core rastrear as mudanças
        _questionnaireRepository.Update(questionnaire);
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<QuestionResponse> AddOptionsToQuestionAsync(Guid questionId, IEnumerable<AddOptionRequest> options, Guid userId)
    {
   var questionnaires = await _questionnaireRepository.GetAllAsync();
     var questionnaire = questionnaires.FirstOrDefault(q => q.Questions.Any(qu => qu.Id == questionId))
      ?? throw new KeyNotFoundException($"Question with ID '{questionId}' not found.");

        await ValidateQuestionnaireOwnership(questionnaire, userId);

      var optionsList = options
   .OrderBy(o => o.Order)
          .Select(o => (o.Text, o.Order));

    questionnaire.AddOptionsToQuestion(questionId, optionsList);
      
        // ✅ CORREÇÃO: Adicionar Update para o EF Core rastrear as mudanças
        _questionnaireRepository.Update(questionnaire);
        
 await _unitOfWork.SaveChangesAsync();

      var question = questionnaire.Questions.First(q => q.Id == questionId);
        return MapToQuestionResponse(question);
    }

    public async Task DeleteOptionAsync(Guid optionId, Guid userId)
  {
        var questionnaires = await _questionnaireRepository.GetAllAsync();
   var questionnaire = questionnaires.FirstOrDefault(q =>
     q.Questions.Any(qu => qu.Options.Any(o => o.Id == optionId)))
            ?? throw new KeyNotFoundException($"Option with ID '{optionId}' not found.");

        await ValidateQuestionnaireOwnership(questionnaire, userId);

        var question = questionnaire.Questions.First(q => q.Options.Any(o => o.Id == optionId));

 questionnaire.RemoveOptionFromQuestion(question.Id, optionId);
        
  // ✅ CORREÇÃO: Adicionar Update para o EF Core rastrear as mudanças
        _questionnaireRepository.Update(questionnaire);
        
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task ValidateQuestionnaireOwnership(Questionnaire questionnaire, Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
         ?? throw new UnauthorizedAccessException($"User with ID '{userId}' not found.");

        if (questionnaire.CreatedByUserId != userId && !user.IsAdministrator())
   {
            throw new UnauthorizedAccessException("You are not authorized to modify this questionnaire.");
        }
    }

    private static QuestionnaireResponse MapToResponse(Questionnaire questionnaire)
    {
        return new QuestionnaireResponse
        {
            Id = questionnaire.Id,
            Title = questionnaire.Title,
            Description = questionnaire.Description,
          Status = questionnaire.Status.ToString(),
     CollectionStart = questionnaire.CollectionStart,
   CollectionEnd = questionnaire.CollectionEnd,
            CreatedByUserId = questionnaire.CreatedByUserId,
    CreatedAt = questionnaire.CreatedAt,
         UpdatedAt = questionnaire.UpdatedAt,
     Questions = null
        };
    }

    private static QuestionnaireResponse MapToDetailedResponse(Questionnaire questionnaire)
    {
        return new QuestionnaireResponse
        {
       Id = questionnaire.Id,
Title = questionnaire.Title,
            Description = questionnaire.Description,
      Status = questionnaire.Status.ToString(),
            CollectionStart = questionnaire.CollectionStart,
     CollectionEnd = questionnaire.CollectionEnd,
  CreatedByUserId = questionnaire.CreatedByUserId,
   CreatedAt = questionnaire.CreatedAt,
         UpdatedAt = questionnaire.UpdatedAt,
       Questions = questionnaire.Questions?.Select(MapToQuestionResponse).ToList()
        };
}

    private static QuestionnaireListResponse MapToListResponse(Questionnaire questionnaire)
    {
      return new QuestionnaireListResponse
        {
            Id = questionnaire.Id,
     Title = questionnaire.Title,
        Description = questionnaire.Description,
 Status = questionnaire.Status.ToString(),
     CollectionStart = questionnaire.CollectionStart,
     CollectionEnd = questionnaire.CollectionEnd,
          QuestionCount = questionnaire.Questions?.Count ?? 0,
       CreatedAt = questionnaire.CreatedAt
    };
    }

    private static QuestionResponse MapToQuestionResponse(Question question)
{
        return new QuestionResponse
        {
   Id = question.Id,
       Text = question.Text,
            IsRequired = question.IsRequired,
     IsMultipleChoice = question.IsMultipleChoice,
            CreatedAt = question.CreatedAt,
            Options = question.Options?.Select(MapToOptionResponse).ToList()
     };
    }

    private static OptionResponse MapToOptionResponse(Option option)
    {
        return new OptionResponse
        {
       Id = option.Id,
            Text = option.Text,
     Order = option.Order
        };
    }
}
