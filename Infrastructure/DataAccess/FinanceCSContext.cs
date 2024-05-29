using Domain.Entities.Account;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess;

public class FinanceCSContext(DbContextOptions<FinanceCSContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<RenewToken> RenewTokens { get; set; }

    public async Task<int> SaveToDbAsync(CancellationToken cancellationToken = default) => await SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}
