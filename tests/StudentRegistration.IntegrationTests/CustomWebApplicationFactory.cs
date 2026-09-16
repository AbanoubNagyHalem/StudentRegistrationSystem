using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace StudentRegistration.IntegrationTests;

/// <summary>
/// Uses the real SQL Server database configured in appsettings (dev).
/// The API startup handles migrations + seeding automatically.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Development");
  }
}