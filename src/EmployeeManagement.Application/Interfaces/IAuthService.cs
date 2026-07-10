using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Application.Interfaces;

public interface IAuthService
{
    LoginResponseDto Login(LoginRequestDto request);
}
