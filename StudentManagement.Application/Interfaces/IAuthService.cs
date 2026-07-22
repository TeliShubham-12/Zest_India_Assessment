using StudentManagement.Application.Common;
using StudentManagement.Application.DTOs.Auth;

namespace StudentManagement.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto);
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto);
}
