using Microsoft.EntityFrameworkCore;

namespace Wallet_lib
{
    public class WalletContext : DbContext
    {
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<AutomatedTransaction> AutomatedTransactions { get; set; }
        public WalletContext(DbContextOptions<WalletContext> options) : base(options)
        {
        }


    }
}
