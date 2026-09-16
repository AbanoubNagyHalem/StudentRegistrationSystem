using StudentRegistration.Domain.Common;
using StudentRegistration.Domain.Enums;

namespace StudentRegistration.Domain.Entities;

public class Enrollment : BaseEntity
{
  public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Registered;
  public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

  // FKs
  public int StudentId { get; set; }
  public int SectionId { get; set; }

  // Navigation properties
  public Student Student { get; set; } = null!;
  public CourseSection Section { get; set; } = null!;
}