using FinanceApp.Data;
using FinanceApp.Models;
using FinanceApp.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FinanceApp.Repositories;

public interface IUserRepositorySQL : IRepository<AppUser>
{
    Task<Guid> GetIdByNameAsync(string name);
    Task<AppUser?> GetUserByNameAndPasswordAsync(string name, string password);
    Task<AppUser?> CreateUserAsync(string name, string password);
    Task<AppUser?> GetByIdAsync(string id);
}

public class UserRepositorySQL : IUserRepositorySQL
{
    private readonly AppDbContext _context;
    private ILogger<UserRepositorySQL> _logger;

    public UserRepositorySQL(AppDbContext context, ILogger<UserRepositorySQL> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Guid> GetIdByNameAsync(string name)
    {
        string sql = "SELECT id FROM users WHERE username = @name";

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@name", name);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return reader.GetGuid(0);
        }

        return Guid.Empty;
    }

    public async Task<AppUser?> GetUserByNameAndPasswordAsync(string name, string password)
    {
        string sql = "SELECT id, username, password_hash, salt FROM users WHERE username = @name";

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@name", name);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            string password_hash = reader.GetString(2);
            string salt = reader.GetString(3);
            if (!PasswordService.VerifyPassword(password, password_hash, salt))
            {
                return null;
            }

            return new AppUser { Id = reader.GetGuid(0), Username = reader.GetString(1) };
        }

        return null;
    }

    public async Task<AppUser?> CreateUserAsync(string username, string password)
    {
        if (await GetIdByNameAsync(username) != Guid.Empty)
        {
            throw new Exception("User exists in database.");
        }

        var newUser = new AppUser { Id = Guid.NewGuid(), Username = username };

        (string password_hash, string salt) = PasswordService.HashPassword(password);
        string sql =
            @"INSERT INTO users (id, username, password_hash, salt) VALUES (@id, @username, @password_hash, @salt)";

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", newUser.Id);
        command.Parameters.AddWithValue("@username", newUser.Username);
        command.Parameters.AddWithValue("@password_hash", password_hash);
        command.Parameters.AddWithValue("@salt", salt);

        await command.ExecuteNonQueryAsync();
        return newUser;
    }

    Task<IEnumerable<AppUser>> IRepository<AppUser>.GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(AppUser entity)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(AppUser entity)
    {
        try
        {
            string sql = "DELETE FROM users WHERE id = @userId";

            await using var connection = new NpgsqlConnection(
                _context.Database.GetConnectionString()
            );
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@userId", entity.Id);

            await command.ExecuteNonQueryAsync();
        }
        catch (NpgsqlException ex)
        {
            // Log the exception
            _logger.LogError(ex, "Error deleting user in database");
        }
        catch (Exception ex)
        {
            // Catch any unexpected exceptions
            _logger.LogError(ex, "Unexpected error during user deletion");
        }
    }

    public async Task<AppUser?> GetByIdAsync(Guid id)
    {
        string sql = "SELECT id, username FROM users WHERE id = @id";

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        try
        {
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new AppUser { Id = reader.GetGuid(0), Username = reader.GetString(1) };
            }
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

    public async Task<AppUser?> GetByIdAsync(string id)
    {
        if (!Guid.TryParse(id, out Guid userId))
        {
            throw new InvalidCastException(id + " is not a valid Guid.");
        }

        try
        {
            return await GetByIdAsync(userId);
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
}
