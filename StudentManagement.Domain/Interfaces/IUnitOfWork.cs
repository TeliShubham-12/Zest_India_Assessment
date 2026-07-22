using StudentManagement.Domain.Entities;

namespace StudentManagement.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Student> Students { get; }
    IGenericRepository<User> Users { get; }
    Task<int> SaveAsync();
}
