using infnet.SurveyQuestionnaire.Application.DTOs.Questionnaires;

namespace infnet.SurveyQuestionnaire.Application.Interfaces;

public interface IQuestionnaireService
{
 // Questionnaire Operations
    Task<QuestionnaireResponse> CreateQuestionnaireAsync(CreateQuestionnaireRequest request, Guid createdByUserId);
    Task<QuestionnaireResponse> GetQuestionnaireByIdAsync(Guid id);
    Task<QuestionnaireResponse> GetQuestionnaireWithQuestionsAsync(Guid id);
    Task<IEnumerable<QuestionnaireListResponse>> GetAllQuestionnairesAsync();
    Task<IEnumerable<QuestionnaireListResponse>> GetQuestionnairesByCreatorAsync(Guid createdByUserId);
    Task<IEnumerable<QuestionnaireListResponse>> GetPublishedQuestionnairesAsync();
    Task<QuestionnaireResponse> UpdateQuestionnaireAsync(Guid id, UpdateQuestionnaireRequest request, Guid userId);
    Task DeleteQuestionnaireAsync(Guid id, Guid userId);
    
    // Publish/Close Operations
    Task<QuestionnaireResponse> PublishQuestionnaireAsync(Guid id, PublishQuestionnaireRequest request, Guid userId);
    Task<QuestionnaireResponse> CloseQuestionnaireAsync(Guid id, Guid userId);
    
    // Question Operations
    Task<QuestionnaireResponse> AddQuestionAsync(Guid questionnaireId, AddQuestionRequest request, Guid userId);
    Task<QuestionResponse> UpdateQuestionAsync(Guid questionId, UpdateQuestionRequest request, Guid userId);
    Task DeleteQuestionAsync(Guid questionId, Guid userId);
    
    // Option Operations
    Task<QuestionResponse> AddOptionsToQuestionAsync(Guid questionId, IEnumerable<AddOptionRequest> options, Guid userId);
    Task DeleteOptionAsync(Guid optionId, Guid userId);
}
