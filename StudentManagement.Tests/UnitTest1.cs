using Microsoft.EntityFrameworkCore;
using StudentManagement.Application.DTOs.Auth;
using StudentManagement.Application.Services;
using StudentManagement.Infrastructure.Data;
using StudentManagement.Infrastructure.Repositories;
using Xunit;

namespace StudentManagement.Tests;

public class AuthServiceTests
{
    private (ApplicationDbContext context, AuthService service) GetService()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        var unitOfWork = new UnitOfWork(context);

        // Mock IConfiguration with Moq
        var configMock = new Moq.Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        configMock.Setup(c => c["Jwt:SecretKey"]).Returns("TestSecretKeyForAuthServiceTests123456789!");
        configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
        configMock.Setup(c => c["Jwt:ExpiryInMinutes"]).Returns("60");

        var tokenGenerator = new JwtTokenGenerator(configMock.Object);
        var authService = new AuthService(unitOfWork, tokenGenerator);

        return (context, authService);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUserAndReturnJwtToken()
    {
        // Arrange
        var (context, service) = GetService();
        var dto = new RegisterDto { Username = "newuser", Email = "newuser@example.com", Password = "Password123" };

        // Act
        var result = await service.RegisterAsync(dto);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.False(string.IsNullOrWhiteSpace(result.Data.Token));
        Assert.Equal("newuser", result.Data.Username);
    }

    [Fact]
    public async Task LoginAsync_ShouldAuthenticateUser_WithCorrectCredentials()
    {
        // Arrange
        var (context, service) = GetService();
        await service.RegisterAsync(new RegisterDto { Username = "validuser", Email = "valid@example.com", Password = "MyPassword123" });

        // Act
        var result = await service.LoginAsync(new LoginDto { Username = "validuser", Password = "MyPassword123" });

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.False(string.IsNullOrWhiteSpace(result.Data.Token));
    }

    [Fact]
    public async Task LoginAsync_ShouldFail_WithIncorrectPassword()
    {
        // Arrange
        var (context, service) = GetService();
        await service.RegisterAsync(new RegisterDto { Username = "validuser", Email = "valid@example.com", Password = "MyPassword123" });

        // Act
        var result = await service.LoginAsync(new LoginDto { Username = "validuser", Password = "WrongPassword" });

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Invalid username or password", result.Message, StringComparison.OrdinalIgnoreCase);
    }
}
