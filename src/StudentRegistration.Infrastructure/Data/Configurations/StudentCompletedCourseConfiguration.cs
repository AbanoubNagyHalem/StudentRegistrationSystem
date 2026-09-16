using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Infrastructure.Data.Configurations;

public class StudentCompletedCourseConfiguration : IEntityTypeConfiguration<StudentCompletedCourse>
{
  public void Configure(EntityTypeBuilder<StudentCompletedCourse> builder)
  {
    builder.ToTable("StudentCompletedCourses");
    builder.HasKey(scc => new { scc.StudentId, scc.CourseId });

    builder.Property(scc => scc.Grade).IsRequired().HasMaxLength(5);

    builder.HasOne(scc => scc.Student)
           .WithMany(s => s.CompletedCourses)
           .HasForeignKey(scc => scc.StudentId)
           .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(scc => scc.Course)
           .WithMany()
           .HasForeignKey(scc => scc.CourseId)
           .OnDelete(DeleteBehavior.Restrict);
  }
}