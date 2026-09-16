using StudentRegistration.Domain.Common;
using StudentRegistration.Domain.Enums;

namespace StudentRegistration.Domain.Entities;

public class StudyPlan : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public int Year { get; set; }
  public StudentLevel Level { get; set; } = StudentLevel.Master;

  // FK
  public int DepartmentId { get; set; }

  // Navigation
  public Department Department { get; set; } = null!;
  public ICollection<StudyPlanCourse> Courses { get; set; } = new List<StudyPlanCourse>();
  public ICollection<Student> Students { get; set; } = new List<Student>();
}