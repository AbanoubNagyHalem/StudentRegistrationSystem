namespace StudentRegistration.Application.DTOs;

public class RegistrationEligibilityDto
{
  public bool IsEligible { get; set; }
  public string? Reason { get; set; }
  public string? ErrorCode { get; set; }
}

public class AvailableCourseDto
{
  public int CourseId { get; set; }
  public string Code { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public int CreditHours { get; set; }
  public bool IsPrerequisitesMet { get; set; }
  public string? PrerequisiteMissing { get; set; }
  public bool IsAlreadyCompleted { get; set; }
  public List<CourseSectionDto> Sections { get; set; } = new();
}

public class CourseSectionDto
{
  public int Id { get; set; }
  public string SectionNumber { get; set; } = string.Empty;
  public int Capacity { get; set; }
  public int EnrolledCount { get; set; }
  public bool IsFull { get; set; }
  public string InstructorName { get; set; } = string.Empty;
  public string? TeachingAssistantName { get; set; }
  public List<SectionScheduleDto> Schedules { get; set; } = new();
}

public class SectionScheduleDto
{
  public string DayOfWeek { get; set; } = string.Empty;
  public string StartTime { get; set; } = string.Empty;
  public string EndTime { get; set; } = string.Empty;
  public string? Room { get; set; }
}

public class ConfirmRegistrationRequest
{
  public List<int> SectionIds { get; set; } = new();
}

public class RegistrationValidationResult
{
  public bool IsValid { get; set; }
  public List<string> Errors { get; set; } = new();
  public List<string> Warnings { get; set; } = new();
  public int TotalCreditHours { get; set; }
  public int MaxCreditHours { get; set; } = 18;
  public List<ConflictDto> Conflicts { get; set; } = new();
}

public class ConflictDto
{
  public string Message { get; set; } = string.Empty;
  public List<int> SectionIds { get; set; } = new();
}

public class RegistrationSummaryDto
{
  public int EnrollmentsCount { get; set; }
  public int TotalCreditHours { get; set; }
  public DateTime RegisteredAt { get; set; }
  public List<EnrolledCourseDto> Courses { get; set; } = new();
}

public class EnrolledCourseDto
{
  public int EnrollmentId { get; set; }
  public string CourseCode { get; set; } = string.Empty;
  public string CourseName { get; set; } = string.Empty;
  public int CreditHours { get; set; }
  public string SectionNumber { get; set; } = string.Empty;
  public string InstructorName { get; set; } = string.Empty;
}