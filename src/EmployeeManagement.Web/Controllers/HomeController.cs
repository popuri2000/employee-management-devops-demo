using System.Diagnostics;
using EmployeeManagement.Web.Models;
using EmployeeManagement.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IEmployeeApiClient _employeeApiClient;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IEmployeeApiClient employeeApiClient, ILogger<HomeController> logger)
    {
        _employeeApiClient = employeeApiClient;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var allEmployees = await _employeeApiClient.GetAllAsync(null, null, 1, 1000, cancellationToken);

        if (allEmployees is null)
        {
            return View(new DashboardViewModel());
        }

        var items = allEmployees.Items;

        var viewModel = new DashboardViewModel
        {
            TotalEmployees = items.Count,
            ActiveEmployees = items.Count(e => e.IsActive),
            InactiveEmployees = items.Count(e => !e.IsActive),
            DepartmentCount = items.Select(e => e.Department).Distinct().Count(),
            AverageSalary = items.Count > 0 ? Math.Round(items.Average(e => e.Salary), 2) : 0,
            DepartmentBreakdown = items
                .GroupBy(e => e.Department)
                .Select(g => new DepartmentSummary { Department = g.Key, Count = g.Count() })
                .OrderByDescending(d => d.Count)
                .ToList()
        };

        return View(viewModel);
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
