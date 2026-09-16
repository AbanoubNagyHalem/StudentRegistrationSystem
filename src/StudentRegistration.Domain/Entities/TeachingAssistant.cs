using StudentRegistration.Domain.Common;

namespace StudentRegistration.Domain.Entities;

public class TeachingAssistant : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public string? Email { get; set; }

  // Navigation properties
  public ICollection<CourseSection> Sections { get; set; } = new List<CourseSection>();
}