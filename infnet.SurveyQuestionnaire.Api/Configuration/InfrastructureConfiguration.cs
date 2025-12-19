using infnet.SurveyQuestionnaire.Application.Interfaces;
using infnet.SurveyQuestionnaire.Domain.Common;
using infnet.SurveyQuestionnaire.Domain.Repositories;
using infnet.SurveyQuestionnaire.Infrastructure.Data;
using infnet.SurveyQuestionnaire.Infrastructure.Data.Context;
using infnet.SurveyQuestionnaire.Infrastructure.Data.Repositories;
using infnet.SurveyQuestionnaire.Infrastructure.Data.ServiceBus;
using Microsoft.EntityFrameworkCore;

namespace infnet.SurveyQuestionnaire.Api.Configuration;

/// <summary>
/// Configuração de injeção de dependências da camada Infrastructure
/// </summary>
public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration,IWebHostEnvironment environment)
    {
      services.AddDbContext<SurveyQuestionnaireDbContext>(options =>
      {
    options.UseSqlServer( configuration.GetConnectionString("DefaultConnection"),
     sqlOptions =>
         {
   sqlOptions.MigrationsAssembly("infnet.SurveyQuestionnaire.Infrastructure.Data");
   sqlOptions.EnableRetryOnFailure(
       maxRetryCount: 5,
   maxRetryDelay: TimeSpan.FromSeconds(30),
        errorNumbersToAdd: null);
  });

  options.EnableSensitiveDataLogging(false);
      options.EnableDetailedErrors(false);
  options.ConfigureWarnings(warnings =>{ warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.ContextInitialized);});

          if (environment.IsDevelopment())
    {
       // Descomente apenas se precisar debugar queries
      // options.LogTo(Console.WriteLine, LogLevel.Information);
        }
    });

      // ==================== Unit of Work ====================
   services.AddScoped<IUnitOfWork, UnitOfWork>();

      // ==================== Repositories ====================
 services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IQuestionnaireRepository, QuestionnaireRepository>();
    services.AddScoped<ISubmissionRepository, SubmissionRepository>();

 // ==================== Azure Service Bus ====================
  // ✅ Sempre usa Azure Service Bus real
    services.AddSingleton<IServiceBusPublisher, AzureServiceBusPublisher>();

        return services;
    }

    /// <summary>
    /// Aplica migrations pendentes automaticamente (usar apenas em Development)
    /// </summary>
    public static IServiceCollection AddDatabaseMigration(this IServiceCollection services)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SurveyQuestionnaireDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SurveyQuestionnaireDbContext>>();

        try
        {
            logger.LogInformation("Applying pending migrations...");
            dbContext.Database.Migrate();
            logger.LogInformation("Migrations applied successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error applying migrations");
            throw;
        }

    return services;
    }
}
