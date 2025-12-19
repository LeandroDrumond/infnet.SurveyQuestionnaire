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

  // ==================== Questionnaire Operations ====================

    public async Task<QuestionnaireResponse> CreateQuestionnaireAsync(CreateQuestionnaireRequest request, Guid createdByUserId)
    {
        // Verificar se o usuário existe e é administrador
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

        // Verificar se o usuário é o criador ou administrador
        await ValidateQuestionnaireOwnership(questionnaire, userId);

        questionnaire.Update(request.Title, request.Description);

      _questionnaireRepository.Update(questionnaire);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(questionnaire);
    }

    public async Task DeleteQuestionnaireAsync(Guid id, Guid userId)
    {
        var questionnaire = await _questionnaireRepository.GetByIdAsync(id)
         ?? throw new KeyNotFoundException($"Questionnaire with ID '{id}' not found.");

        // Verificar se o usuário é o criador ou administrador
        await ValidateQuestionnaireOwnership(questionnaire, userId);

        _questionnaireRepository.Remove(questionnaire);
        await _unitOfWork.SaveChangesAsync();
}

    // ==================== Publish/Close Operations ====================

    public async Task<QuestionnaireResponse> PublishQuestionnaireAsync(Guid id, PublishQuestionnaireRequest request, Guid userId)
    {
        var questionnaire = await _questionnaireRepository.GetByIdWithQuestionsAsync(id)
         ?? throw new KeyNotFoundException($"Questionnaire with ID '{id}' not found.");

        // Verificar se o usuário é o criador ou administrador
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

        // Verificar se o usuário é o criador ou administrador
   await ValidateQuestionnaireOwnership(questionnaire, userId);

        questionnaire.Close();

 _questionnaireRepository.Update(questionnaire);
     await _unitOfWork.SaveChangesAsync();

        return MapToResponse(questionnaire);
    }

    // ==================== Question Operations ====================

    /// <summary>
  /// Adiciona uma questão ao questionário (através do Aggregate Root)
    /// </summary>
    public async Task<QuestionnaireResponse> AddQuestionAsync(Guid questionnaireId, AddQuestionRequest request, Guid userId)
    {
  // 1. Buscar o agregado (Root)
 var questionnaire = await _questionnaireRepository.GetByIdWithQuestionsAsync(questionnaireId)
    ?? throw new KeyNotFoundException($"Questionnaire with ID '{questionnaireId}' not found.");

  // 2. Validar ownership
   await ValidateQuestionnaireOwnership(questionnaire, userId);

     Guid questionId;

    // 3. ✅ Adicionar questão com opções (se for múltipla escolha E tiver opções)
      if (request.IsMultipleChoice && request.Options != null && request.Options.Any())
        {
    // ✅ NOVO: Adiciona questão com todas as opções de uma vez
            var options = request.Options
      .OrderBy(o => o.Order)
 .Select(o => (o.Text, o.Order));
         
     questionId = questionnaire.AddQuestionWithOptions(
    request.Text,
      request.IsRequired,
         options
);
        }
        else
        {
   // Adiciona questão sem opções (texto livre ou múltipla escolha vazia)
      questionId = questionnaire.AddQuestion(
           request.Text,
 request.IsRequired,
 request.IsMultipleChoice
  );
      }

     // 4. Salvar agregado
   _questionnaireRepository.Update(questionnaire);
      await _unitOfWork.SaveChangesAsync();

 return MapToDetailedResponse(questionnaire);
    }

    /// <summary>
    /// Atualiza uma questão (através do Aggregate Root)
    /// </summary>
    public async Task<QuestionResponse> UpdateQuestionAsync(Guid questionId, UpdateQuestionRequest request, Guid userId)
    {
   // 1. Buscar o questionário que contém a questão
        var questionnaires = await _questionnaireRepository.GetAllAsync();
        var questionnaire = questionnaires.FirstOrDefault(q => q.Questions.Any(qu => qu.Id == questionId))
     ?? throw new KeyNotFoundException($"Question with ID '{questionId}' not found.");

        // 2. Validar ownership
   await ValidateQuestionnaireOwnership(questionnaire, userId);

        // 3. ✅ Atualizar ATRAVÉS do Root
        questionnaire.UpdateQuestion(questionId, request.Text);

        // 4. Salvar
_questionnaireRepository.Update(questionnaire);
    await _unitOfWork.SaveChangesAsync();

    // 5. Retornar response
        var question = questionnaire.Questions.First(q => q.Id == questionId);
     return MapToQuestionResponse(question);
    }

    /// <summary>
 /// Remove uma questão (através do Aggregate Root)
    /// </summary>
    public async Task DeleteQuestionAsync(Guid questionId, Guid userId)
    {
        // 1. Buscar o questionário que contém a questão
var questionnaires = await _questionnaireRepository.GetAllAsync();
        var questionnaire = questionnaires.FirstOrDefault(q => q.Questions.Any(qu => qu.Id == questionId))
       ?? throw new KeyNotFoundException($"Question with ID '{questionId}' not found.");

  // 2. Validar ownership
   await ValidateQuestionnaireOwnership(questionnaire, userId);

   // 3. ✅ Remover ATRAVÉS do Root
    questionnaire.RemoveQuestion(questionId);

        // 4. Salvar
        _questionnaireRepository.Update(questionnaire);
 await _unitOfWork.SaveChangesAsync();
    }

    // ==================== Option Operations ====================

    /// <summary>
    /// Adiciona uma ou mais opções a uma questão (através do Aggregate Root)
    /// </summary>
    /// <param name="questionId">ID da questão</param>
    /// <param name="options">Lista de opções a adicionar (1 ou mais)</param>
    /// <param name="userId">ID do usuário fazendo a operação</param>
    public async Task<QuestionResponse> AddOptionsToQuestionAsync(Guid questionId, IEnumerable<AddOptionRequest> options, Guid userId)
    {
        // 1. Buscar o questionário que contém a questão
        var questionnaires = await _questionnaireRepository.GetAllAsync();
        var questionnaire = questionnaires.FirstOrDefault(q => q.Questions.Any(qu => qu.Id == questionId))
       ?? throw new KeyNotFoundException($"Question with ID '{questionId}' not found.");

        // 2. Validar ownership
        await ValidateQuestionnaireOwnership(questionnaire, userId);

 // 3. ✅ Adicionar opções ATRAVÉS do Root
        var optionsList = options
          .OrderBy(o => o.Order)
          .Select(o => (o.Text, o.Order));
      
 questionnaire.AddOptionsToQuestion(questionId, optionsList);

     // 4. Salvar
        _questionnaireRepository.Update(questionnaire);
        await _unitOfWork.SaveChangesAsync();

  // 5. Retornar response
        var question = questionnaire.Questions.First(q => q.Id == questionId);
        return MapToQuestionResponse(question);
    }

    /// <summary>
    /// Remove uma opção (através do Aggregate Root)
    /// </summary>
    public async Task DeleteOptionAsync(Guid optionId, Guid userId)
    {
     // 1. Buscar o questionário que contém a opção
  var questionnaires = await _questionnaireRepository.GetAllAsync();
var questionnaire = questionnaires.FirstOrDefault(q =>
            q.Questions.Any(qu => qu.Options.Any(o => o.Id == optionId)))
          ?? throw new KeyNotFoundException($"Option with ID '{optionId}' not found.");

        // 2. Validar ownership
        await ValidateQuestionnaireOwnership(questionnaire, userId);

     // 3. Encontrar questionId
        var question = questionnaire.Questions.First(q => q.Options.Any(o => o.Id == optionId));

      // 4. ✅ Remover ATRAVÉS do Root
   questionnaire.RemoveOptionFromQuestion(question.Id, optionId);

   // 5. Salvar
_questionnaireRepository.Update(questionnaire);
        await _unitOfWork.SaveChangesAsync();
    }

    // ==================== Private Helper Methods ====================

    private async Task ValidateQuestionnaireOwnership(Questionnaire questionnaire, Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
       ?? throw new UnauthorizedAccessException($"User with ID '{userId}' not found.");

 // Só o criador ou um administrador pode modificar
        if (questionnaire.CreatedByUserId != userId && !user.IsAdministrator())
        {
   throw new UnauthorizedAccessException("You are not authorized to modify this questionnaire.");
        }
    }

    // ==================== Mapping Methods ====================

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
Questions = null // Sem questões na resposta básica
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
