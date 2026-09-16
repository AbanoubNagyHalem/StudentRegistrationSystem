namespace StudentRegistration.Application.DTOs;

public class ApiErrorResponse
{
  public int StatusCode { get; set; }
  public string Message { get; set; } = string.Empty;
  public string? ErrorCode { get; set; }
  public Dictionary<string, object>? Details { get; set; }
  public string? TraceId { get; set; }
}