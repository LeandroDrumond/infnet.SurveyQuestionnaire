using Microsoft.AspNetCore.Mvc;

namespace infnet.SurveyQuestionnaire.Api.Configuration;

/// <summary>
/// Configuração de controllers e APIs
/// </summary>
public static class ApiConfiguration
{
    public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
    {
        services.AddControllers()
          .AddJsonOptions(options =>
          {

              options.JsonSerializerOptions.PropertyNamingPolicy = null;
              options.JsonSerializerOptions.WriteIndented = true;
              options.JsonSerializerOptions.DefaultIgnoreCondition =
               System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
          })
        .ConfigureApiBehaviorOptions(options =>
        {

            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
              .Where(e => e.Value?.Errors.Count > 0)
              .ToDictionary(e => e.Key, e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray());

                var problemDetails = new ValidationProblemDetails(errors)
                {
                    Title = "One or more validation errors occurred.",
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Instance = context.HttpContext.Request.Path
                };

                return new BadRequestObjectResult(problemDetails)
                {
                    ContentTypes = { "application/json" }
                };
            };
        });

        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = ctx =>
            {
                ctx.ProblemDetails.Instance = ctx.HttpContext.Request.Path;
                ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
            };
        });

        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
        });

        return services;
    }
}
