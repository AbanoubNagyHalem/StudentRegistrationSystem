namespace StudentRegistration.Domain.Entities;

/// <summary>
/// Represents a course that a student has already completed successfully.
/// Used for prerequisite validation.
/// </summary>
public class StudentCompletedCourse
{
  public int StudentId { get; set; }
  public int CourseId { get; set; }
  public string Grade { get; set; } = string.Empty;
  public DateTime CompletedAt { get; set; }

  // Navigation properties
  public Student Student { get; set; } = null!;
  public Course Course { get; set; } = null!;
}