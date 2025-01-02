using FinanceApp.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Repositories;

public interface IRepository<T>
    where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}

public abstract class Repository<T> : IRepository<T>
    where T : class
{
    protected readonly AppDbContext _context;
    protected ILogger<Repository<T>> _logger;

    public Repository(AppDbContext context, ILogger<Repository<T>> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _context.Set<T>().FindAsync(id);
    }
}
