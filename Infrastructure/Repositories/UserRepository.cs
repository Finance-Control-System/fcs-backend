using Application.Abstractions.Repositories;
using Domain.Entities.Account;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(FinanceCSContext dbContext) : IUserRepository
{
    private readonly FinanceCSContext _dbContext = dbContext;

    public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default) =>
        !await _dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
}
