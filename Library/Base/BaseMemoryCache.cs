using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Library.Base
{
    public interface ILibraryMemoryCache
    {
        public static IReadOnlyDictionary<string, BaseJob> Jobs => BaseMemoryCache.Jobs;
    }

    static class BaseMemoryCache
    {
        public static readonly ConcurrentDictionary<string, BaseJob> Jobs = new();
    }
}