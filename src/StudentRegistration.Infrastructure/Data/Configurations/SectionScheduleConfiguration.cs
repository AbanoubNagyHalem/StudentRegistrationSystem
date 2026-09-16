using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Infrastructure.Data.Configurations;

public class SectionScheduleConfiguration : IEntityTypeConfiguration<SectionSchedule>
{
  public void Configure(EntityTypeBuilder<SectionSchedule> builder)
  {
    builder.ToTable("SectionSchedules");
    builder.HasKey(ss => ss.Id);

    builder.Property(ss => ss.Room).HasMaxLength(50);

    // CHECK: StartTime < EndTime
    builder.ToTable(t => t.HasCheckConstraint(
        "CK_SectionSchedules_TimeOrder",
        "[StartTime] < [EndTime]"));

    builder.HasOne(ss => ss.Section)
           .WithMany(cs => cs.Schedules)
           .HasForeignKey(ss => ss.SectionId)
           .OnDelete(DeleteBehavior.Cascade);
  }
}