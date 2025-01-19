using FinanceApp.Data;
using FinanceApp.Models;
using FinanceApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FinanceApp.Repositories;

public interface IPersonRepositorySQL : IRepository<Person>
{
    Task<Person> AddPersonAsync(Person person);
    Task<Person?> GetPersonAsync(AppUser user);
    Task UpdatePersonAsync(Person person);
    Task<List<CombinedUserData>> GetAllCombinedUserDataAsync();
    Task<CombinedUserData> GetCombinedUserDataAsync(AppUser user);
}

public class PersonRepositorySQL : IPersonRepositorySQL
{
    private readonly AppDbContext _context;
    private ILogger<PersonRepositorySQL> _logger;

    public PersonRepositorySQL(AppDbContext context, ILogger<PersonRepositorySQL> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Person> AddPersonAsync(Person person)
    {
        string sql =
            @"INSERT INTO persons (user_id, first_name, last_name, email) VALUES (@userId, @firstName, @lastName, @email) RETURNING person_id";

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@userId", person.UserId);
        command.Parameters.AddWithValue("@firstName", person.FirstName);
        command.Parameters.AddWithValue("@lastName", person.LastName);
        command.Parameters.AddWithValue("@email", person.Email);

        // Need a reader to get the id from the database.
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            person.PersonId = reader.GetInt32(0);
        }

        return person;
    }

    public async Task<Person?> GetPersonAsync(AppUser user)
    {
        string sql = "SELECT * FROM persons WHERE user_id = @userId";

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@userId", user.Id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Person
            {
                PersonId = reader.GetInt32(0),
                UserId = reader.GetGuid(1),
                FirstName = reader.GetString(2),
                LastName = reader.GetString(3),
                Email = reader.GetString(4),
            };
        }

        return null;
    }

    public async Task UpdatePersonAsync(Person entity)
    {
        string sql =
            @"UPDATE persons
                SET first_name = @firstName, last_name = @lastName, email = @email
                WHERE person_id = @personId";
        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@firstName", entity.FirstName);
        command.Parameters.AddWithValue("@lastName", entity.LastName);
        command.Parameters.AddWithValue("@email", entity.Email);
        command.Parameters.AddWithValue("@personId", entity.PersonId);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<CombinedUserData> GetCombinedUserDataAsync(AppUser user)
    {
        string sql =
            @"SELECT users.username, persons.first_name, persons.last_name, persons.email, accounts.account_number
                        FROM users
                        LEFT JOIN persons ON users.id = persons.user_id
                        LEFT JOIN accounts ON users.id = accounts.user_id
                        WHERE users.id = @userId";

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@userId", user.Id);

        using var reader = await command.ExecuteReaderAsync();

        var combinedUserData = new CombinedUserData();
        while (await reader.ReadAsync())
        {
            var username = reader.GetString(0);
            var firstName = reader.IsDBNull(1) ? null : reader.GetString(1);
            var lastName = reader.IsDBNull(2) ? null : reader.GetString(2);
            var email = reader.IsDBNull(3) ? null : reader.GetString(3);
            var accountNumber = reader.IsDBNull(4) ? -1 : reader.GetInt32(4);

            if (string.IsNullOrEmpty(combinedUserData.UserName))
            {
                combinedUserData.UserName = username;
                combinedUserData.FirstName = firstName;
                combinedUserData.LastName = lastName;
                combinedUserData.Email = email;
                combinedUserData.AccountNumbers = new List<int>();
            }
            if (accountNumber != -1)
            {
                combinedUserData.AccountNumbers.Add(accountNumber);
            }
        }

        return combinedUserData;
    }

    public async Task<List<CombinedUserData>> GetAllCombinedUserDataAsync()
    {
        List<CombinedUserData> combinedUserData = [];
        string sql =
            @"SELECT users.username, persons.first_name, persons.last_name, persons.email, accounts.account_number
                        FROM users
                        LEFT JOIN persons ON users.id = persons.user_id
                        LEFT JOIN accounts ON users.id = accounts.user_id";

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());

        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            bool newData = false;
            var username = reader.GetString(0);
            var firstName = reader.IsDBNull(1) ? null : reader.GetString(1);
            var lastName = reader.IsDBNull(2) ? null : reader.GetString(2);
            var email = reader.IsDBNull(3) ? null : reader.GetString(3);
            var accountNumber = reader.IsDBNull(4) ? -1 : reader.GetInt32(4);

            var combinedData = combinedUserData.FirstOrDefault(cd => cd.UserName == username);
            if (combinedData == null)
            {
                newData = true;
                combinedData = new CombinedUserData
                {
                    UserName = username,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    AccountNumbers = new List<int>(),
                };
            }
            if (accountNumber != -1)
            {
                combinedData.AccountNumbers.Add(accountNumber);
            }
            if (newData)
            {
                combinedUserData.Add(combinedData);
            }
        }

        return combinedUserData;
    }

    public Task DeleteAsync(Person entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Person>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Person?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Person entity)
    {
        throw new NotImplementedException();
    }
}
