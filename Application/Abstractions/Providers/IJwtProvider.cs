using Domain.Entities.Account;

namespace Application.Abstractions.Providers;

public interface IJwtProvider
{
    string Create(User user);

    string GetEmailFromToken(string token);

    string GenerateRandom(int size = 32);
}
