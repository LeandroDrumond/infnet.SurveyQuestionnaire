using infnet.SurveyQuestionnaire.Api.Configuration;
using infnet.SurveyQuestionnaire.Api.Middleware;

namespace infnet.SurveyQuestionnaire.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddApplicationServices(builder.Configuration,
            builder.Environment);

        var app = builder.Build();
            app.UseExceptionHandling();
            app.UseStaticFiles();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerConfiguration();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
