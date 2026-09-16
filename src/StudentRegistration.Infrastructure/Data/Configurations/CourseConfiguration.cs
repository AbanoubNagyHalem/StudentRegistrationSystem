using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Infrastructure.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
  public void Configure(EntityTypeBuilder<Course> builder)
  {
    builder.ToTable("Courses");
    builder.HasKey(c => c.Id);

    builder.Property(c => c.Code).IsRequired().HasMaxLength(20);
    builder.Property(c => c.Name).IsRequired().HasMaxLength(150);

    builder.HasIndex(c => c.Code).IsUnique();

    // CHECK: CreditHours > 0
    builder.ToTable(t => t.HasCheckConstraint("CK_Courses_CreditHours", "[CreditHours] > 0"));

    builder.HasOne(c => c.Department)
           .WithMany(d => d.Courses)
           .HasForeignKey(c => c.DepartmentId)
           .OnDelete(DeleteBehavior.Restrict);
  }
}