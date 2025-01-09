using FinanceApp.Data;
using FinanceApp.Models;
using FinanceApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FinanceApp.Repositories;

public interface IAccountRepositorySQL : IRepository<Account>
{
    public Task<Account> CreateAccountAsync(AccountType type, Guid userId);
    public Task<Account> GetAccountsAsync();
    public Task<List<Account>> GetAllAccountsAsync(string userId);
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

    // Note: Not checking for valid Guid. Will it ever be invalid?
    public async Task<Account> CreateAccountAsync(AccountType accountType, Guid userId)
    {
        _logger.LogDebug("Where can I find logs?", accountType, userId);

        string sql =
            @"INSERT INTO accounts (user_id, account_type) VALUES (@user_id, @type::account_type) RETURNING *";

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@user_id", userId);
        command.Parameters.AddWithValue("@type", accountType.ToString().ToLower());

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

        throw new Exception("Unable to create account. reader contains nothing.");
    }

    public async Task<List<Account>> GetAllAccountsAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var userGuid))
        {
            throw new ArgumentException("userId is not a valid Guid.");
        }
        string sql = "SELECT * FROM accounts WHERE user_id = @user_id";
        var accounts = new List<Account>();

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@user_id", userGuid);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            accounts.Add(
                new Account
                {
                    AccountNumber = reader.GetInt64(0),
                    UserId = reader.GetGuid(1),
                    BalanceMinorUnit = reader.GetInt64(2),
                    AccountType = (AccountType)
                        Enum.Parse(typeof(AccountType), reader.GetString(3), true),
                    CreatedAt = reader.GetDateTime(4),
                }
            );
        }
        return accounts;
    }

    public async Task<Account?> GetAsync(long accountNumber)
    {
        string sql = "SELECT * FROM accounts WHERE id = @account_number";

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@account_number", accountNumber);

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

        throw new NullReferenceException($"No account with number: {accountNumber}");
    }

    public async Task<List<TransactionData>> GetTransactionsUsingJoinAsync(AppUser user)
    {
        List<TransactionData> transactions = [];
        string sql =
            @"SELECT * FROM transactions
                JOIN accounts ON accounts.account_number = transactions.account_number WHERE accounts.user_id = @userId
            ";

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@userId", user.Id);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            transactions.Add(
                new()
                {
                    Id = reader.GetGuid(0),
                    AmountMinorUnit = reader.GetInt64(1),
                    AccountNumber = reader.GetInt64(2),
                    TransactionType = (TransactionType)
                        Enum.Parse(typeof(TransactionType), reader.GetString(3), true),
                    CreatedAt = reader.GetDateTime(4),
                }
            );
        }

        return transactions;
    }

    public Task UpdateAsync(Account entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Account>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Account?> GetByIdAsync(Guid id)
    {
        throw new InvalidDataException("Account number should not be a Guid.");
    }

    public Task DeleteAsync(Account entity)
    {
        throw new NotImplementedException();
    }

    public Task<Account> GetAccountsAsync()
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Account entity)
    {
        throw new NotImplementedException();
    }
}
