namespace StudentRegistration.Client.Models;

// ============================================================
// Error
// ============================================================
public class ApiError
{
  public int StatusCode { get; set; }
  public string Message { get; set; } = string.Empty;
  public string? ErrorCode { get; set; }
  public string? TraceId { get; set; }
}

// ============================================================
// Auth
// ============================================================
public class LoginRequest
{
  public string RegistrationNumber { get; set; } = string.Empty;
  public string Pin { get; set; } = string.Empty;
}

public class LoginResponse
{
  public string Token { get; set; } = string.Empty;
  public DateTime ExpiresAt { get; set; }
  public string FullName { get; set; } = string.Empty;
  public string Role { get; set; } = string.Empty;
  public string? RegistrationNumber { get; set; }
  public int UserId { get; set; }
}

// ============================================================
// Student
// ============================================================
public class StudentProfileDto
{
  public int Id { get; set; }
  public string RegistrationNumber { get; set; } = string.Empty;
  public string FullName { get; set; } = string.Empty;
  public string CollegeName { get; set; } = string.Empty;
  public string DepartmentName { get; set; } = string.Empty;
  public string? StudyPlanName { get; set; }
  public string Status { get; set; } = string.Empty;
  public string Level { get; set; } = string.Empty;
  public decimal? GPA { get; set; }
  public decimal? TotalAchievement { get; set; }
  public string? Sponsor { get; set; }
}

public class StudentDashboardDto
{
  public StudentProfileDto Profile { get; set; } = null!;
  public CurrentSemesterDto? CurrentSemester { get; set; }
  public bool CanRegister { get; set; }
  public string? RegistrationBlockReason { get; set; }
}

public class CurrentSemesterDto
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
}

// ============================================================
// Registration
// ============================================================
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

// ============================================================
// Schedule
// ============================================================
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