namespace StudentRegistration.Application.DTOs;

public class CurrentSemesterDto
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
}