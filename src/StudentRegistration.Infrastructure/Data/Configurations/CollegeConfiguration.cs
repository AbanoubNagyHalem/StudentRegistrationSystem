using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Infrastructure.Data.Configurations;

public class CollegeConfiguration : IEntityTypeConfiguration<College>
{
  public void Configure(EntityTypeBuilder<College> builder)
  {
    builder.ToTable("Colleges");
    builder.HasKey(c => c.Id);

    builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
    builder.Property(c => c.Code).IsRequired().HasMaxLength(10);

    builder.HasIndex(c => c.Code).IsUnique();
  }
}