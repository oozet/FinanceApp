using System;
using System.Security.Cryptography;
using System.Text;

public static class PasswordService
{
    public static (string hash, string salt) HashPassword(string password)
    {
        // Generate a random salt
        var saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        var salt = Convert.ToBase64String(saltBytes);

        // Hash the password with the salt
        var sha256 = SHA256.Create();
        var saltedPassword = password + salt;
        var saltedPasswordBytes = Encoding.UTF8.GetBytes(saltedPassword);
        var hashBytes = sha256.ComputeHash(saltedPasswordBytes);

        var hash = Convert.ToBase64String(hashBytes);
        return (hash, salt);
    }

    public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
    {
        var sha256 = SHA256.Create();
        var saltedPassword = enteredPassword + storedSalt;
        var saltedPasswordBytes = Encoding.UTF8.GetBytes(saltedPassword);
        var hashBytes = sha256.ComputeHash(saltedPasswordBytes);

        var enteredHash = Convert.ToBase64String(hashBytes);
        return enteredHash == storedHash;
    }
}
