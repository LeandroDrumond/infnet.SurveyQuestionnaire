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
        builder.ToTable("SubmissionItems");
        builder.HasKey(si => si.Id);

        builder.Property(si => si.Id)
               .IsRequired()
              .ValueGeneratedNever();

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

 
        builder.HasOne<Question>()
                .WithMany()
                .HasForeignKey(si => si.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

     
        builder.HasOne<Option>()
               .WithMany()
                 .HasForeignKey(si => si.SelectedOptionId)
                    .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired(false);
               
      


       builder.HasIndex(si => si.QuestionId)
              .HasDatabaseName("IX_SubmissionItems_QuestionId");

 
        builder.HasIndex(si => si.SubmissionId)
               .HasDatabaseName("IX_SubmissionItems_SubmissionId");

 
       builder.HasIndex(si => si.SelectedOptionId)
              .HasDatabaseName("IX_SubmissionItems_SelectedOptionId");
    }
}
