using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Infrastructure.Data.Configurations;

public class StudyPlanConfiguration : IEntityTypeConfiguration<StudyPlan>
{
  public void Configure(EntityTypeBuilder<StudyPlan> builder)
  {
    builder.ToTable("StudyPlans");
    builder.HasKey(sp => sp.Id);

    builder.Property(sp => sp.Name).IsRequired().HasMaxLength(150);

    builder.HasOne(sp => sp.Department)
           .WithMany(d => d.StudyPlans)
           .HasForeignKey(sp => sp.DepartmentId)
           .OnDelete(DeleteBehavior.Restrict);
  }
}