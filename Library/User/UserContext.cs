using Microsoft.EntityFrameworkCore;

namespace Library.User
{
    class UserContext : BaseContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }
        public DbSet<UserTypeEntity> UserTypes { get; set; } = default!;
        public DbSet<UserEntity> Users { get; set; } = default!;
        public DbSet<UserGoogleEntity> UsersGoogle { get; set; } = default!;
        public DbSet<UserStocktwitsEntity> UsersStocktwits { get; set; } = default!;
        public DbSet<UserSessionEntity> UserSessions { get; set; } = default!;
    }
}