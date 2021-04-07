using Microsoft.Extensions.Caching.Memory;

namespace Api.Library
{
    public abstract class LibraryService<TConfiguration>
    {
        protected LibraryService(TConfiguration configuration, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        protected readonly TConfiguration _configuration;
        protected readonly IMemoryCache _memoryCache;
    }

    public abstract class LibraryService<TConfiguration, TRepository>
    {
        protected LibraryService(TConfiguration configuration, TRepository repository, IMemoryCache memoryCache)
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