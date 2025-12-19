namespace infnet.SurveyQuestionnaire.Api.Configuration;

/// <summary>
/// Extension method central para configuração de todos os serviços
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adiciona todos os serviços da aplicação
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration configuration,IWebHostEnvironment environment)
    {
        services.AddApiConfiguration();
        services.AddAutoMapperConfiguration();
        services.AddValidation();
        services.AddSwaggerConfiguration();
        services.AddInfrastructure(configuration, environment);
        services.AddApplication();

       return services;
    }
}
