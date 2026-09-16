using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Infrastructure.Data.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
  public void Configure(EntityTypeBuilder<Department> builder)
  {
    builder.ToTable("Departments");
    builder.HasKey(d => d.Id);

    builder.Property(d => d.Name).IsRequired().HasMaxLength(150);
    builder.Property(d => d.Code).IsRequired().HasMaxLength(20);

    builder.HasIndex(d => d.Code).IsUnique();

    builder.HasOne(d => d.College)
           .WithMany(c => c.Departments)
           .HasForeignKey(d => d.CollegeId)
           .OnDelete(DeleteBehavior.Restrict);
  }
}