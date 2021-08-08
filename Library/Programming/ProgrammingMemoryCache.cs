using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Programming
{
    static class ProgrammingMemoryCache
    {
        public static bool IsReady { get; private set; }

        public static readonly ConcurrentDictionary<int, ProgrammingLanguageContract> Languages = new();
        public static readonly ConcurrentDictionary<int, ProgrammingJobContract> Jobs = new();
        public static readonly ConcurrentDictionary<int, ProgrammingTechnologyStackContract> TechnologyStacks = new();
        public static readonly ConcurrentDictionary<int, ProgrammingProjectTypeContract> ProjectTypes = new();
        public static readonly ConcurrentDictionary<int, ProgrammingProjectContract> Projects = new();

        public static async Task LoadAsync(ProgrammingRepository repository)
        {
            if (IsReady) return;

            var languages = (await repository.GetLanguagesAsync()).Select(languageEntity => languageEntity.MapToLanguageContract()).ToList();
            var jobs = (await repository.GetJobsAsync()).Select(jobEntity => jobEntity.MapToJobContract()).ToList();
            var technologyStacks = (await repository.GetTechnologyStacksAsync()).Select(technologyStackEntity => technologyStackEntity.MapToTechnologyStackContract()).ToList();
            var projectTypes = (await repository.GetProjectTypesAsync()).Select(projectTypeEntity => projectTypeEntity.MapToProjectTypeContract()).ToList();
            var projects = (await repository.GetProjectsAsync()).Select(projectEntity => projectEntity.MapToProjectContract()).ToList();

            foreach (var language in languages)
                Languages[language.LanguageId] = language;

            foreach (var job in jobs)
                Jobs[job.JobId] = job;

            foreach (var technologyStack in technologyStacks)
                TechnologyStacks[technologyStack.TechnologyStackId] = technologyStack;

            foreach (var projectType in projectTypes)
                ProjectTypes[projectType.ProjectTypeId] = projectType;

            foreach (var project in projects)
                Projects[project.ProjectId] = project;

            IsReady = true;
        }
    }
}