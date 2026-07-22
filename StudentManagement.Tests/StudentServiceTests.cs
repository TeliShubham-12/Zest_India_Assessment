using Microsoft.EntityFrameworkCore;
using StudentManagement.Application.DTOs.Student;
using StudentManagement.Application.Services;
using StudentManagement.Domain.Entities;
using StudentManagement.Infrastructure.Data;
using StudentManagement.Infrastructure.Repositories;
using Xunit;

namespace StudentManagement.Tests;

public class StudentServiceTests
{
    private (ApplicationDbContext context, StudentService service) GetService()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        var unitOfWork = new UnitOfWork(context);
        var service = new StudentService(unitOfWork);

        return (context, service);
    }

    [Fact]
    public async Task CreateStudentAsync_ShouldCreateStudent_WhenValidDtoProvided()
    {
        // Arrange
        var (context, service) = GetService();
        var dto = new CreateStudentDto { Name = "John Doe", Email = "john@example.com", Age = 20, Course = "CS" };

        // Act
        var result = await service.CreateStudentAsync(dto);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("John Doe", result.Data.Name);
        Assert.Equal("john@example.com", result.Data.Email);
    }

    [Fact]
    public async Task CreateStudentAsync_ShouldFail_WhenDuplicateEmailProvided()
    {
        // Arrange
        var (context, service) = GetService();
        var dto1 = new CreateStudentDto { Name = "John", Email = "duplicate@example.com", Age = 20, Course = "CS" };
        var dto2 = new CreateStudentDto { Name = "Jane", Email = "duplicate@example.com", Age = 21, Course = "IT" };

        await service.CreateStudentAsync(dto1);

        // Act
        var result = await service.CreateStudentAsync(dto2);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("already exists", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetAllStudentsAsync_ShouldReturnAllStudents()
    {
        // Arrange
        var (context, service) = GetService();
        await service.CreateStudentAsync(new CreateStudentDto { Name = "Student A", Email = "a@example.com", Age = 20, Course = "CS" });
        await service.CreateStudentAsync(new CreateStudentDto { Name = "Student B", Email = "b@example.com", Age = 22, Course = "IT" });

        // Act
        var result = await service.GetAllStudentsAsync();

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Data!.Count());
    }

    [Fact]
    public async Task UpdateStudentAsync_ShouldUpdateExistingStudent()
    {
        // Arrange
        var (context, service) = GetService();
        var createResult = await service.CreateStudentAsync(new CreateStudentDto { Name = "Old Name", Email = "old@example.com", Age = 20, Course = "Math" });
        int studentId = createResult.Data!.Id;

        var updateDto = new UpdateStudentDto { Name = "New Name", Email = "new@example.com", Age = 21, Course = "Physics" };

        // Act
        var updateResult = await service.UpdateStudentAsync(studentId, updateDto);

        // Assert
        Assert.True(updateResult.Success);
        Assert.Equal("New Name", updateResult.Data!.Name);
        Assert.Equal("new@example.com", updateResult.Data!.Email);
    }

    [Fact]
    public async Task DeleteStudentAsync_ShouldRemoveStudent()
    {
        // Arrange
        var (context, service) = GetService();
        var createResult = await service.CreateStudentAsync(new CreateStudentDto { Name = "To Delete", Email = "delete@example.com", Age = 22, Course = "Bio" });
        int id = createResult.Data!.Id;

        // Act
        var deleteResult = await service.DeleteStudentAsync(id);

        // Assert
        Assert.True(deleteResult.Success);

        var getResult = await service.GetStudentByIdAsync(id);
        Assert.False(getResult.Success);
    }
}
