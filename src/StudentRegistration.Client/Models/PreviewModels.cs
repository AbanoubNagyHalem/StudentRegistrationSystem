namespace StudentRegistration.Client.Models;

public class PreviewItem
{
  public int SectionId { get; set; }
  public string CourseCode { get; set; } = string.Empty;
  public string CourseName { get; set; } = string.Empty;
  public string SectionNumber { get; set; } = string.Empty;
  public string InstructorName { get; set; } = string.Empty;
  public string? TeachingAssistantName { get; set; }
  public string DayOfWeek { get; set; } = string.Empty;
  public string StartTime { get; set; } = string.Empty;
  public string EndTime { get; set; } = string.Empty;
  public string? Room { get; set; }
  public bool IsLab => Room?.StartsWith("Lab-", StringComparison.OrdinalIgnoreCase) ?? false;
}