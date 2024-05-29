namespace Application.Abstractions.Cryptography;

public interface IPasswordHasher
{
    string HashPassword(string password, string salt);

    string GenerateSalt();
}
