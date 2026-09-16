using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Infrastructure.Data.Configurations;

public class SemesterConfiguration : IEntityTypeConfiguration<Semester>
{
  public void Configure(EntityTypeBuilder<Semester> builder)
  {
    builder.ToTable("Semesters");
    builder.HasKey(s => s.Id);

    builder.Property(s => s.Name).IsRequired().HasMaxLength(50);
  }
}