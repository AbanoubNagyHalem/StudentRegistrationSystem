namespace StudentRegistration.Client.Models;

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

public class AdminSemesterDto
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public bool IsCurrent { get; set; }
}

public class AdminCourseDto
{
  public int Id { get; set; }
  public string Code { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public int CreditHours { get; set; }
  public int DepartmentId { get; set; }
  public string DepartmentName { get; set; } = string.Empty;
}

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

public class AdminDepartmentDto
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Code { get; set; } = string.Empty;
  public int CollegeId { get; set; }
  public string CollegeName { get; set; } = string.Empty;
}

public class AdminCollegeDto
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Code { get; set; } = string.Empty;
  public int DepartmentsCount { get; set; }
}

public class AdminInstructorDto
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Email { get; set; }
}

public class AdminTADto
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Email { get; set; }
}