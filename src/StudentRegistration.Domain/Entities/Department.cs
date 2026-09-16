using StudentRegistration.Domain.Common;

namespace StudentRegistration.Domain.Entities;

public class Department : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public string Code { get; set; } = string.Empty;

  // FK
  public int CollegeId { get; set; }

  // Navigation
  public College College { get; set; } = null!;
  public ICollection<Student> Students { get; set; } = new List<Student>();
  public ICollection<StudyPlan> StudyPlans { get; set; } = new List<StudyPlan>();
  public ICollection<Course> Courses { get; set; } = new List<Course>();
  public ICollection<RegistrationPeriod> RegistrationPeriods { get; set; } = new List<RegistrationPeriod>();
}