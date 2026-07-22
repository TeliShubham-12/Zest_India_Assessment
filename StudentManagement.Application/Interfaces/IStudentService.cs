using StudentManagement.Application.Common;
using StudentManagement.Application.DTOs.Student;

namespace StudentManagement.Application.Interfaces;

public interface IStudentService
{
    Task<ApiResponse<IEnumerable<StudentDto>>> GetAllStudentsAsync();
    Task<ApiResponse<StudentDto>> GetStudentByIdAsync(int id);
    Task<ApiResponse<StudentDto>> CreateStudentAsync(CreateStudentDto dto);
    Task<ApiResponse<StudentDto>> UpdateStudentAsync(int id, UpdateStudentDto dto);
    Task<ApiResponse<bool>> DeleteStudentAsync(int id);
}
