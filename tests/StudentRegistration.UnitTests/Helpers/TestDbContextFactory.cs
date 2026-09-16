using Xunit;
using Microsoft.EntityFrameworkCore;
using StudentRegistration.Infrastructure.Data;

namespace StudentRegistration.UnitTests.Helpers;

public static class TestDbContextFactory
{
  public static AppDbContext CreateInMemory(string dbName)
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: dbName)
        .Options;

    var db = new AppDbContext(options);
    db.Database.EnsureCreated();
    return db;
  }
}