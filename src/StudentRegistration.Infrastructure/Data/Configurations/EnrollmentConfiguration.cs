using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Infrastructure.Data.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
  public void Configure(EntityTypeBuilder<Enrollment> builder)
  {
    builder.ToTable("Enrollments");
    builder.HasKey(e => e.Id);

    builder.HasIndex(e => new { e.StudentId, e.SectionId }).IsUnique();
    builder.HasIndex(e => new { e.StudentId, e.Status });

    builder.HasOne(e => e.Student)
           .WithMany(s => s.Enrollments)
           .HasForeignKey(e => e.StudentId)
           .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(e => e.Section)
           .WithMany(cs => cs.Enrollments)
           .HasForeignKey(e => e.SectionId)
           .OnDelete(DeleteBehavior.Restrict);
  }
}