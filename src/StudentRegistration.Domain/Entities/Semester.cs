using StudentRegistration.Domain.Common;

namespace StudentRegistration.Domain.Entities;

public class Semester : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public bool IsCurrent { get; set; }

  // Navigation properties
  public ICollection<CourseSection> Sections { get; set; } = new List<CourseSection>();
  public ICollection<RegistrationPeriod> RegistrationPeriods { get; set; } = new List<RegistrationPeriod>();
}