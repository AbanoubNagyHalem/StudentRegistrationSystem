namespace StudentRegistration.Application.DTOs;

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