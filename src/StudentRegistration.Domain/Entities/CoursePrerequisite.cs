namespace StudentRegistration.Domain.Entities;

/// <summary>
/// Self-referencing Many-to-Many: A Course can have multiple prerequisite Courses.
/// </summary>
public class CoursePrerequisite
{
  public int CourseId { get; set; }
  public int PrerequisiteCourseId { get; set; }

  // Navigation properties
  public Course Course { get; set; } = null!;
  public Course PrerequisiteCourse { get; set; } = null!;
}