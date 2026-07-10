namespace EmployeeManagement.Application.DTOs;

public class EmployeeQueryParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    public string? SearchTerm { get; set; }
    public string? Department { get; set; }
    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 1 : value;
    }
}
