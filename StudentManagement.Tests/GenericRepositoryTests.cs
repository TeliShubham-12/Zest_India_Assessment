using Microsoft.EntityFrameworkCore;
using StudentManagement.Domain.Entities;
using StudentManagement.Infrastructure.Data;
using StudentManagement.Infrastructure.Repositories;
using Xunit;

namespace StudentManagement.Tests;

public class GenericRepositoryTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntityToDatabase()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new GenericRepository<Student>(context);
        var student = new Student { Name = "Test Student", Email = "test@example.com", Age = 22, Course = "CS" };

        // Act
        await repository.AddAsync(student);
        await context.SaveChangesAsync();

        // Assert
        var result = await context.Students.FirstOrDefaultAsync(s => s.Email == "test@example.com");
        Assert.NotNull(result);
        Assert.Equal("Test Student", result.Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new GenericRepository<Student>(context);
        await repository.AddAsync(new Student { Name = "Student 1", Email = "s1@example.com", Age = 20, Course = "CS" });
        await repository.AddAsync(new Student { Name = "Student 2", Email = "s2@example.com", Age = 21, Course = "IT" });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectEntity()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new GenericRepository<Student>(context);
        var student = new Student { Name = "Target Student", Email = "target@example.com", Age = 23, Course = "AI" };
        await repository.AddAsync(student);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(student.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Target Student", result.Name);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntityFromDatabase()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new GenericRepository<Student>(context);
        var student = new Student { Name = "Delete Me", Email = "delete@example.com", Age = 24, Course = "Math" };
        await repository.AddAsync(student);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(student);
        await context.SaveChangesAsync();

        // Assert
        var result = await repository.GetByIdAsync(student.Id);
        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrueForExistingEntity()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new GenericRepository<Student>(context);
        var student = new Student { Name = "Exists Test", Email = "exists@example.com", Age = 25, Course = "Physics" };
        await repository.AddAsync(student);
        await context.SaveChangesAsync();

        // Act
        var exists = await repository.ExistsAsync(student.Id);

        // Assert
        Assert.True(exists);
    }
}
