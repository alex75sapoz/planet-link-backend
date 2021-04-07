using Microsoft.EntityFrameworkCore;

namespace Api.Library
{
    public abstract class LibraryContext : DbContext
    {
        protected LibraryContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}