using API.DMO;
using Microsoft.EntityFrameworkCore.Storage;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<User> User { get; }
    IGenericRepository<WorkoutPlan> WorkoutPlan { get; }

    Task<int> SaveAsync();
    Task RollbackTransactionAsync();
    Task CommitTransactionAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
}