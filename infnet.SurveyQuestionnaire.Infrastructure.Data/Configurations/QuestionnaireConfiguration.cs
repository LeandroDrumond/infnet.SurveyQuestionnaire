using infnet.SurveyQuestionnaire.Domain.Entities;
using infnet.SurveyQuestionnaire.Domain.Questionnaires;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infnet.SurveyQuestionnaire.Infrastructure.Data.Configurations;

public class QuestionnaireConfiguration : IEntityTypeConfiguration<Questionnaire>
{
    public void Configure(EntityTypeBuilder<Questionnaire> builder)
    {
  builder.ToTable("Questionnaires");

        builder.HasKey(q => q.Id);

  builder.Property(q => q.Title)
            .IsRequired()
   .HasMaxLength(200);

     builder.Property(q => q.Description)
            .IsRequired()
       .HasMaxLength(1000);

  builder.Property(q => q.Status)
    .IsRequired()
 .HasConversion<string>()
 .HasMaxLength(20);

        builder.Property(q => q.CollectionStart)
            .IsRequired(false);

      builder.Property(q => q.CollectionEnd)
.IsRequired(false);

  builder.Property(q => q.CreatedByUserId)
  .IsRequired();

        builder.Property(q => q.CreatedAt)
   .IsRequired();

        builder.Property(q => q.UpdatedAt)
 .IsRequired(false);

        // Relacionamento com Questions
  builder.HasMany(q => q.Questions)
        .WithOne()
         .HasForeignKey(question => question.QuestionnaireId)
         .OnDelete(DeleteBehavior.Cascade);

   // Relacionamento com User (criador)
        builder.HasOne<User>()
   .WithMany()
      .HasForeignKey(q => q.CreatedByUserId)
      .OnDelete(DeleteBehavior.Restrict);

        // Índices
    builder.HasIndex(q => q.CreatedByUserId);
        builder.HasIndex(q => q.Status);
        builder.HasIndex(q => q.CreatedAt);
    }
}
