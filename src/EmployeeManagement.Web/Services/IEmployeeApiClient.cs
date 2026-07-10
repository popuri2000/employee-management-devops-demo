using EmployeeManagement.Application.Common;
using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Web.Services;

public interface IEmployeeApiClient
{
    Task<PagedResult<EmployeeDto>?> GetAllAsync(string? searchTerm, string? department, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResult<EmployeeDto>> CreateAsync(CreateEmployeeDto dto, CancellationToken cancellationToken = default);
    Task<ApiResult<EmployeeDto>> UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken cancellationToken = default);
    Task<ApiResult<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public class ApiResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
}
