using Domain.Entities.Account;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess;

public class FinanceCSContext : DbContext
{
    public FinanceCSContext(DbContextOptions<FinanceCSContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; }

    public DbSet<RenewToken> RenewTokens { get; set; }

    public async Task<int> SaveToDbAsync(CancellationToken cancellationToken = default) => await SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}
