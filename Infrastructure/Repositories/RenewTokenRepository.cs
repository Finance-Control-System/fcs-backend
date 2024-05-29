using Application.Abstractions.Repositories;
using Domain.Entities.Account;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RenewTokenRepository(FinanceCSContext dbContext) : IRenewTokenRepository
{
    private readonly FinanceCSContext _dbContext = dbContext;

    public async Task AddOrUpdateAsync(Guid userId, string token, CancellationToken cancellationToken = default)
    {
        var found = await _dbContext.RenewTokens.FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);
        if (found == null)
        {
            _dbContext.RenewTokens.Add(new RenewToken { UserId = userId, Token = token });
        }
        else
        {
            found.Token = token;
        }

        await _dbContext.SaveToDbAsync(cancellationToken);
    }

    public async Task<int> FindRenewTokenIdByUserIdAndTokenAsync(Guid userId, string renewToken, CancellationToken cancellationToken = default)
    {
        var found = await _dbContext.RenewTokens.FirstOrDefaultAsync(t => t.UserId == userId && t.Token == renewToken, cancellationToken);
        return found == null ? 0 : found.Id;
    }

    public async Task UpdateRenewTokenAsync(int renewTokenId, string newToken, CancellationToken cancellationToken = default)
    {
        var found = _dbContext.RenewTokens.FirstOrDefault(t => t.Id == renewTokenId);
        if (found != null)
        {
            found.Token = newToken;
            await _dbContext.SaveToDbAsync(cancellationToken);
        }
    }
}
