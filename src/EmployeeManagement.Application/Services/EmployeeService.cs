using AutoMapper;
using EmployeeManagement.Application.Common;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper, ILogger<EmployeeService> logger)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<EmployeeDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<PagedResult<EmployeeDto>> GetAllAsync(EmployeeQueryParameters queryParameters, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _employeeRepository.GetAllAsync(
            queryParameters.SearchTerm,
            queryParameters.Department,
            queryParameters.PageNumber,
            queryParameters.PageSize,
            cancellationToken);

        return new PagedResult<EmployeeDto>
        {
            Items = _mapper.Map<IReadOnlyList<EmployeeDto>>(items),
            TotalCount = totalCount,
            PageNumber = queryParameters.PageNumber,
            PageSize = queryParameters.PageSize
        };
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        if (await _employeeRepository.EmailExistsAsync(dto.Email, cancellationToken: cancellationToken))
        {
            throw new ValidationException($"An employee with email '{dto.Email}' already exists.");
        }

        var employee = _mapper.Map<Employee>(dto);
        await _employeeRepository.AddAsync(employee, cancellationToken);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created employee {EmployeeId} - {EmployeeName}", employee.Id, employee.Name);

        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        if (await _employeeRepository.EmailExistsAsync(dto.Email, id, cancellationToken))
        {
            throw new ValidationException($"An employee with email '{dto.Email}' already exists.");
        }

        _mapper.Map(dto, employee);
        _employeeRepository.Update(employee);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated employee {EmployeeId}", employee.Id);

        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        _employeeRepository.Remove(employee);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted employee {EmployeeId}", id);
    }
}
