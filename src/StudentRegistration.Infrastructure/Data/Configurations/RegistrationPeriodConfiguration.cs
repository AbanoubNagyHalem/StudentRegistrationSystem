using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Infrastructure.Data.Configurations;

public class RegistrationPeriodConfiguration : IEntityTypeConfiguration<RegistrationPeriod>
{
  public void Configure(EntityTypeBuilder<RegistrationPeriod> builder)
  {
    builder.ToTable("RegistrationPeriods");
    builder.HasKey(rp => rp.Id);

    builder.HasIndex(rp => new { rp.DepartmentId, rp.SemesterId }).IsUnique();

    builder.HasOne(rp => rp.Department)
           .WithMany(d => d.RegistrationPeriods)
           .HasForeignKey(rp => rp.DepartmentId)
           .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(rp => rp.Semester)
           .WithMany(s => s.RegistrationPeriods)
           .HasForeignKey(rp => rp.SemesterId)
           .OnDelete(DeleteBehavior.Cascade);
  }
}