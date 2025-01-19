using FinanceApp.Data;
using FinanceApp.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FinanceApp.Repositories;

public interface ITransactionRepository : IRepository<TransactionData> { }

public class TransactionRepository : Repository<TransactionData>
{
    public TransactionRepository(AppDbContext context, ILogger<Repository<TransactionData>> logger)
        : base(context, logger) { }

    public async Task<int> AddAsync(TransactionData entity)
    {
        string sql =
            @"
                BEGIN;
                INSERT INTO transactions (id, amount_minor_unit, account_number, transaction_type, created_at)
                VALUES (@id, @amount_minor_unit, @account_number, @transaction_type::transaction_type, @created_at);
                
                UPDATE accounts
                SET balance_minor_unit = balance_minor_unit + (@amount_minor_unit * (CASE WHEN @transaction_type = 'deposit' THEN 1 ELSE -1 END))
                WHERE account_number = @account_number;
                COMMIT;
                ";
        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", entity.Id);
        command.Parameters.AddWithValue("@amount_minor_unit", entity.AmountMinorUnit);
        command.Parameters.AddWithValue("@account_number", entity.AccountNumber);
        command.Parameters.AddWithValue(
            "@transaction_type",
            entity.TransactionType.ToString().ToLower()
        );
        command.Parameters.AddWithValue("@created_at", entity.CreatedAt);

        return await command.ExecuteNonQueryAsync();
    }

    public async Task<int> AddTransactionEntriesAsync(List<TransactionData> entities)
    {
        var sql =
            @"
            BEGIN;

            INSERT INTO transactions (id, amount_minor_unit, account_number, transaction_type, created_at)
            VALUES ";

        var values = new List<string>();
        for (var i = 0; i < entities.Count; i++)
        {
            values.Add(
                $@"(@id{i}, @amount_minor_unit{i}, @account_number{i}, @transaction_type{i}::transaction_type, @created_at{i})"
            );
        }
        sql += string.Join(", ", values) + ";";

        // Update account balances
        for (var i = 0; i < entities.Count; i++)
        {
            sql +=
                $@"
                UPDATE accounts
                SET balance_minor_unit = balance_minor_unit + (@amount_minor_unit{i} * (CASE WHEN @transaction_type{i} = 'deposit' THEN 1 ELSE -1 END))
                WHERE account_number = @account_number{i};
                ";
        }
        sql += "COMMIT;";

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        for (var i = 0; i < entities.Count; i++)
        {
            command.Parameters.AddWithValue($"@id{i}", entities[i].Id);
            command.Parameters.AddWithValue($"@amount_minor_unit{i}", entities[i].AmountMinorUnit);
            command.Parameters.AddWithValue($"@account_number{i}", entities[i].AccountNumber);
            command.Parameters.AddWithValue(
                $"@transaction_type{i}",
                entities[i].TransactionType.ToString().ToLower()
            );
            command.Parameters.AddWithValue($"@created_at{i}", entities[i].CreatedAt);
        }

        return await command.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<TransactionData>> GetAllFromAccountAsync(long accountNumber)
    {
        string sql =
            "SELECT * FROM transactions WHERE account_number = @account_number ORDER BY created_at DESC";

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@account_number", accountNumber);

        using var reader = await command.ExecuteReaderAsync();

        var transactions = new List<TransactionData>();
        while (await reader.ReadAsync())
        {
            transactions.Add(
                new()
                {
                    Id = reader.GetGuid(0),
                    AmountMinorUnit = reader.GetInt64(1),
                    AccountNumber = reader.GetInt32(2),
                    TransactionType = (TransactionType)
                        Enum.Parse(typeof(TransactionType), reader.GetString(3), true),
                    CreatedAt = reader.GetDateTime(4),
                    DeletedAt = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                }
            );
        }
        return transactions;
    }

    public async Task<List<TransactionData>> GetTransactionsBetweenDatesAsync(
        DateTime startDate,
        DateTime endDate,
        long accountNumber
    )
    {
        string sql =
            @"SELECT * FROM transactions
                WHERE created_at >= @StartDate AND created_at <= @EndDate
                AND account_number = @AccountNumber
                AND deleted_at IS NULL
                ORDER BY created_at DESC";

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@StartDate", startDate);
        command.Parameters.AddWithValue("@EndDate", endDate);
        command.Parameters.AddWithValue("@AccountNumber", accountNumber);

        using var reader = await command.ExecuteReaderAsync();

        var transactions = new List<TransactionData>();
        while (await reader.ReadAsync())
        {
            transactions.Add(
                new()
                {
                    Id = reader.GetGuid(0),
                    AmountMinorUnit = reader.GetInt64(1),
                    AccountNumber = reader.GetInt32(2),
                    TransactionType = (TransactionType)
                        Enum.Parse(typeof(TransactionType), reader.GetString(3), true),
                    CreatedAt = reader.GetDateTime(4),
                }
            );
        }
        return transactions;
    }

    public async Task<List<TransactionData>> GetTransactionsForYearAsync(
        int year,
        long accountNumber
    )
    {
        string sql =
            @"SELECT * FROM transactions
                WHERE EXTRACT(YEAR FROM created_at) = @year
                AND account_number = @AccountNumber
                AND deleted_at IS NULL
                ORDER BY created_at DESC";

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@year", year);
        command.Parameters.AddWithValue("@AccountNumber", accountNumber);

        using var reader = await command.ExecuteReaderAsync();

        var transactions = new List<TransactionData>();
        while (await reader.ReadAsync())
        {
            transactions.Add(
                new()
                {
                    Id = reader.GetGuid(0),
                    AmountMinorUnit = reader.GetInt64(1),
                    AccountNumber = reader.GetInt32(2),
                    TransactionType = (TransactionType)
                        Enum.Parse(typeof(TransactionType), reader.GetString(3), true),
                    CreatedAt = reader.GetDateTime(4),
                }
            );
        }
        return transactions;
    }

    // This is only used for 'deletion', by setting the deleted_at to not null.
    // That is why we can remove amount_minor_unit from account balance_minor_unit.
    // Still won't be able to delete a transaction that sets balance to < 0.
    new public async Task UpdateAsync(TransactionData entity)
    {
        string sql =
            @"
            BEGIN;

            UPDATE transactions
            SET account_number = @p0, amount_minor_unit = @p1, created_at = @p2, deleted_at = @p3, transaction_type = @p4::transaction_type
            WHERE id = @p5;

            UPDATE accounts
            SET balance_minor_unit = balance_minor_unit - (@p1 * (CASE WHEN @p4 = 'deposit' THEN 1 ELSE -1 END))
            WHERE account_number = @p0;

            COMMIT;
            ";

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@p0", entity.AccountNumber);
        command.Parameters.AddWithValue("@p1", entity.AmountMinorUnit);
        command.Parameters.AddWithValue("@p2", entity.CreatedAt);
        command.Parameters.AddWithValue("@p3", entity.DeletedAt!);
        command.Parameters.AddWithValue("@p4", entity.TransactionType.ToString().ToLower());
        command.Parameters.AddWithValue("@p5", entity.Id);

        int result = await command.ExecuteNonQueryAsync();
        if (result == 0)
        {
            throw new Exception("Unable to update entry.");
        }
        return;
    }

    public new async Task DeleteAsync(TransactionData entity)
    {
        string sql =
            @"
            BEGIN;

            DELETE FROM transactions WHERE id = @id;

            UPDATE accounts
            SET balance_minor_unit = balance_minor_unit - (@amount * (CASE WHEN @transactionType = 'deposit' THEN 1 ELSE -1 END))
            WHERE account_number = @accountNumber;

            COMMIT;
            ";

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", entity.Id);
        command.Parameters.AddWithValue("@accountNumber", entity.AccountNumber);
        command.Parameters.AddWithValue("@amount", entity.AmountMinorUnit);
        command.Parameters.AddWithValue(
            "@transactionType",
            entity.TransactionType.ToString().ToLower()
        );

        int result = await command.ExecuteNonQueryAsync();
        if (result == 0)
        {
            throw new Exception("Unable to delete entry.");
        }
        return;
    }
}
