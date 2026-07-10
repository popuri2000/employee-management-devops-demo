using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Tests.TestHelpers;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace EmployeeManagement.Tests.Services;

public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly EmployeeService _sut;

    public EmployeeServiceTests()
    {
        _repositoryMock = new Mock<IEmployeeRepository>();
        _sut = new EmployeeService(_repositoryMock.Object, MapperFactory.Create(), NullLogger<EmployeeService>.Instance);
    }

    private static Employee CreateEmployee(int id = 1, string email = "jane.doe@example.com") => new()
    {
        Id = id,
        Name = "Jane Doe",
        Email = email,
        Department = "Engineering",
        Designation = "Software Engineer",
        Salary = 75000m,
        JoiningDate = new DateTime(2022, 1, 1),
        IsActive = true
    };

    [Fact]
    public async Task GetByIdAsync_ReturnsMappedDto_WhenEmployeeExists()
    {
        var employee = CreateEmployee();
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(employee);

        var result = await _sut.GetByIdAsync(1);

        Assert.Equal(employee.Id, result.Id);
        Assert.Equal(employee.Email, result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenEmployeeDoesNotExist()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Employee?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByIdAsync(99));
    }

    [Fact]
    public async Task CreateAsync_AddsEmployee_WhenEmailIsUnique()
    {
        _repositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee e, CancellationToken _) => e);

        var dto = new CreateEmployeeDto
        {
            Name = "John Smith",
            Email = "john.smith@example.com",
            Department = "Finance",
            Designation = "Analyst",
            Salary = 60000m,
            JoiningDate = DateTime.Today,
            IsActive = true
        };

        var result = await _sut.CreateAsync(dto);

        Assert.Equal(dto.Email, result.Email);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ThrowsValidationException_WhenEmailAlreadyExists()
    {
        _repositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var dto = new CreateEmployeeDto
        {
            Name = "Duplicate",
            Email = "existing@example.com",
            Department = "Finance",
            Designation = "Analyst",
            Salary = 60000m,
            JoiningDate = DateTime.Today
        };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(dto));
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEmployee_WhenEmployeeExistsAndEmailIsUnique()
    {
        var employee = CreateEmployee();
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(employee);
        _repositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), 1, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var dto = new UpdateEmployeeDto
        {
            Name = "Jane Updated",
            Email = "jane.updated@example.com",
            Department = "Engineering",
            Designation = "Senior Software Engineer",
            Salary = 90000m,
            JoiningDate = employee.JoiningDate,
            IsActive = true
        };

        var result = await _sut.UpdateAsync(1, dto);

        Assert.Equal("Jane Updated", result.Name);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Employee>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFoundException_WhenEmployeeDoesNotExist()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Employee?)null);

        var dto = new UpdateEmployeeDto { Name = "X", Email = "x@example.com", Department = "D", Designation = "T", Salary = 1, JoiningDate = DateTime.Today };

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(99, dto));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsValidationException_WhenNewEmailBelongsToAnotherEmployee()
    {
        var employee = CreateEmployee();
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(employee);
        _repositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var dto = new UpdateEmployeeDto
        {
            Name = "Jane",
            Email = "taken@example.com",
            Department = "Engineering",
            Designation = "Engineer",
            Salary = 80000m,
            JoiningDate = employee.JoiningDate
        };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.UpdateAsync(1, dto));
    }

    [Fact]
    public async Task DeleteAsync_RemovesEmployee_WhenEmployeeExists()
    {
        var employee = CreateEmployee();
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(employee);

        await _sut.DeleteAsync(1);

        _repositoryMock.Verify(r => r.Remove(employee), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsNotFoundException_WhenEmployeeDoesNotExist()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Employee?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(99));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult_WithCorrectPagingMetadata()
    {
        var employees = new List<Employee> { CreateEmployee(1), CreateEmployee(2, "second@example.com") };
        _repositoryMock
            .Setup(r => r.GetAllAsync(null, null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((employees, 2));

        var result = await _sut.GetAllAsync(new EmployeeQueryParameters { PageNumber = 1, PageSize = 10 });

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(1, result.TotalPages);
    }
}
