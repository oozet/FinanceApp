using FinanceApp.Data;
using FinanceApp.Models;
using FinanceApp.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FinanceApp.Repositories;

public interface IAccountRepositorySQL : IRepository<Account>
{
    public Task<Account> CreateAccountAsync(AccountType type, Guid userId);
    public Task<Account> GetAccountsAsync();
}

public class AccountRepositorySQL : IAccountRepositorySQL
{
    private readonly AppDbContext _context;
    private ILogger _logger;

    public AccountRepositorySQL(AppDbContext context, ILogger<UserRepositorySQL> logger)
    {
        _context = context;
        _logger = logger;
    }

    public int AccountNumber { get; private set; }

    public Task AddAsync(Account entity)
    {
        throw new NotImplementedException();
    }

    // Note: Not checking for valid Guid. Will it ever be invalid?
    public async Task<Account> CreateAccountAsync(AccountType accountType, Guid userId)
    {
        try
        {
            string sql =
                @"INSERT INTO accounts (user_id, account_type) VALUES (@user_id, @type::account_type) RETURNING *";
            try
            {
                await using var connection = new NpgsqlConnection(
                    _context.Database.GetConnectionString()
                );
                await connection.OpenAsync();

                await using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@user_id", userId);
                command.Parameters.AddWithValue("@type", accountType.ToString());

                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Account
                    {
                        AccountNumber = reader.GetInt64(0),
                        UserId = reader.GetGuid(1),
                        BalanceMinorUnit = reader.GetInt64(2),
                        AccountType = (AccountType)
                            Enum.Parse(typeof(AccountType), reader.GetString(3), true),
                        CreatedAt = reader.GetDateTime(4),
                    };
                }
                else
                    return null;
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

    public async Task<Account?> GetByIdAsync(string id)
    {
        if (!int.TryParse(id, out var accountNumber))
        {
            throw new ArgumentException("id is not a valid integer value.");
        }
        string sql = "SELECT * FROM accounts WHERE id = @account_number";

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        try
        {
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@account_number", accountNumber);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Account
                {
                    AccountNumber = reader.GetInt32(0),
                    UserId = reader.GetGuid(1),
                    BalanceMinorUnit = reader.GetInt64(2),
                    CreatedAt = reader.GetDateTime(3),
                };
            }
            else
                return null;
        }
        catch (NpgsqlException ex)
        {
            // Log the exception
            _logger.LogError(ex, "Error creating user in database");
        }
        catch (Exception ex)
        {
            // Catch any unexpected exceptions
            _logger.LogError(ex, "Unexpected error during user creation");
        }

        return null;
    }

    public Task UpdateAsync(Account entity)
    {
        throw new NotImplementedException();
    }
}
