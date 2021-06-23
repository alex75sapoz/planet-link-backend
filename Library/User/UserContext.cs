using Microsoft.EntityFrameworkCore;

namespace Library.User
{
    internal class UserContext : BaseContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
            UserTypes = default!;
            Users = default!;
            UsersGoogle = default!;
            UsersStocktwits = default!;
            UserSessions = default!;
        }

        public DbSet<UserTypeEntity> UserTypes { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<UserGoogleEntity> UsersGoogle { get; set; }
        public DbSet<UserStocktwitsEntity> UsersStocktwits { get; set; }
        public DbSet<UserSessionEntity> UserSessions { get; set; }
    }
}