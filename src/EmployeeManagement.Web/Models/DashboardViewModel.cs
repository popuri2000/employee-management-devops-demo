namespace EmployeeManagement.Web.Models;

public class DashboardViewModel
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public int InactiveEmployees { get; set; }
    public int DepartmentCount { get; set; }
    public decimal AverageSalary { get; set; }
    public IReadOnlyList<DepartmentSummary> DepartmentBreakdown { get; set; } = Array.Empty<DepartmentSummary>();
}

public class DepartmentSummary
{
    public string Department { get; set; } = string.Empty;
    public int Count { get; set; }
}
