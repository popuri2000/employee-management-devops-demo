using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Application.DTOs;

public class CreateEmployeeDto
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "A valid email address is required.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required.")]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required(ErrorMessage = "Designation is required.")]
    [StringLength(100)]
    public string Designation { get; set; } = string.Empty;

    [Range(0, 100_000_000, ErrorMessage = "Salary must be a positive value.")]
    public decimal Salary { get; set; }

    [Required(ErrorMessage = "Joining date is required.")]
    public DateTime JoiningDate { get; set; }

    public bool IsActive { get; set; } = true;
}
