using System.Net;
using System.Net.Http.Json;
using EmployeeManagement.Application.Common;
using EmployeeManagement.Application.DTOs;

namespace EmployeeManagement.Web.Services;

public class EmployeeApiClient : IEmployeeApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EmployeeApiClient> _logger;

    public EmployeeApiClient(HttpClient httpClient, ILogger<EmployeeApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PagedResult<EmployeeDto>?> GetAllAsync(string? searchTerm, string? department, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = new List<string> { $"pageNumber={pageNumber}", $"pageSize={pageSize}" };

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
        }

        if (!string.IsNullOrWhiteSpace(department))
        {
            query.Add($"department={Uri.EscapeDataString(department)}");
        }

        var response = await _httpClient.GetAsync($"api/v1/employees?{string.Join("&", query)}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Failed to fetch employees. Status: {StatusCode}", response.StatusCode);
            return null;
        }

        return await response.Content.ReadFromJsonAsync<PagedResult<EmployeeDto>>(cancellationToken: cancellationToken);
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/v1/employees/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EmployeeDto>(cancellationToken: cancellationToken);
    }

    public async Task<ApiResult<EmployeeDto>> CreateAsync(CreateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/v1/employees", dto, cancellationToken);
        return await BuildResultAsync<EmployeeDto>(response, cancellationToken);
    }

    public async Task<ApiResult<EmployeeDto>> UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/v1/employees/{id}", dto, cancellationToken);
        return await BuildResultAsync<EmployeeDto>(response, cancellationToken);
    }

    public async Task<ApiResult<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"api/v1/employees/{id}", cancellationToken);
        return await BuildResultAsync<bool>(response, cancellationToken, successData: true);
    }

    private static async Task<ApiResult<T>> BuildResultAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken, T? successData = default)
    {
        if (response.IsSuccessStatusCode)
        {
            var data = successData is not null
                ? successData
                : await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);

            return new ApiResult<T> { Success = true, Data = data };
        }

        var errorMessage = "An error occurred while communicating with the API.";

        try
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(cancellationToken: cancellationToken);
            if (error is not null)
            {
                errorMessage = error.Message;
            }
        }
        catch
        {
            // Response body was not a well-formed ApiErrorResponse; fall back to default message.
        }

        return new ApiResult<T> { Success = false, ErrorMessage = errorMessage };
    }
}
