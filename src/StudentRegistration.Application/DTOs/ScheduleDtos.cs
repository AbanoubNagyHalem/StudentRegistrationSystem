namespace StudentRegistration.Application.DTOs;

public class ScheduleDto
{
  public int StudentId { get; set; }
  public string StudentName { get; set; } = string.Empty;
  public string RegistrationNumber { get; set; } = string.Empty;
  public string DepartmentName { get; set; } = string.Empty;
  public string SemesterName { get; set; } = string.Empty;
  public int TotalCreditHours { get; set; }
  public List<ScheduleItemDto> Items { get; set; } = new();
}

public class ScheduleItemDto
{
  public string CourseCode { get; set; } = string.Empty;
  public string CourseName { get; set; } = string.Empty;
  public string SectionNumber { get; set; } = string.Empty;
  public string InstructorName { get; set; } = string.Empty;
  public string? TeachingAssistantName { get; set; }
  public string DayOfWeek { get; set; } = string.Empty;
  public string StartTime { get; set; } = string.Empty;
  public string EndTime { get; set; } = string.Empty;
  public string? Room { get; set; }
}