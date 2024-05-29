namespace Application.Abstractions.Repositories;

public interface IRenewTokenRepository
{
    Task AddOrUpdateAsync(Guid userId, string token, CancellationToken cancellationToken = default);

    Task<int> FindRenewTokenIdByUserIdAndTokenAsync(Guid userId, string renewToken, CancellationToken cancellationToken = default);

    Task UpdateRenewTokenAsync(int renewTokenId, string newToken, CancellationToken cancellationToken = default);
}
