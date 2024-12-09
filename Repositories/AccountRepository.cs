using FinanceApp.Data;
using FinanceApp.Models;
using FinanceApp.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

public interface IAccountRepositorySQL : IRepository<Account>
{
    Task<Guid> GetIdByNameAsync(string name);
    Task<User?> GetUserByNameAndPasswordAsync(string name, string password);
    Task<User?> CreateUserAsync(string name, string password);
}

public class AccountRepositorySQL : IAccountRepositorySQL
{
    public Task AddAsync(Account entity)
    {
        throw new NotImplementedException();
    }

    public Task<User?> CreateUserAsync(string name, string password)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Account entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Account>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Account?> GetByIdAsync(Account id)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> GetIdByNameAsync(string name)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUserByNameAndPasswordAsync(string name, string password)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Account entity)
    {
        throw new NotImplementedException();
    }
}
