using Microsoft.Extensions.Caching.Memory;
using NodaTime;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Programming
{
    class ProgrammingService : BaseService<ProgrammingConfiguration, ProgrammingRepository>, IProgrammingService
    {
        public ProgrammingService(ProgrammingConfiguration configuration, ProgrammingRepository repository, IMemoryCache memoryCache) : base(configuration, repository, memoryCache) { }

        #region MemoryCache

        internal static ConcurrentDictionary<int, ProgrammingLanguageContract> _languages = new();
        internal static ConcurrentDictionary<int, ProgrammingJobContract> _jobs = new();
        internal static ConcurrentDictionary<int, ProgrammingTechnologyStackContract> _technologyStacks = new();
        internal static ConcurrentDictionary<int, ProgrammingProjectTypeContract> _projectTypes = new();
        internal static ConcurrentDictionary<int, ProgrammingProjectContract> _projects = new();

        public async Task MemoryCacheRefreshAsync(MemoryCacheDictionary? dictionary = null, int? id = null)
        {
            if (!dictionary.HasValue || dictionary.Value == MemoryCacheDictionary.Languages)
            {
                if (!id.HasValue)
                    _languages = new((await _repository.GetLanguagesAsync()).Select(languageEntity => languageEntity.MapToLanguageContract()).ToDictionary(language => language.LanguageId));
                else
                    _languages[id.Value] = (await _repository.GetLanguageAsync(id.Value) ?? throw new BadRequestException($"{nameof(id)} is invalid")).MapToLanguageContract();
            }

            if (!dictionary.HasValue || dictionary.Value == MemoryCacheDictionary.Jobs)
            {
                if (!id.HasValue)
                    _jobs = new((await _repository.GetJobsAsync()).Select(jobEntity => jobEntity.MapToJobContract()).ToDictionary(job => job.JobId));
                else
                    _jobs[id.Value] = (await _repository.GetJobAsync(id.Value) ?? throw new BadRequestException($"{nameof(id)} is invalid")).MapToJobContract();
            }

            if (!dictionary.HasValue || dictionary.Value == MemoryCacheDictionary.TechnologyStacks)
            {
                if (!id.HasValue)
                    _technologyStacks = new((await _repository.GetTechnologyStacksAsync()).Select(technologyStackEntity => technologyStackEntity.MapToTechnologyStackContract()).ToDictionary(technologyStack => technologyStack.TechnologyStackId));
                else
                    _technologyStacks[id.Value] = (await _repository.GetTechnologyStackAsync(id.Value) ?? throw new BadRequestException($"{nameof(id)} is invalid")).MapToTechnologyStackContract();
            }

            if (!dictionary.HasValue || dictionary.Value == MemoryCacheDictionary.ProjectTypes)
            {
                if (!id.HasValue)
                    _projectTypes = new((await _repository.GetProjectTypesAsync()).Select(projectTypeEntity => projectTypeEntity.MapToProjectTypeContract()).ToDictionary(projectType => projectType.ProjectTypeId));
                else
                    _projectTypes[id.Value] = (await _repository.GetProjectTypeAsync(id.Value) ?? throw new BadRequestException($"{nameof(id)} is invalid")).MapToProjectTypeContract();
            }

            if (!dictionary.HasValue || dictionary.Value == MemoryCacheDictionary.Projects)
            {
                if (!id.HasValue)
                    _projects = new((await _repository.GetProjectsAsync()).Select(projectEntity => projectEntity.MapToProjectContract()).ToDictionary(project => project.ProjectId));
                else
                    _projects[id.Value] = (await _repository.GetProjectAsync(id.Value) ?? throw new BadRequestException($"{nameof(id)} is invalid")).MapToProjectContract();
            }
        }

        #endregion

        #region Search

        public List<ProgrammingProjectContract> SearchProjects(int? projectTypeId, int? languageId, int? jobId, int? technologyStackId) =>
            IProgrammingMemoryCache.Projects.Where(project =>
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
            Languages = IProgrammingMemoryCache.Languages.Values.ToList(),
            Jobs = IProgrammingMemoryCache.Jobs.Values.ToList(),
            TechnologyStacks = IProgrammingMemoryCache.TechnologyStacks.Values.ToList(),
            ProjectTypes = IProgrammingMemoryCache.ProjectTypes.Values.ToList()
        };

        #endregion

        #region Create

        public async Task<ProgrammingProjectContract> CreateProjectAsync(ProgrammingProjectCreateContract newProject, DateTimeZone timezone)
        {
            var projectType = IProgrammingMemoryCache.GetProjectType(newProject.ProjectTypeId);
            var job = IProgrammingMemoryCache.GetJob(newProject.JobId);
            var technologyStack = IProgrammingMemoryCache.GetTechnologyStack(newProject.TechnologyStackId);
            var languages = newProject.LanguageIds.Select(languageId => IProgrammingMemoryCache.GetLanguage(languageId)).ToList();

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

            _projects.TryAdd(project.ProjectId, project);

            return project;
        }

        #endregion
    }

    public interface IProgrammingMemoryCache
    {
        Task MemoryCacheRefreshAsync(MemoryCacheDictionary? dictionary = null, int? id = null);

        public static IReadOnlyDictionary<int, ProgrammingLanguageContract> Languages => ProgrammingService._languages;
        public static IReadOnlyDictionary<int, ProgrammingJobContract> Jobs => ProgrammingService._jobs;
        public static IReadOnlyDictionary<int, ProgrammingTechnologyStackContract> TechnologyStacks => ProgrammingService._technologyStacks;
        public static IReadOnlyDictionary<int, ProgrammingProjectTypeContract> ProjectTypes => ProgrammingService._projectTypes;
        public static IReadOnlyDictionary<int, ProgrammingProjectContract> Projects => ProgrammingService._projects;

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
    }

    public interface IProgrammingService : IProgrammingMemoryCache
    {
        ProgrammingConfigurationContract GetConfiguration();
        List<ProgrammingProjectContract> SearchProjects(int? projectTypeId, int? languageId, int? jobId, int? technologyStackId);
        Task<ProgrammingProjectContract> CreateProjectAsync(ProgrammingProjectCreateContract newProject, DateTimeZone timezone);
    }
}