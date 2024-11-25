using System;
using Npgsql;

public static class DatabaseService
{
    public static async void EnsureTablesExists(AppDbContext appDbContext)
    {
        string createTablesQuery =
            @"
                CREATE TABLE IF NOT EXISTS users (
                    id SERIAL PRIMARY KEY,
                    username VARCHAR(50) NOT NULL,
                    password_hash TEXT NOT NULL,
                    salt TEXT NOT NULL
                );";
        using (var command = new NpgsqlCommand(createTablesQuery))
        {
            await appDbContext.ExecuteCommandAsync(command);
        }
    }
}


// CREATE TABLE IF NOT EXISTS transactions (
//     id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
// )
