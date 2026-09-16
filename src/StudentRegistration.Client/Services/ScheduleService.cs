using StudentRegistration.Client.Models;

namespace StudentRegistration.Client.Services;

public class ScheduleService
{
  private readonly ApiClient _api;

  public ScheduleService(ApiClient api) => _api = api;

  public Task<ScheduleDto?> GetCurrentScheduleAsync()
      => _api.GetAsync<ScheduleDto>("/api/schedule/current");
}