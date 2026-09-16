namespace StudentRegistration.Application.DTOs;

// ============================================================
// Students
// ============================================================
public class AdminStudentDto
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
  public string? Sponsor { get; set; }
}

public class CreateStudentRequest
{
  public string RegistrationNumber { get; set; } = string.Empty;
  public string FullName { get; set; } = string.Empty;
  public string Pin { get; set; } = "1234";
  public int DepartmentId { get; set; }
  public int? StudyPlanId { get; set; }
  public int Status { get; set; } = 1;
  public int Level { get; set; } = 2;
  public string? Sponsor { get; set; }
}

public class UpdateStudentRequest
{
  public string FullName { get; set; } = string.Empty;
  public int DepartmentId { get; set; }
  public int? StudyPlanId { get; set; }
  public int Status { get; set; }
  public int Level { get; set; }
  public string? Sponsor { get; set; }
}

public class ResetPinRequest
{
  public string NewPin { get; set; } = string.Empty;
}

// ============================================================
// Registration Periods
// ============================================================
public class AdminRegistrationPeriodDto
{
  public int Id { get; set; }
  public int DepartmentId { get; set; }
  public string DepartmentName { get; set; } = string.Empty;
  public int SemesterId { get; set; }
  public string SemesterName { get; set; } = string.Empty;
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public bool IsOpen { get; set; }
  public bool IsCurrentlyActive { get; set; }
}

public class CreateRegistrationPeriodRequest
{
  public int DepartmentId { get; set; }
  public int SemesterId { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public bool IsOpen { get; set; } = true;
}

public class UpdateRegistrationPeriodRequest
{
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
}

// ============================================================
// Semesters
// ============================================================
public class AdminSemesterDto
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public bool IsCurrent { get; set; }
}

public class CreateSemesterRequest
{
  public string Name { get; set; } = string.Empty;
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public bool IsCurrent { get; set; }
}

// ============================================================
// Courses
// ============================================================
public class AdminCourseDto
{
  public int Id { get; set; }
  public string Code { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public int CreditHours { get; set; }
  public int DepartmentId { get; set; }
  public string DepartmentName { get; set; } = string.Empty;
}

public class CreateCourseRequest
{
  public string Code { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public int CreditHours { get; set; }
  public int DepartmentId { get; set; }
}

public class UpdateCourseRequest
{
  public string Name { get; set; } = string.Empty;
  public int CreditHours { get; set; }
  public int DepartmentId { get; set; }
}

// ============================================================
// Sections
// ============================================================
public class AdminSectionDto
{
  public int Id { get; set; }
  public int CourseId { get; set; }
  public string CourseCode { get; set; } = string.Empty;
  public string CourseName { get; set; } = string.Empty;
  public int SemesterId { get; set; }
  public string SemesterName { get; set; } = string.Empty;
  public string SectionNumber { get; set; } = string.Empty;
  public int Capacity { get; set; }
  public int EnrolledCount { get; set; }
  public int InstructorId { get; set; }
  public string InstructorName { get; set; } = string.Empty;
  public int? TAId { get; set; }
  public string? TAName { get; set; }
  public List<SectionScheduleDto> Schedules { get; set; } = new();
}

public class CreateSectionRequest
{
  public int CourseId { get; set; }
  public int SemesterId { get; set; }
  public string SectionNumber { get; set; } = string.Empty;
  public int Capacity { get; set; }
  public int InstructorId { get; set; }
  public int? TAId { get; set; }
  public List<CreateScheduleItem> Schedules { get; set; } = new();
}

public class CreateScheduleItem
{
  public int DayOfWeek { get; set; }
  public string StartTime { get; set; } = "08:00";
  public string EndTime { get; set; } = "10:00";
  public string? Room { get; set; }
}

public class UpdateSectionRequest
{
  public int Capacity { get; set; }
  public int InstructorId { get; set; }
  public int? TAId { get; set; }
}

// ============================================================
// Departments
// ============================================================
public class AdminDepartmentDto
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Code { get; set; } = string.Empty;
  public int CollegeId { get; set; }
  public string CollegeName { get; set; } = string.Empty;
}

public class CreateDepartmentRequest
{
  public string Name { get; set; } = string.Empty;
  public string Code { get; set; } = string.Empty;
  public int CollegeId { get; set; }
}

// ============================================================
// Colleges
// ============================================================
public class AdminCollegeDto
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Code { get; set; } = string.Empty;
  public int DepartmentsCount { get; set; }
}

// ============================================================
// Instructors
// ============================================================
public class AdminInstructorDto
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Email { get; set; }
}

public class CreateInstructorRequest
{
  public string Name { get; set; } = string.Empty;
  public string? Email { get; set; }
}

// ============================================================
// Teaching Assistants
// ============================================================
public class AdminTADto
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Email { get; set; }
}

public class CreateTARequest
{
  public string Name { get; set; } = string.Empty;
  public string? Email { get; set; }
}