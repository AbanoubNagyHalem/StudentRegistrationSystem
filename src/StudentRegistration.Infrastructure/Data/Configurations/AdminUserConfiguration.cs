using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Infrastructure.Data.Configurations;

public class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
{
  public void Configure(EntityTypeBuilder<AdminUser> builder)
  {
    builder.ToTable("AdminUsers");
    builder.HasKey(a => a.Id);

    builder.Property(a => a.Username).IsRequired().HasMaxLength(50);
    builder.Property(a => a.PasswordHash).IsRequired().HasMaxLength(255);
    builder.Property(a => a.Role).IsRequired().HasMaxLength(20);

    builder.HasIndex(a => a.Username).IsUnique();
  }
}