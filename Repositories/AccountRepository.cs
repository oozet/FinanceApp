using FinanceApp.Data;
using FinanceApp.Models;
using FinanceApp.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

public interface IAccountRepositorySQL : IRepository<Account>
{
    public Task<Account> CreateAccountAsync(AccountType type, Guid id);
    public Task<Account> GetAccountsAsync()
}

public class AccountRepositorySQL : IAccountRepositorySQL
{
    private readonly AppDbContext _context;
    private ILogger _logger;
    private AppUser _user;

    public AccountRepositorySQL(AppDbContext context, ILogger<UserRepositorySQL> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Task AddAsync(Account entity)
    {
        throw new NotImplementedException();
    }

    public async Task<Account> CreateAccountAsync(AccountType accountType, Guid id)
    {
        try
        {
            string sql =
                @"INSERT INTO accounts (user_id, type, balance_minor_unit) VALUES (@user_id, @type, 0)";
            try
            {
                await using var connection = new NpgsqlConnection(
                    _context.Database.GetConnectionString()
                );
                await connection.OpenAsync();

                await using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@user_id", id);
                command.Parameters.AddWithValue("@type", accountType);

                await command.ExecuteNonQueryAsync();
                return newUser;
            }
            catch (NpgsqlException ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error creating user in database");
                return null;
            }
            catch (Exception ex)
            {
                // Catch any unexpected exceptions
                _logger.LogError(ex, "Unexpected error during user creation");
                return null;
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions in the method (e.g., from GetIdByNameAsync)
            _logger.LogError(ex, "Error in user creation process");
            return null;
        }
    }

    public Task DeleteAsync(Account entity)
    {
        throw new NotImplementedException();
    }

    public Task<Account> GetAccountsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Account>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Account?> GetByGuidAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Account?> GetByIdAsync(Account id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Account entity)
    {
        throw new NotImplementedException();
    }
}
