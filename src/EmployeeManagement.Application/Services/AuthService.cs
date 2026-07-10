using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public LoginResponseDto Login(LoginRequestDto request)
    {
        var demoUsername = _configuration["DemoLogin:Username"] ?? "admin";
        var demoPassword = _configuration["DemoLogin:Password"] ?? "Admin@123";

        if (string.Equals(request.Username, demoUsername, StringComparison.OrdinalIgnoreCase)
            && request.Password == demoPassword)
        {
            return new LoginResponseDto
            {
                Success = true,
                Message = "Login successful.",
                Username = request.Username,
                Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            };
        }

        return new LoginResponseDto
        {
            Success = false,
            Message = "Invalid username or password."
        };
    }
}
