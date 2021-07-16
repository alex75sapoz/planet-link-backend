using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    static class ProgrammingMemoryCache
    {
        public static bool IsReady { get; private set; }

        public static readonly ConcurrentDictionary<int, ProgrammingLanguageContract> ProgrammingLanguages = new();
        public static readonly ConcurrentDictionary<int, ProgrammingJobContract> ProgrammingJobs = new();
        public static readonly ConcurrentDictionary<int, ProgrammingTechnologyStackContract> ProgrammingTechnologyStacks = new();
        public static readonly ConcurrentDictionary<int, ProgrammingProjectTypeContract> ProgrammingProjectTypes = new();
        public static readonly ConcurrentDictionary<int, ProgrammingProjectContract> ProgrammingProjects = new();

        public static async Task LoadAsync(ProgrammingRepository repository)
        {
            if (IsReady) return;

            var languages = (await repository.GetLanguagesAsync()).Select(languageEntity => languageEntity.MapToLanguageContract()).ToList();
            var jobs = (await repository.GetJobsAsync()).Select(jobEntity => jobEntity.MapToJobContract()).ToList();
            var technologyStacks = (await repository.GetTechnologyStacksAsync()).Select(technologyStackEntity => technologyStackEntity.MapToTechnologyStackContract()).ToList();
            var projectTypes = (await repository.GetProjectTypesAsync()).Select(projectTypeEntity => projectTypeEntity.MapToProjectTypeContract()).ToList();
            var projects = (await repository.GetProjectsAsync()).Select(projectEntity => projectEntity.MapToProjectContract()).ToList();

            foreach (var language in languages)
                ProgrammingLanguages[language.LanguageId] = language;

            foreach (var job in jobs)
                ProgrammingJobs[job.JobId] = job;

            foreach (var technologyStack in technologyStacks)
                ProgrammingTechnologyStacks[technologyStack.TechnologyStackId] = technologyStack;

            foreach (var projectType in projectTypes)
                ProgrammingProjectTypes[projectType.ProjectTypeId] = projectType;

            foreach (var project in projects)
                ProgrammingProjects[project.ProjectId] = project;

            IsReady = true;
        }
    }
}