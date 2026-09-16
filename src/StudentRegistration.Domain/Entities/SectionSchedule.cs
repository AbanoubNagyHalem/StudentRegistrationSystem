using StudentRegistration.Domain.Common;
using StudentRegistration.Domain.Enums;

namespace StudentRegistration.Domain.Entities;

public class SectionSchedule : BaseEntity
{
  public WeekDay DayOfWeek { get; set; }
  public TimeOnly StartTime { get; set; }
  public TimeOnly EndTime { get; set; }
  public string? Room { get; set; }

  // FK
  public int SectionId { get; set; }

  // Navigation property
  public CourseSection Section { get; set; } = null!;
}