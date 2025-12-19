using infnet.SurveyQuestionnaire.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infnet.SurveyQuestionnaire.Infrastructure.Data.Configurations;

public class OptionConfiguration : IEntityTypeConfiguration<Option>
{
    public void Configure(EntityTypeBuilder<Option> builder)
    {
        builder.ToTable("Options");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Text)
     .IsRequired()
  .HasMaxLength(200);

builder.Property(o => o.QuestionId)
            .IsRequired();

        builder.Property(o => o.Order)
        .IsRequired();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UpdatedAt)
    .IsRequired(false);

        // Índices
        builder.HasIndex(o => o.QuestionId);
        builder.HasIndex(o => new { o.QuestionId, o.Order }).IsUnique();
    }
}
