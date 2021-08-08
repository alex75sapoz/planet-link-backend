using Microsoft.Extensions.Caching.Memory;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Programming
{
    public interface IProgrammingService
    {
        ProgrammingConfigurationContract GetConfiguration();
        List<ProgrammingProjectContract> SearchProjects(int? projectTypeId, int? languageId, int? jobId, int? technologyStackId);
        Task<ProgrammingProjectContract> CreateProjectAsync(ProgrammingProjectCreateContract newProject, DateTimeZone timezone);

        #region Memory Cache

        internal static IReadOnlyDictionary<int, ProgrammingLanguageContract> Languages => ProgrammingMemoryCache.Languages;
        internal static IReadOnlyDictionary<int, ProgrammingJobContract> Jobs => ProgrammingMemoryCache.Jobs;
        internal static IReadOnlyDictionary<int, ProgrammingTechnologyStackContract> TechnologyStacks => ProgrammingMemoryCache.TechnologyStacks;
        internal static IReadOnlyDictionary<int, ProgrammingProjectTypeContract> ProjectTypes => ProgrammingMemoryCache.ProjectTypes;
        internal static IReadOnlyDictionary<int, ProgrammingProjectContract> Projects => ProgrammingMemoryCache.Projects;

        public static ProgrammingLanguageContract GetLanguage(int languageId) =>
            Languages.TryGetValue(languageId, out ProgrammingLanguageContract? language)
                ? language
                : throw new BadRequestException($"{nameof(languageId)} is invalid");

        public static ProgrammingJobContract GetJob(int jobId) =>
            Jobs.TryGetValue(jobId, out ProgrammingJobContract? job)
                ? job
                : throw new BadRequestException($"{nameof(jobId)} is invalid");

        public static ProgrammingTechnologyStackContract GetTechnologyStack(int technologyStackId) =>
            TechnologyStacks.TryGetValue(technologyStackId, out ProgrammingTechnologyStackContract? technologyStack)
                ? technologyStack
                : throw new BadRequestException($"{nameof(technologyStackId)} is invalid");

        public static ProgrammingProjectTypeContract GetProjectType(int projectTypeId) =>
            ProjectTypes.TryGetValue(projectTypeId, out ProgrammingProjectTypeContract? projectType)
                ? projectType
                : throw new BadRequestException($"{nameof(projectTypeId)} is invalid");

        public static ProgrammingProjectContract GetProject(int projectId) =>
           Projects.TryGetValue(projectId, out ProgrammingProjectContract? project)
               ? project
               : throw new BadRequestException($"{nameof(projectId)} is invalid");

        #endregion
    }

    class ProgrammingService : BaseService<ProgrammingConfiguration, ProgrammingRepository>, IProgrammingService
    {
        public ProgrammingService(ProgrammingConfiguration configuration, ProgrammingRepository repository, IMemoryCache memoryCache) : base(configuration, repository, memoryCache) { }

        #region Search

        public List<ProgrammingProjectContract> SearchProjects(int? projectTypeId, int? languageId, int? jobId, int? technologyStackId) =>
            IProgrammingService.Projects.Where(project =>
                (!projectTypeId.HasValue || project.Value.ProjectTypeId == projectTypeId) &&
                (!languageId.HasValue || project.Value.LanguageIds.Contains(languageId.Value)) &&
                (!jobId.HasValue || project.Value.JobId == jobId) &&
                (!technologyStackId.HasValue || project.Value.TechnologyStackId == technologyStackId)
            )
            .Select(project => project.Value)
            .OrderByDescending(project => project.CreatedOn)
            .ToList();

        #endregion

        #region Get

        public ProgrammingConfigurationContract GetConfiguration() => new()
        {
            Languages = IProgrammingService.Languages.Values.ToList(),
            Jobs = IProgrammingService.Jobs.Values.ToList(),
            TechnologyStacks = IProgrammingService.TechnologyStacks.Values.ToList(),
            ProjectTypes = IProgrammingService.ProjectTypes.Values.ToList()
        };

        #endregion

        #region Create

        public async Task<ProgrammingProjectContract> CreateProjectAsync(ProgrammingProjectCreateContract newProject, DateTimeZone timezone)
        {
            var projectType = IProgrammingService.GetProjectType(newProject.ProjectTypeId);
            var job = IProgrammingService.GetJob(newProject.JobId);
            var technologyStack = IProgrammingService.GetTechnologyStack(newProject.TechnologyStackId);
            var languages = newProject.LanguageIds.Select(languageId => IProgrammingService.GetLanguage(languageId)).ToList();

            var project = (await _repository.AddAndSaveChangesAsync(new ProgrammingProjectEntity
            {
                ProjectTypeId = projectType.ProjectTypeId,
                JobId = job.JobId,
                TechnologyStackId = technologyStack.TechnologyStackId,
                ProjectLanguages = languages.Select(language => new ProgrammingProjectLanguageEntity
                {
                    LanguageId = language.LanguageId
                }).ToList(),
                Name = newProject.Name,
                Tag = newProject.Tag,
                Description = newProject.Description,
                IsImportant = newProject.IsImportant,
                CreatedOn = DateTimeOffset.Now.AtTimezone(timezone)
            })).MapToProjectContract();

            ProgrammingMemoryCache.Projects.TryAdd(project.ProjectId, project);

            return project;
        }

        #endregion
    }
}