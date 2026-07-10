using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Web.Services;

public interface IAuthApiClient
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
