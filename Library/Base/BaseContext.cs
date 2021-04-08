using Microsoft.EntityFrameworkCore;

namespace Library.Base
{
    public abstract class BaseContext : DbContext
    {
        protected BaseContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}