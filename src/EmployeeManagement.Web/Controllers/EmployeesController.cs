using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Web.Controllers;

[Authorize]
public class EmployeesController : Controller
{
    private readonly IEmployeeApiClient _employeeApiClient;

    public EmployeesController(IEmployeeApiClient employeeApiClient)
    {
        _employeeApiClient = employeeApiClient;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> List(string? searchTerm, string? department, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _employeeApiClient.GetAllAsync(searchTerm, department, pageNumber, pageSize, cancellationToken);
        return Json(result);
    }

    public IActionResult Create()
    {
        return View(new CreateEmployeeDto { JoiningDate = DateTime.Today, IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { success = false, message = "Please correct the highlighted fields." });
        }

        var result = await _employeeApiClient.CreateAsync(dto, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new { success = false, message = result.ErrorMessage });
        }

        return Ok(new { success = true, message = "Employee created successfully.", data = result.Data });
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var employee = await _employeeApiClient.GetByIdAsync(id, cancellationToken);

        if (employee is null)
        {
            return NotFound();
        }

        return View(employee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [FromBody] UpdateEmployeeDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { success = false, message = "Please correct the highlighted fields." });
        }

        var result = await _employeeApiClient.UpdateAsync(id, dto, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new { success = false, message = result.ErrorMessage });
        }

        return Ok(new { success = true, message = "Employee updated successfully.", data = result.Data });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _employeeApiClient.DeleteAsync(id, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new { success = false, message = result.ErrorMessage });
        }

        return Ok(new { success = true, message = "Employee deleted successfully." });
    }
}
