using FinanceApp.Data;
using FinanceApp.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FinanceApp.Repositories;

public interface ITransactionRepository : IRepository<TransactionData> { }

public class TransactionRepository : ITransactionRepository
{
    private AppDbContext _context;
    private ILogger _logger;

    public TransactionRepository(AppDbContext context, ILogger<TransactionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> AddAsync(TransactionData entity)
    {
        try
        {
            string sql =
                @"INSERT INTO transactions (id, amount_minor_unit, account_number, transaction_type, created_at) VALUES (@id, @amount_minor_unit, account_number, @type::transaction_type, created_at)";
            try
            {
                await using var connection = new NpgsqlConnection(
                    _context.Database.GetConnectionString()
                );
                await connection.OpenAsync();

                await using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", entity.Id);
                command.Parameters.AddWithValue("@amount_minor_unit", entity.AmountMinorUnit);
                command.Parameters.AddWithValue("@account_number", entity.AccountNumber);
                command.Parameters.AddWithValue(
                    "@type",
                    entity.TransactionType.ToString().ToLower()
                );
                command.Parameters.AddWithValue("@created_at", entity.CreatedAt);

                return await command.ExecuteNonQueryAsync();
            }
            catch (NpgsqlException ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error creating transaction in database");
                return 0;
            }
            catch (Exception ex)
            {
                // Catch any unexpected exceptions
                _logger.LogError(ex, "Unexpected error during transaction creation");
                return 0;
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions in the method (e.g., from GetIdByNameAsync)
            _logger.LogError(ex, "Error in transaction creation process");
            return null;
        }
        return null;
    }

    public Task DeleteAsync(TransactionData entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TransactionData>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<TransactionData?> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(TransactionData entity)
    {
        throw new NotImplementedException();
    }
}
