
using System.Linq.Expressions;
using API.DataAccessLayer;
using Microsoft.EntityFrameworkCore;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly FitnessAIContext _context;
    protected readonly DbSet<T> _dbSet;
    public GenericRepository(FitnessAIContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public Task<int> DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return _context.SaveChangesAsync();
    }

    public Task<IEnumerable<T>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<T> GetByUuidAsync(Guid uuid)
    {
        return await _dbSet.FindAsync(uuid);
    }

    public async Task<int> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return await _context.SaveChangesAsync();
    }

    public async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }
}