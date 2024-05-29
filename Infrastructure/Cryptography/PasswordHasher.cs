using Application.Abstractions.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Cryptography;

public class PasswordHasher : IPasswordHasher, IPasswordHashChecker, IDisposable
{
    private const KeyDerivationPrf Prf = KeyDerivationPrf.HMACSHA256;
    private const int IterationCount = 10000;
    private const int NumberOfBytesRequested = 256 / 8;
    private const int SaltSize = 128 / 8;
    private readonly RandomNumberGenerator _rng;

    public PasswordHasher() => _rng = new RNGCryptoServiceProvider();

    public string HashPassword(string password, string salt)
    {
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(salt);

        return Convert.ToBase64String(HashPasswordInternal(password, Encoding.UTF8.GetBytes(salt)));
    }

    public string GenerateSalt()
    {
        byte[] randomBytes = new byte[SaltSize];
        _rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public bool HashesMath(string passwordHash, string providedPassword, string salt)
    {
        ArgumentNullException.ThrowIfNull(passwordHash);
        ArgumentNullException.ThrowIfNull(providedPassword);

        byte[] decodedHashedPassword = Convert.FromBase64String(passwordHash);

        return decodedHashedPassword.Length != 0 &&
            VerifyPasswordHashInternal(decodedHashedPassword, providedPassword, Encoding.UTF8.GetBytes(salt));
    }

    private byte[] HashPasswordInternal(string password, byte[] salt)
    {
        byte[] subKey = KeyDerivation.Pbkdf2(password, salt, Prf, IterationCount, NumberOfBytesRequested);

        byte[] outputBytes = new byte[salt.Length + subKey.Length];

        Buffer.BlockCopy(salt, 0, outputBytes, 0, salt.Length);

        Buffer.BlockCopy(subKey, 0, outputBytes, salt.Length, subKey.Length);

        return outputBytes;
    }

    private static bool VerifyPasswordHashInternal(byte[] hashedPassword, string password, byte[] salt)
    {
        try
        {
            Buffer.BlockCopy(hashedPassword, 0, salt, 0, salt.Length);

            int subKeyLength = hashedPassword.Length - salt.Length;

            if (subKeyLength < SaltSize)
            {
                return false;
            }

            byte[] expectedSubKey = new byte[subKeyLength];

            Buffer.BlockCopy(hashedPassword, salt.Length, expectedSubKey, 0, expectedSubKey.Length);

            byte[] actualSubKey = KeyDerivation.Pbkdf2(password, salt, Prf, IterationCount, subKeyLength);

            return ByteArraysEqual(actualSubKey, expectedSubKey);
        }
        catch
        {
            return false;
        }
    }

    private static bool ByteArraysEqual(byte[] a, byte[] b)
    {
        if (a == null && b == null)
        {
            return true;
        }

        if (a == null || b == null || a.Length != b.Length)
        {
            return false;
        }

        bool areSame = true;

        for (int i = 0; i < a.Length; i++)
        {
            areSame &= a[i] == b[i];
        }

        return areSame;
    }

    public void Dispose() => _rng.Dispose();
}