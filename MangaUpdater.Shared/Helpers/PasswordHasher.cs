using System.Security.Cryptography;

namespace MangaUpdater.Shared.Helpers;

public static class PasswordHasher
{
    public static string Hash(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[16];
        rng.GetBytes(salt);
       
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        var result = new byte[1 + salt.Length + hash.Length];
        result[0] = 0x01; // version
        
        Buffer.BlockCopy(salt, 0, result, 1, salt.Length);
        Buffer.BlockCopy(hash, 0, result, 1 + salt.Length, hash.Length);
        
        return Convert.ToBase64String(result);
    }

    public static bool Verify(string password, string hashed)
    {
        try
        {
            var bytes = Convert.FromBase64String(hashed);
            if (bytes.Length < 1 + 16 + 32) return false;
            
            var version = bytes[0];
            if (version != 0x01) return false;
            
            var salt = new byte[16];
            Buffer.BlockCopy(bytes, 1, salt, 0, salt.Length);
            
            var hash = new byte[32];
            Buffer.BlockCopy(bytes, 1 + salt.Length, hash, 0, hash.Length);
            
            var computed = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
            return CryptographicOperations.FixedTimeEquals(computed, hash);
        }
        catch
        {
            return false;
        }
    }
}
