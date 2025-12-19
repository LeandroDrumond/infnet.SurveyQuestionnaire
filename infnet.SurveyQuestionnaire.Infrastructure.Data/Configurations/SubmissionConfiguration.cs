using infnet.SurveyQuestionnaire.Domain;
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
        // ==================== Table ====================
        builder.ToTable("Submissions");

        // ==================== Primary Key ====================
        builder.HasKey(s => s.Id);

        // ==================== Properties ====================

        builder.Property(s => s.Id)
   .IsRequired()
            .ValueGeneratedNever(); // Guid gerado no Domain

     builder.Property(s => s.QuestionnaireId)
            .IsRequired();

  builder.Property(s => s.RespondentUserId)
            .IsRequired();

        builder.Property(s => s.SubmittedAt)
     .IsRequired();

    builder.Property(s => s.Status)
     .IsRequired()
       .HasConversion<string>() // Armazena como string no banco
   .HasMaxLength(20);

        builder.Property(s => s.FailureReason)
       .HasMaxLength(1000);

      builder.Property(s => s.CreatedAt)
    .IsRequired();

        builder.Property(s => s.UpdatedAt);

        // ==================== Relationships ====================

        // Submission -> Questionnaire (Many-to-One)
    builder.HasOne<Questionnaire>()
     .WithMany()
   .HasForeignKey(s => s.QuestionnaireId)
    .OnDelete(DeleteBehavior.Restrict); // Não permite deletar questionário com submissions

// Submission -> User (Many-to-One)
        builder.HasOne<User>()
      .WithMany()
      .HasForeignKey(s => s.RespondentUserId)
        .OnDelete(DeleteBehavior.Restrict); // Não permite deletar usuário com submissions

    // Submission -> SubmissionItems (One-to-Many)
  // Usa o backing field "_items" porque Items é ReadOnly
        builder.Metadata.FindNavigation(nameof(Submission.Items))!
      .SetPropertyAccessMode(PropertyAccessMode.Field);
      
        builder.Navigation(nameof(Submission.Items))
       .HasField("_items")
        .UsePropertyAccessMode(PropertyAccessMode.Field);

   // ==================== Indexes ====================

        // Index para buscar submissions por questionário
        builder.HasIndex(s => s.QuestionnaireId)
        .HasDatabaseName("IX_Submissions_QuestionnaireId");

        // Index para buscar submissions por usuário
        builder.HasIndex(s => s.RespondentUserId)
            .HasDatabaseName("IX_Submissions_RespondentUserId");

  // Index para buscar submissions por status
        builder.HasIndex(s => s.Status)
     .HasDatabaseName("IX_Submissions_Status");

        // Index composto para verificar duplicação (usuário + questionário)
        builder.HasIndex(s => new { s.QuestionnaireId, s.RespondentUserId })
         .IsUnique()
         .HasDatabaseName("IX_Submissions_QuestionnaireId_RespondentUserId");

   // Index para buscar por data de submissão
  builder.HasIndex(s => s.SubmittedAt)
          .HasDatabaseName("IX_Submissions_SubmittedAt");
    }
}
