using StudentManagement.Application.Common;
using StudentManagement.Application.DTOs.Student;
using StudentManagement.Application.Interfaces;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Interfaces;

namespace StudentManagement.Application.Services;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApiResponse<IEnumerable<StudentDto>>> GetAllStudentsAsync()
    {
        var students = await _unitOfWork.Students.GetAllAsync();
        var dtos = students.Select(MapToDto);
        return ApiResponse<IEnumerable<StudentDto>>.SuccessResult(dtos, "Students retrieved successfully.");
    }

    public async Task<ApiResponse<StudentDto>> GetStudentByIdAsync(int id)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id);
        if (student == null)
        {
            return ApiResponse<StudentDto>.FailureResult($"Student with ID {id} was not found.");
        }

        return ApiResponse<StudentDto>.SuccessResult(MapToDto(student), "Student retrieved successfully.");
    }

    public async Task<ApiResponse<StudentDto>> CreateStudentAsync(CreateStudentDto dto)
    {
        // Check email uniqueness
        var existingEmail = await _unitOfWork.Students.FindAsync(s => s.Email.ToLower() == dto.Email.ToLower());
        if (existingEmail.Any())
        {
            return ApiResponse<StudentDto>.FailureResult($"A student with email '{dto.Email}' already exists.");
        }

        var student = new Student
        {
            Name = dto.Name.Trim(),
            Email = dto.Email.Trim().ToLower(),
            Age = dto.Age,
            Course = dto.Course.Trim(),
            CreatedDate = DateTime.UtcNow
        };

        await _unitOfWork.Students.AddAsync(student);
        await _unitOfWork.SaveAsync();

        return ApiResponse<StudentDto>.SuccessResult(MapToDto(student), "Student created successfully.");
    }

    public async Task<ApiResponse<StudentDto>> UpdateStudentAsync(int id, UpdateStudentDto dto)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id);
        if (student == null)
        {
            return ApiResponse<StudentDto>.FailureResult($"Student with ID {id} was not found.");
        }

        // Check email uniqueness if email is changed
        if (!string.Equals(student.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existingEmail = await _unitOfWork.Students.FindAsync(s => s.Email.ToLower() == dto.Email.ToLower() && s.Id != id);
            if (existingEmail.Any())
            {
                return ApiResponse<StudentDto>.FailureResult($"A student with email '{dto.Email}' already exists.");
            }
        }

        student.Name = dto.Name.Trim();
        student.Email = dto.Email.Trim().ToLower();
        student.Age = dto.Age;
        student.Course = dto.Course.Trim();

        await _unitOfWork.Students.UpdateAsync(student);
        await _unitOfWork.SaveAsync();

        return ApiResponse<StudentDto>.SuccessResult(MapToDto(student), "Student updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteStudentAsync(int id)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id);
        if (student == null)
        {
            return ApiResponse<bool>.FailureResult($"Student with ID {id} was not found.");
        }

        await _unitOfWork.Students.DeleteAsync(student);
        await _unitOfWork.SaveAsync();

        return ApiResponse<bool>.SuccessResult(true, "Student deleted successfully.");
    }

    private static StudentDto MapToDto(Student student)
    {
        return new StudentDto
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            Age = student.Age,
            Course = student.Course,
            CreatedDate = student.CreatedDate
        };
    }
}
