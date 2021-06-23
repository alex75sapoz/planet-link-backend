using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Library.Programming
{
    public interface IProgrammingMemoryCache
    {
        public static IReadOnlyDictionary<int, ProgrammingLanguageContract> ProgrammingLanguages => ProgrammingMemoryCache.ProgrammingLanguages;
        public static IReadOnlyDictionary<int, ProgrammingJobContract> ProgrammingJobs => ProgrammingMemoryCache.ProgrammingJobs;
        public static IReadOnlyDictionary<int, ProgrammingTechnologyStackContract> ProgrammingTechnologyStacks => ProgrammingMemoryCache.ProgrammingTechnologyStacks;
        public static IReadOnlyDictionary<int, ProgrammingProjectTypeContract> ProgrammingProjectTypes => ProgrammingMemoryCache.ProgrammingProjectTypes;
        public static IReadOnlyDictionary<int, ProgrammingProjectContract> ProgrammingProjects => ProgrammingMemoryCache.ProgrammingProjects;
    }

    internal static class ProgrammingMemoryCache
    {
        public static readonly ConcurrentDictionary<int, ProgrammingLanguageContract> ProgrammingLanguages = new();
        public static readonly ConcurrentDictionary<int, ProgrammingJobContract> ProgrammingJobs = new();
        public static readonly ConcurrentDictionary<int, ProgrammingTechnologyStackContract> ProgrammingTechnologyStacks = new();
        public static readonly ConcurrentDictionary<int, ProgrammingProjectTypeContract> ProgrammingProjectTypes = new();
        public static readonly ConcurrentDictionary<int, ProgrammingProjectContract> ProgrammingProjects = new();
    }
}