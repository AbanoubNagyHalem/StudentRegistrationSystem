using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Infrastructure.Data.Configurations;

public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
  public void Configure(EntityTypeBuilder<Instructor> builder)
  {
    builder.ToTable("Instructors");
    builder.HasKey(i => i.Id);

    builder.Property(i => i.Name).IsRequired().HasMaxLength(150);
    builder.Property(i => i.Email).HasMaxLength(150);
  }
}