using StudentManagement.Domain.Entities;

namespace StudentManagement.Application.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}
