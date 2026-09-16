using StudentRegistration.Domain.Common;

namespace StudentRegistration.Domain.Entities;

public class Course : BaseEntity
{
  public string Code { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public int CreditHours { get; set; }

  // FK
  public int DepartmentId { get; set; }

  // Navigation properties
  public Department Department { get; set; } = null!;
  public ICollection<CoursePrerequisite> Prerequisites { get; set; } = new List<CoursePrerequisite>();
  public ICollection<CourseSection> Sections { get; set; } = new List<CourseSection>();
  public ICollection<StudyPlanCourse> StudyPlanCourses { get; set; } = new List<StudyPlanCourse>();
}