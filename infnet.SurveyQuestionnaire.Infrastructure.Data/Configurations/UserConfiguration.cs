using infnet.SurveyQuestionnaire.Domain.Entities;
using infnet.SurveyQuestionnaire.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infnet.SurveyQuestionnaire.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        
        builder.OwnsOne(u => u.Email, emailBuilder =>
        {
            emailBuilder.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(200);

            emailBuilder.HasIndex(e => e.Value)
                .IsUnique();
        });

       
        builder.Property(u => u.UserType)
            .IsRequired()
            .HasDefaultValue(UserType.Public)
            .HasConversion<int>(); // 0 = Public, 1 = Administrator

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
        builder.HasIndex(u => u.UserType);
    }
}
