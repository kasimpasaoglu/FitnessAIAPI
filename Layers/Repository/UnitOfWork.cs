using API.DataAccessLayer;
using API.DMO;
using Microsoft.EntityFrameworkCore.Storage;

public class UnitOfWork : IUnitOfWork
{
    private readonly FitnessAIContext _context;
    public IGenericRepository<User> User { get; private set; }
    public IGenericRepository<WorkoutPlan> WorkoutPlan { get; private set; }

    public UnitOfWork(FitnessAIContext context)
    {
        _context = context;
        User = new GenericRepository<User>(_context);
        WorkoutPlan = new GenericRepository<WorkoutPlan>(_context);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        var currentTransaction = _context.Database.CurrentTransaction;
        if (currentTransaction != null)
        {
            await currentTransaction.RollbackAsync();
        }
    }

    public async Task CommitTransactionAsync()
    {
        var currentTransaction = _context.Database.CurrentTransaction;
        if (currentTransaction != null)
        {
            await currentTransaction.CommitAsync();
        }
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }


    public void Dispose()
    {
        _context.Dispose();
    }
}