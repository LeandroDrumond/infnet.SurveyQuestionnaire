using infnet.SurveyQuestionnaire.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infnet.SurveyQuestionnaire.Infrastructure.Data.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
         builder.ToTable("Questions");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.Text)
            .IsRequired()
          .HasMaxLength(500);

        builder.Property(q => q.QuestionnaireId)
               .IsRequired();

       builder.Property(q => q.IsRequired)
            .IsRequired()
        .HasDefaultValue(false);

        builder.Property(q => q.IsMultipleChoice)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(q => q.CreatedAt)
                .IsRequired();

        builder.Property(q => q.UpdatedAt)
               .IsRequired(false);

        builder.HasMany(q => q.Options)
               .WithOne()
               .HasForeignKey(o => o.QuestionId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(q => q.QuestionnaireId);
    }
}
