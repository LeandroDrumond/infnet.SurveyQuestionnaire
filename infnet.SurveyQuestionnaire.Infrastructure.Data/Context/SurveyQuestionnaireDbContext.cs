using infnet.SurveyQuestionnaire.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace infnet.SurveyQuestionnaire.Infrastructure.Data.Context;

public class SurveyQuestionnaireDbContext : DbContext
{
    public SurveyQuestionnaireDbContext(DbContextOptions<SurveyQuestionnaireDbContext> options)
    : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Questionnaire> Questionnaires => Set<Questionnaire>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Option> Options => Set<Option>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<SubmissionItem> SubmissionItems => Set<SubmissionItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SurveyQuestionnaireDbContext).Assembly);
    }
}
