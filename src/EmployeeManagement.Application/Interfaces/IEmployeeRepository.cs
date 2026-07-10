using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Employee> Items, int TotalCount)> GetAllAsync(
        string? searchTerm,
        string? department,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default);
    void Update(Employee employee);
    void Remove(Employee employee);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
