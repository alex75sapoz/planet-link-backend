using Microsoft.EntityFrameworkCore;

namespace Library.Account
{
    class AccountContext : BaseContext
    {
        public AccountContext(DbContextOptions<AccountContext> options) : base(options) { }
        public DbSet<AccountUserTypeEntity> UserTypes { get; set; } = default!;
        public DbSet<AccountUserEntity> Users { get; set; } = default!;
        public DbSet<AccountUserGoogleEntity> UsersGoogle { get; set; } = default!;
        public DbSet<AccountUserStocktwitsEntity> UsersStocktwits { get; set; } = default!;
        public DbSet<AccountUserSessionEntity> UserSessions { get; set; } = default!;
    }
}