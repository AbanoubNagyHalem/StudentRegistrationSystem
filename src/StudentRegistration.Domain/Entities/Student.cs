using StudentRegistration.Domain.Common;
using StudentRegistration.Domain.Enums;

namespace StudentRegistration.Domain.Entities;

public class Student : BaseEntity
{
  public string RegistrationNumber { get; set; } = string.Empty;
  public string FullName { get; set; } = string.Empty;
  public string PinHash { get; set; } = string.Empty;
  public StudentStatus Status { get; set; } = StudentStatus.Active;
  public StudentLevel Level { get; set; } = StudentLevel.Master;
  public decimal? TotalAchievement { get; set; }
  public decimal? GPA { get; set; }
  public string? Sponsor { get; set; }

  // FKs
  public int DepartmentId { get; set; }
  public int? StudyPlanId { get; set; }

  // Navigation
  public Department Department { get; set; } = null!;
  public StudyPlan? StudyPlan { get; set; }
  public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
  public ICollection<StudentCompletedCourse> CompletedCourses { get; set; } = new List<StudentCompletedCourse>();
}