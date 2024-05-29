namespace Application.Abstractions.Cryptography;

public interface IPasswordHashChecker
{
    bool HashesMath(string passwordHash, string providedPassword, string salt);
}
