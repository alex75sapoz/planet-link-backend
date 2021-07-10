using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Programming
{
    public interface IProgrammingMemoryCache
    {
        public static bool IsReady => ProgrammingMemoryCache.IsReady;

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

        public static async Task RefreshAsync(ProgrammingRepository repository)
        {
            var languages = (await repository.GetLanguagesAsync()).Select(languageEntity => languageEntity.MapToLanguageContract()).ToDictionary(language => language.LanguageId);
            var jobs = (await repository.GetJobsAsync()).Select(jobEntity => jobEntity.MapToJobContract()).ToDictionary(job => job.JobId);
            var technologyStacks = (await repository.GetTechnologyStacksAsync()).Select(technologyStackEntity => technologyStackEntity.MapToTechnologyStackContract()).ToDictionary(technologyStack => technologyStack.TechnologyStackId);
            var projectTypes = (await repository.GetProjectTypesAsync()).Select(projectTypeEntity => projectTypeEntity.MapToProjectTypeContract()).ToDictionary(projectType => projectType.ProjectTypeId);
            var projects = (await repository.GetProjectsAsync()).Select(projectEntity => projectEntity.MapToProjectContract()).ToDictionary(project => project.ProjectId);

            //Languages
            foreach (var language in languages)
                ProgrammingLanguages[language.Key] = language.Value;

            foreach (var language in ProgrammingLanguages.Where(language => !languages.ContainsKey(language.Key)).ToList())
                ProgrammingLanguages.TryRemove(language);

            //Jobs
            foreach (var job in jobs)
                ProgrammingJobs[job.Key] = job.Value;

            foreach (var job in ProgrammingJobs.Where(job => !jobs.ContainsKey(job.Key)).ToList())
                ProgrammingJobs.TryRemove(job);

            //TechnologyStacks
            foreach (var technologyStack in technologyStacks)
                ProgrammingTechnologyStacks[technologyStack.Key] = technologyStack.Value;

            foreach (var technologyStack in ProgrammingTechnologyStacks.Where(technologyStack => !technologyStacks.ContainsKey(technologyStack.Key)).ToList())
                ProgrammingTechnologyStacks.TryRemove(technologyStack);

            //ProjectTypes
            foreach (var projectType in projectTypes)
                ProgrammingProjectTypes[projectType.Key] = projectType.Value;

            foreach (var projectType in ProgrammingProjectTypes.Where(projectType => !projectTypes.ContainsKey(projectType.Key)).ToList())
                ProgrammingProjectTypes.TryRemove(projectType);

            //Projects
            foreach (var project in projects)
                ProgrammingProjects[project.Key] = project.Value;

            foreach (var project in ProgrammingProjects.Where(project => !projects.ContainsKey(project.Key)).ToList())
                ProgrammingProjects.TryRemove(project);

            IsReady = true;
        }
    }
}