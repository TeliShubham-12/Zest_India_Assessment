using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Student configuration
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Age).IsRequired();
            entity.Property(e => e.Course).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.PasswordSalt).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        // Seed Default Admin User ("admin" / "Admin@123")
        CreatePasswordHash("Admin@123", out byte[] passwordHash, out byte[] passwordSalt);

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "admin",
            Email = "admin@zestindia.com",
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Role = "Admin",
            CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        // Seed Sample Students
        modelBuilder.Entity<Student>().HasData(
            new Student
            {
                Id = 1,
                Name = "Aarav Sharma",
                Email = "aarav.sharma@example.com",
                Age = 21,
                Course = "Computer Science",
                CreatedDate = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            },
            new Student
            {
                Id = 2,
                Name = "Priya Patel",
                Email = "priya.patel@example.com",
                Age = 22,
                Course = "Information Technology",
                CreatedDate = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Student
            {
                Id = 3,
                Name = "Rohan Verma",
                Email = "rohan.verma@example.com",
                Age = 20,
                Course = "Data Science",
                CreatedDate = new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
}