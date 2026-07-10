using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Infrastructure.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(e => e.Email)
            .IsUnique();

        builder.Property(e => e.Department)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Designation)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Salary)
            .HasColumnType("decimal(18,2)");

        builder.Property(e => e.JoiningDate)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        builder.HasData(
            new Employee
            {
                Id = 1,
                Name = "Ava Thompson",
                Email = "ava.thompson@example.com",
                Department = "Engineering",
                Designation = "Senior Software Engineer",
                Salary = 95000m,
                JoiningDate = new DateTime(2021, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Employee
            {
                Id = 2,
                Name = "Liam Carter",
                Email = "liam.carter@example.com",
                Department = "Human Resources",
                Designation = "HR Manager",
                Salary = 72000m,
                JoiningDate = new DateTime(2019, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Employee
            {
                Id = 3,
                Name = "Sophia Nguyen",
                Email = "sophia.nguyen@example.com",
                Department = "Finance",
                Designation = "Financial Analyst",
                Salary = 68000m,
                JoiningDate = new DateTime(2022, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Employee
            {
                Id = 4,
                Name = "Noah Patel",
                Email = "noah.patel@example.com",
                Department = "Engineering",
                Designation = "DevOps Engineer",
                Salary = 88000m,
                JoiningDate = new DateTime(2020, 11, 5, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Employee
            {
                Id = 5,
                Name = "Isabella Garcia",
                Email = "isabella.garcia@example.com",
                Department = "Marketing",
                Designation = "Marketing Specialist",
                Salary = 60000m,
                JoiningDate = new DateTime(2023, 5, 20, 0, 0, 0, DateTimeKind.Utc),
                IsActive = false
            }
        );
    }
}
