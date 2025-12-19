using FluentValidation;
using FluentValidation.AspNetCore;
using infnet.SurveyQuestionnaire.Application.Validators;

namespace infnet.SurveyQuestionnaire.Api.Configuration;

/// <summary>
/// Configuração de validações (FluentValidation)
/// </summary>
public static class ValidationConfiguration
{
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
     
        services.AddFluentValidationAutoValidation();

        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();

        return services;
    }
}
