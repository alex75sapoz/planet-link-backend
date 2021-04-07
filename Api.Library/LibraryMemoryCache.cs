using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Api.Library
{
    public interface ILibraryMemoryCache
    {
        public static IReadOnlyDictionary<string, LibraryJob> Jobs => LibraryMemoryCache.Jobs;
    }

    internal static class LibraryMemoryCache
    {
        public static readonly ConcurrentDictionary<string, LibraryJob> Jobs = new();
    }
}