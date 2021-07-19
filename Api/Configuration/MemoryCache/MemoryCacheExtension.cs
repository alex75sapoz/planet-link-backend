using Microsoft.Extensions.DependencyInjection;

namespace Api.Configuration.MemoryCache
{
    static class MemoryCacheExtension
    {
        public static void AddApiMemoryCache(this IServiceCollection services) =>
            services
                .AddMemoryCache()
                .AddHostedService<MemoryCacheLoader>();
    }
}