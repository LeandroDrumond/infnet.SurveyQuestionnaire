using infnet.SurveyQuestionnaire.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infnet.SurveyQuestionnaire.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração do EF Core para a entidade Submission
/// </summary>
public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
  public void Configure(EntityTypeBuilder<Submission> builder)
  {
      
        builder.ToTable("Submissions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
               .IsRequired()
               .ValueGeneratedNever();

        builder.Property(s => s.QuestionnaireId)
                .IsRequired();

        builder.Property(s => s.RespondentUserId)
                .IsRequired();

        builder.Property(s => s.SubmittedAt)
               .IsRequired();

    builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.FailureReason)
       .HasMaxLength(1000);

      builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

    builder.HasOne<Questionnaire>()
           .WithMany()
         .HasForeignKey(s => s.QuestionnaireId)
          .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(s => s.RespondentUserId)
                  .OnDelete(DeleteBehavior.Restrict);


        builder.Metadata.FindNavigation(nameof(Submission.Items))!
                        .SetPropertyAccessMode(PropertyAccessMode.Field);
      
        builder.Navigation(nameof(Submission.Items))
               .HasField("_items")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(s => s.QuestionnaireId)
              .HasDatabaseName("IX_Submissions_QuestionnaireId");

        builder.HasIndex(s => s.RespondentUserId)
               .HasDatabaseName("IX_Submissions_RespondentUserId");

        builder.HasIndex(s => s.Status)
               .HasDatabaseName("IX_Submissions_Status");

        builder.HasIndex(s => new { s.QuestionnaireId, s.RespondentUserId })
               .IsUnique()
               .HasDatabaseName("IX_Submissions_QuestionnaireId_RespondentUserId");

   
        builder.HasIndex(s => s.SubmittedAt)
                .HasDatabaseName("IX_Submissions_SubmittedAt");
    }
}
