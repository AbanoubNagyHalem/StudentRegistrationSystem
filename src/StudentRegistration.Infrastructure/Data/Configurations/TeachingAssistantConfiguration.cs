using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Infrastructure.Data.Configurations;

public class TeachingAssistantConfiguration : IEntityTypeConfiguration<TeachingAssistant>
{
  public void Configure(EntityTypeBuilder<TeachingAssistant> builder)
  {
    builder.ToTable("TeachingAssistants");
    builder.HasKey(ta => ta.Id);

    builder.Property(ta => ta.Name).IsRequired().HasMaxLength(150);
    builder.Property(ta => ta.Email).HasMaxLength(150);
  }
}