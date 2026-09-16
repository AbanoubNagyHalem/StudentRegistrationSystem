namespace StudentRegistration.Domain.Entities;

public class StudyPlanCourse
{
  public int StudyPlanId { get; set; }
  public int CourseId { get; set; }
  public int SuggestedTerm { get; set; }
  public bool IsMandatory { get; set; } = true;

  public StudyPlan StudyPlan { get; set; } = null!;
  public Course Course { get; set; } = null!;
}