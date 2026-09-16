using StudentRegistration.Domain.Common;

namespace StudentRegistration.Domain.Entities;

public class CourseSection : BaseEntity
{
  public string SectionNumber { get; set; } = string.Empty;
  public int Capacity { get; set; }
  public int EnrolledCount { get; set; }

  // FKs
  public int CourseId { get; set; }
  public int SemesterId { get; set; }
  public int InstructorId { get; set; }
  public int? TAId { get; set; }

  // Navigation properties
  public Course Course { get; set; } = null!;
  public Semester Semester { get; set; } = null!;
  public Instructor Instructor { get; set; } = null!;
  public TeachingAssistant? TeachingAssistant { get; set; }
  public ICollection<SectionSchedule> Schedules { get; set; } = new List<SectionSchedule>();
  public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

  /// <summary>
  /// Returns true if section has available seats.
  /// </summary>
  public bool HasAvailableSeats() => EnrolledCount < Capacity;
}