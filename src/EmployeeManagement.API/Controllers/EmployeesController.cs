using Asp.Versioning;
using EmployeeManagement.Application.Common;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>Gets a paged, filterable list of employees.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<EmployeeDto>>> GetAll([FromQuery] EmployeeQueryParameters queryParameters, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetAllAsync(queryParameters, cancellationToken);
        return Ok(result);
    }

    /// <summary>Gets a single employee by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var employee = await _employeeService.GetByIdAsync(id, cancellationToken);
        return Ok(employee);
    }

    /// <summary>Creates a new employee.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmployeeDto>> Create([FromBody] CreateEmployeeDto dto, CancellationToken cancellationToken)
    {
        var created = await _employeeService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Updates an existing employee.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeDto>> Update(int id, [FromBody] UpdateEmployeeDto dto, CancellationToken cancellationToken)
    {
        var updated = await _employeeService.UpdateAsync(id, dto, cancellationToken);
        return Ok(updated);
    }

    /// <summary>Deletes an employee.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _employeeService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
