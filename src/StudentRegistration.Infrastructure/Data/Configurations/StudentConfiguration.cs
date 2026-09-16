using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Infrastructure.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
  public void Configure(EntityTypeBuilder<Student> builder)
  {
    builder.ToTable("Students");
    builder.HasKey(s => s.Id);

    builder.Property(s => s.RegistrationNumber).IsRequired().HasMaxLength(20);
    builder.Property(s => s.FullName).IsRequired().HasMaxLength(150);
    builder.Property(s => s.PinHash).IsRequired().HasMaxLength(255);
    builder.Property(s => s.Sponsor).HasMaxLength(100);
    builder.Property(s => s.TotalAchievement).HasColumnType("decimal(5,2)");
    builder.Property(s => s.GPA).HasColumnType("decimal(4,2)");

    builder.HasIndex(s => s.RegistrationNumber).IsUnique();

    builder.HasOne(s => s.Department)
           .WithMany(d => d.Students)
           .HasForeignKey(s => s.DepartmentId)
           .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(s => s.StudyPlan)
           .WithMany(sp => sp.Students)
           .HasForeignKey(s => s.StudyPlanId)
           .OnDelete(DeleteBehavior.SetNull);
  }
}