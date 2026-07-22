using System.Security.Cryptography;
using System.Text;
using StudentManagement.Application.Common;
using StudentManagement.Application.DTOs.Auth;
using StudentManagement.Application.Interfaces;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Interfaces;

namespace StudentManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthService(IUnitOfWork unitOfWork, IJwtTokenGenerator tokenGenerator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _tokenGenerator = tokenGenerator ?? throw new ArgumentNullException(nameof(tokenGenerator));
    }

    public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto)
    {
        // Check if username exists
        var existingUsernames = await _unitOfWork.Users.FindAsync(u => u.Username.ToLower() == dto.Username.ToLower());
        if (existingUsernames.Any())
        {
            return ApiResponse<AuthResponseDto>.FailureResult("Username is already taken.");
        }

        // Check if email exists
        var existingEmails = await _unitOfWork.Users.FindAsync(u => u.Email.ToLower() == dto.Email.ToLower());
        if (existingEmails.Any())
        {
            return ApiResponse<AuthResponseDto>.FailureResult("Email address is already registered.");
        }

        CreatePasswordHash(dto.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User
        {
            Username = dto.Username.Trim(),
            Email = dto.Email.Trim(),
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Role = "User",
            CreatedDate = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveAsync();

        var (token, expiresAt) = _tokenGenerator.GenerateToken(user);

        var authResponse = new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = expiresAt
        };

        return ApiResponse<AuthResponseDto>.SuccessResult(authResponse, "User registered successfully.");
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        var users = await _unitOfWork.Users.FindAsync(u => u.Username.ToLower() == dto.Username.ToLower());
        var user = users.FirstOrDefault();

        if (user == null || !VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
        {
            return ApiResponse<AuthResponseDto>.FailureResult("Invalid username or password.");
        }

        var (token, expiresAt) = _tokenGenerator.GenerateToken(user);

        var authResponse = new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = expiresAt
        };

        return ApiResponse<AuthResponseDto>.SuccessResult(authResponse, "Login successful.");
    }

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }
}
