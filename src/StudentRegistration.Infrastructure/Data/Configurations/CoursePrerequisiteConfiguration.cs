using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Infrastructure.Data.Configurations;

public class CoursePrerequisiteConfiguration : IEntityTypeConfiguration<CoursePrerequisite>
{
  public void Configure(EntityTypeBuilder<CoursePrerequisite> builder)
  {
    builder.ToTable("CoursePrerequisites");
    builder.HasKey(cp => new { cp.CourseId, cp.PrerequisiteCourseId });

    builder.HasOne(cp => cp.Course)
           .WithMany(c => c.Prerequisites)
           .HasForeignKey(cp => cp.CourseId)
           .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(cp => cp.PrerequisiteCourse)
           .WithMany()
           .HasForeignKey(cp => cp.PrerequisiteCourseId)
           .OnDelete(DeleteBehavior.Restrict);

    // Prevent a course being prerequisite of itself
    builder.ToTable(t => t.HasCheckConstraint(
        "CK_CoursePrerequisites_NotSelf",
        "[CourseId] <> [PrerequisiteCourseId]"));
  }
}