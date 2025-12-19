
namespace infnet.SurveyQuestionnaire.Api.Configuration;

/// <summary>
/// Configuração do AutoMapper
/// </summary>
public static class AutoMapperConfiguration
{
    public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
    {

        services.AddAutoMapper(typeof(Program).Assembly);

        return services;
    }
}
