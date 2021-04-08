using Microsoft.Extensions.Caching.Memory;

namespace Library.Base
{
    public abstract class BaseService<TConfiguration>
    {
        protected BaseService(TConfiguration configuration, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        protected readonly TConfiguration _configuration;
        protected readonly IMemoryCache _memoryCache;
    }

    public abstract class BaseService<TConfiguration, TRepository>
    {
        protected BaseService(TConfiguration configuration, TRepository repository, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _repository = repository;
            _memoryCache = memoryCache;
        }

        protected readonly TConfiguration _configuration;
        protected readonly TRepository _repository;
        protected readonly IMemoryCache _memoryCache;
    }
}