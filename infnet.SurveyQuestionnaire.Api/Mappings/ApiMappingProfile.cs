using AutoMapper;
using infnet.SurveyQuestionnaire.Api.DTOs;
using infnet.SurveyQuestionnaire.Application.DTOs.Questionnaires;
using infnet.SurveyQuestionnaire.Application.DTOs.Submissions;
using infnet.SurveyQuestionnaire.Application.DTOs.Users;

namespace infnet.SurveyQuestionnaire.Api.Mappings;

/// <summary>
/// Perfil de mapeamento entre DTOs da API e DTOs da Application
/// </summary>
public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {

        CreateMap<CreateUserRequestDto, CreateUserRequest>();
        CreateMap<UpdateUserRequestDto, UpdateUserRequest>();
        CreateMap<UserResponse, UserResponseDto>();

        CreateMap<CreateQuestionnaireRequestDto, CreateQuestionnaireRequest>();
        CreateMap<UpdateQuestionnaireRequestDto, UpdateQuestionnaireRequest>();
        CreateMap<PublishQuestionnaireRequestDto, PublishQuestionnaireRequest>();
        CreateMap<AddQuestionRequestDto, AddQuestionRequest>();
        CreateMap<UpdateQuestionRequestDto, UpdateQuestionRequest>();
        CreateMap<AddOptionRequestDto, AddOptionRequest>();

        CreateMap<QuestionnaireResponse, QuestionnaireResponseDto>();
        CreateMap<QuestionnaireListResponse, QuestionnaireListResponseDto>();
        CreateMap<QuestionResponse, QuestionResponseDto>();
        CreateMap<OptionResponse, OptionResponseDto>();

        CreateMap<CreateSubmissionRequestDto, CreateSubmissionRequest>();
        CreateMap<SubmissionAnswerDto, SubmissionAnswerRequest>();

        CreateMap<SubmissionResponse, SubmissionResponseDto>();
        CreateMap<SubmissionListResponse, SubmissionListResponseDto>();
        CreateMap<SubmissionItemResponse, SubmissionItemResponseDto>();
    }
}
