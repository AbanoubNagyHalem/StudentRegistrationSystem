using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Infrastructure.Data.Configurations;

public class CourseSectionConfiguration : IEntityTypeConfiguration<CourseSection>
{
  public void Configure(EntityTypeBuilder<CourseSection> builder)
  {
    builder.ToTable("CourseSections");
    builder.HasKey(cs => cs.Id);

    builder.Property(cs => cs.SectionNumber).IsRequired().HasMaxLength(10);

    builder.HasIndex(cs => new { cs.CourseId, cs.SemesterId, cs.SectionNumber }).IsUnique();

    builder.ToTable(t =>
    {
      t.HasCheckConstraint("CK_CourseSections_Capacity", "[Capacity] > 0");
      t.HasCheckConstraint("CK_CourseSections_EnrolledCount",
              "[EnrolledCount] >= 0 AND [EnrolledCount] <= [Capacity]");
    });

    builder.HasOne(cs => cs.Course)
           .WithMany(c => c.Sections)
           .HasForeignKey(cs => cs.CourseId)
           .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(cs => cs.Semester)
           .WithMany(s => s.Sections)
           .HasForeignKey(cs => cs.SemesterId)
           .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(cs => cs.Instructor)
           .WithMany(i => i.Sections)
           .HasForeignKey(cs => cs.InstructorId)
           .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(cs => cs.TeachingAssistant)
           .WithMany(ta => ta.Sections)
           .HasForeignKey(cs => cs.TAId)
           .OnDelete(DeleteBehavior.SetNull);
  }
}