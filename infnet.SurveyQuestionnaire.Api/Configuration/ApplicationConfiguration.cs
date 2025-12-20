using infnet.SurveyQuestionnaire.Application.Interfaces;
using infnet.SurveyQuestionnaire.Application.Services;

namespace infnet.SurveyQuestionnaire.Api.Configuration;

/// <summary>
/// Configuração de injeção de dependências da camada Application
/// </summary>
public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IQuestionnaireService, QuestionnaireService>();
        services.AddScoped<ISubmissionService, SubmissionService>();
        services.AddScoped<ISubmissionProcessor, SubmissionProcessor>();
  
        return services;
    }
}
