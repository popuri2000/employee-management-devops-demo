namespace EmployeeManagement.Application.Common;

public class ApiErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public IDictionary<string, string[]>? Errors { get; set; }
}
