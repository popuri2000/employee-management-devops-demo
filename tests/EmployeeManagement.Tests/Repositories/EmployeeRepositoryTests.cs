using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Persistence;
using EmployeeManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EmployeeManagement.Tests.Repositories;

public class EmployeeRepositoryTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static async Task SeedAsync(ApplicationDbContext context)
    {
        context.Employees.AddRange(
            new Employee { Name = "Alice Johnson", Email = "alice@example.com", Department = "Engineering", Designation = "Engineer", Salary = 80000m, JoiningDate = DateTime.Today, IsActive = true },
            new Employee { Name = "Bob Williams", Email = "bob@example.com", Department = "Finance", Designation = "Analyst", Salary = 65000m, JoiningDate = DateTime.Today, IsActive = true },
            new Employee { Name = "Carol Davis", Email = "carol@example.com", Department = "Engineering", Designation = "Manager", Salary = 95000m, JoiningDate = DateTime.Today, IsActive = false }
        );

        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllAsync_FiltersBySearchTerm()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var repository = new EmployeeRepository(context);

        var (items, totalCount) = await repository.GetAllAsync("alice", null, 1, 10);

        Assert.Equal(1, totalCount);
        Assert.Single(items);
        Assert.Equal("Alice Johnson", items[0].Name);
    }

    [Fact]
    public async Task GetAllAsync_FiltersByDepartment()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var repository = new EmployeeRepository(context);

        var (items, totalCount) = await repository.GetAllAsync(null, "Engineering", 1, 10);

        Assert.Equal(2, totalCount);
        Assert.All(items, e => Assert.Equal("Engineering", e.Department));
    }

    [Fact]
    public async Task GetAllAsync_AppliesPagination()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var repository = new EmployeeRepository(context);

        var (items, totalCount) = await repository.GetAllAsync(null, null, 2, 2);

        Assert.Equal(3, totalCount);
        Assert.Single(items);
    }

    [Fact]
    public async Task EmailExistsAsync_ReturnsTrue_WhenEmailIsTaken()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var repository = new EmployeeRepository(context);

        var exists = await repository.EmailExistsAsync("alice@example.com");

        Assert.True(exists);
    }

    [Fact]
    public async Task EmailExistsAsync_ExcludesGivenId()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var repository = new EmployeeRepository(context);
        var alice = await context.Employees.FirstAsync(e => e.Email == "alice@example.com");

        var exists = await repository.EmailExistsAsync("alice@example.com", alice.Id);

        Assert.False(exists);
    }
}
