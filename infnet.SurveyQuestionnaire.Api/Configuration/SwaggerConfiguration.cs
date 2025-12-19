using System.Reflection;

namespace infnet.SurveyQuestionnaire.Api.Configuration;

/// <summary>
/// Configuração do Swagger/OpenAPI
/// </summary>
public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            
            options.SwaggerDoc("v1", new()
            {
                Title = "Survey Questionnaire API",
                Version = "v1",
                Description = "API para gerenciamento de questionários e pesquisas."
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Survey Questionnaire API v1");
            options.RoutePrefix = "swagger";
        });

        return app;
    }
}
