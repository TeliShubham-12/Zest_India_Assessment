using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Application.DTOs.Student;

public class UpdateStudentDto
{
    [Required(ErrorMessage = "Student Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Age is required.")]
    [Range(1, 120, ErrorMessage = "Age must be between 1 and 120.")]
    public int Age { get; set; }

    [Required(ErrorMessage = "Course is required.")]
    [StringLength(100, ErrorMessage = "Course name cannot exceed 100 characters.")]
    public string Course { get; set; } = string.Empty;
}
