using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Infrastructure.Data.Configurations;

public class StudyPlanCourseConfiguration : IEntityTypeConfiguration<StudyPlanCourse>
{
  public void Configure(EntityTypeBuilder<StudyPlanCourse> builder)
  {
    builder.ToTable("StudyPlanCourses");
    builder.HasKey(spc => new { spc.StudyPlanId, spc.CourseId });

    builder.HasOne(spc => spc.StudyPlan)
           .WithMany(sp => sp.Courses)
           .HasForeignKey(spc => spc.StudyPlanId)
           .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(spc => spc.Course)
           .WithMany(c => c.StudyPlanCourses)
           .HasForeignKey(spc => spc.CourseId)
           .OnDelete(DeleteBehavior.Restrict);
  }
}