using System.ComponentModel.DataAnnotations;

namespace StudentPanel.Models;

public class CreateStudentRequest
{
    [Required(ErrorMessage = "Index number is required.")]
    [StringLength(20, ErrorMessage = "Index number cannot exceed 20 characters.")]
    public string IndexNumber { get; set; } = "";

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    public string FirstName { get; set; } = "";

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
    public string LastName { get; set; } = "";

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    public string Email { get; set; } = "";

    [Range(1, 8, ErrorMessage = "Semester must be between 1 and 8.")]
    public int Semester { get; set; } = 1;
}
