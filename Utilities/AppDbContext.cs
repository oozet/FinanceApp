using System.Data;
using System.Security.Cryptography;
using Npgsql;

public class AppDbContext
{
    public User? currentUser { get; private set; }
    private string _connectionString;

    public AppDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task ExecuteCommandAsync(NpgsqlCommand command)
    {
        await using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            command.Connection = connection;

            try
            {
                int rowsAffected = await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    public async Task<NpgsqlDataReader> GetReaderAsync(NpgsqlCommand command)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        command.Connection = connection;

        // CommandBehavior.CloseConnection to close connection when reader is disposed.
        return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
    }

    public async Task<bool> SetUser(string user, string password)
    {
        string SqlQuery =
            "SELECT id, username, password_hash, salt FROM users WHERE username = @user";
        using var command = new NpgsqlCommand(SqlQuery);
        command.Parameters.AddWithValue("@user", user);

        using var reader = await GetReaderAsync(command);
        if (!reader.HasRows)
        {
            return false;
        }

        await reader.ReadAsync();

        string password_hash = reader.GetString(2);
        string salt = reader.GetString(3);
        if (!PasswordService.VerifyPassword(password, password_hash, salt))
        {
            return false;
        }
        currentUser = new() { Id = reader.GetInt32(0), Username = reader.GetString(1) };
        return true;
    }

    public async Task<bool> DoesUserExistAsync(string user)
    {
        string SqlQuery = "SELECT id FROM users WHERE username = @user";
        using (var command = new NpgsqlCommand(SqlQuery))
        {
            command.Parameters.AddWithValue("@user", user);
            using (var reader = await GetReaderAsync(command))
            {
                return reader.HasRows;
            }
        }
    }

    public async Task<bool> CreateUser(string username, string password)
    {
        if (await DoesUserExistAsync(username))
        {
            return false;
        }

        (string password_hash, string salt) = PasswordService.HashPassword(password);
        string SqlQuery =
            "INSERT INTO users (username, password_hash, salt) VALUES (@username, @password_hash, @salt)";
        using var command = new NpgsqlCommand(SqlQuery);
        command.Parameters.AddWithValue("@username", username);
        command.Parameters.AddWithValue("@password_hash", password_hash);
        command.Parameters.AddWithValue("@salt", salt);

        await ExecuteCommandAsync(command);
        await SetUser(username, password);
        return true;
    }

    public void Logout()
    {
        currentUser = null;
    }
}
