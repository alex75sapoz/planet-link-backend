using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Base
{
    public abstract class BaseRepository<TContext>
        where TContext : BaseContext
    {
        protected BaseRepository(TContext context) =>
            _context = context;

        protected readonly TContext _context;

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();

        public async Task<TEntity> AddAndSaveChangesAsync<TEntity>(TEntity entity)
            where TEntity : class, new()
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ICollection<TEntity>> AddRangeAndSaveChangesAsync<TEntity>(ICollection<TEntity> entities)
            where TEntity : class, new()
        {
            await _context.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public async Task<TEntity> AddAsync<TEntity>(TEntity entity)
            where TEntity : class, new()
        {
            await _context.AddAsync(entity);
            return entity;
        }

        public async Task RemoveAsync<TEntity>(TEntity entity)
            where TEntity : class, new()
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}