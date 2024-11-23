using System.Security.Cryptography;

namespace Sefia.Utils;

public class PasswordHasher
{
    private const int HashSize = 32;
    private const int SaltSize = 16;
    private const int Iterations = 310000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

    public static string HashPassword(string password)
    {
        byte[] salt = new byte[SaltSize];
        RandomNumberGenerator.Fill(salt);

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithm))
        {
            byte[] hash = pbkdf2.GetBytes(HashSize);
            return $"{Iterations}:{HashAlgorithm.Name}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
            throw new ArgumentException("Password or storedHash cannot be null or empty.");

        var parts = storedHash.Split(':');
        if (parts.Length != 4)
            throw new FormatException("Invalid hash format.");

        int iterations = int.Parse(parts[0]);
        var algorithm = new HashAlgorithmName(parts[1]);
        byte[] salt = Convert.FromBase64String(parts[2]);
        byte[] storedPasswordHash = Convert.FromBase64String(parts[3]);

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, algorithm))
        {
            byte[] computedHash = pbkdf2.GetBytes(storedPasswordHash.Length);
            return SecureEquals(computedHash, storedPasswordHash);
        }
    }

    private static bool SecureEquals(byte[] a, byte[] b)
    {
        if (a.Length != b.Length) return false;

        int diff = 0;
        for (int i = 0; i < a.Length; i++)
        {
            diff |= a[i] ^ b[i];
        }
        return diff == 0;
    }
}
