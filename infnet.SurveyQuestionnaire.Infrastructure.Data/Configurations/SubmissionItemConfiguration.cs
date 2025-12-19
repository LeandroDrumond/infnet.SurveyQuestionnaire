using infnet.SurveyQuestionnaire.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infnet.SurveyQuestionnaire.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração do EF Core para a entidade SubmissionItem
/// </summary>
public class SubmissionItemConfiguration : IEntityTypeConfiguration<SubmissionItem>
{
    public void Configure(EntityTypeBuilder<SubmissionItem> builder)
    {
        // ==================== Table ====================
        builder.ToTable("SubmissionItems");

    // ==================== Primary Key ====================
 builder.HasKey(si => si.Id);

        // ==================== Properties ====================

        builder.Property(si => si.Id)
            .IsRequired()
         .ValueGeneratedNever(); // Guid gerado no Domain

        builder.Property(si => si.QuestionId)
    .IsRequired();

      builder.Property(si => si.Answer)
 .IsRequired()
   .HasMaxLength(5000);

        builder.Property(si => si.SelectedOptionId);

    builder.Property(si => si.SubmissionId)
            .IsRequired();

        builder.Property(si => si.CreatedAt)
       .IsRequired();

        builder.Property(si => si.UpdatedAt);

   // ==================== Relationships ====================

        // SubmissionItem -> Submission (Many-to-One)
    // Não precisa configurar aqui, já está configurado no SubmissionConfiguration

        // SubmissionItem -> Question (Many-to-One)
        builder.HasOne<Question>()
     .WithMany()
       .HasForeignKey(si => si.QuestionId)
  .OnDelete(DeleteBehavior.Restrict); // Não permite deletar questão com respostas

        // SubmissionItem -> Option (Many-to-One) - Opcional
        builder.HasOne<Option>()
 .WithMany()
   .HasForeignKey(si => si.SelectedOptionId)
      .OnDelete(DeleteBehavior.Restrict) // Não permite deletar opção com respostas
    .IsRequired(false);

        // ==================== Indexes ====================

   // Index para buscar respostas por questão
   builder.HasIndex(si => si.QuestionId)
   .HasDatabaseName("IX_SubmissionItems_QuestionId");

     // Index para buscar respostas por submission
        builder.HasIndex(si => si.SubmissionId)
   .HasDatabaseName("IX_SubmissionItems_SubmissionId");

  // Index para buscar respostas por opção selecionada
     builder.HasIndex(si => si.SelectedOptionId)
          .HasDatabaseName("IX_SubmissionItems_SelectedOptionId");
    }
}
