dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Username=postgres;Password=password;Database=finance" Npgsql.EntityFrameworkCore.PostgreSQL -o Models


You can access shadow properties in your code using the Entry method of the DbContext:
var user = _context.Users.FirstOrDefault(u => u.Username == username);

if (user != null)
{
    var passwordHash = _context.Entry(user).Property("PasswordHash").CurrentValue.ToString();
    var salt = _context.Entry(user).Property("Salt").CurrentValue.ToString();

    if (_passwordService.VerifyPassword(passwordHash, password))
    {
        // User is authenticated
    }
}
