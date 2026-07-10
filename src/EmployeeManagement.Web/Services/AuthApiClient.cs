using System.Net.Http.Json;
using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Web.Services;

public class AuthApiClient : IAuthApiClient
{
    private readonly HttpClient _httpClient;

    public AuthApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>(cancellationToken: cancellationToken);

        return result ?? new LoginResponseDto { Success = false, Message = "Unable to reach the authentication service." };
    }
}
