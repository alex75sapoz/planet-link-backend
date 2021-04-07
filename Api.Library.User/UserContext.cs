using Api.Library.User.Entity;
using Microsoft.EntityFrameworkCore;

namespace Api.Library.User
{
    internal class UserContext : LibraryContext
    {
        public UserContext(DbContextOptions options) : base(options) { }

        public DbSet<UserTypeEntity> UserTypes { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<UserGoogleEntity> UsersGoogle { get; set; }
        public DbSet<UserStocktwitsEntity> UsersStocktwits { get; set; }
        public DbSet<UserSessionEntity> UserSessions { get; set; }
    }
}