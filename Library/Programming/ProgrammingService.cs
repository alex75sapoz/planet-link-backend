using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;

namespace Library.Programming
{
    public interface IProgrammingService
    {
        ProgrammingConfigurationContract GetConfiguration();
        List<ProgrammingProjectContract> SearchProjects(int? projectTypeId, int? languageId, int? jobId, int? technologyStackId);
    }

    class ProgrammingService : BaseService<ProgrammingConfiguration, ProgrammingRepository>, IProgrammingService
    {
        public ProgrammingService(ProgrammingConfiguration configuration, ProgrammingRepository repository, IMemoryCache memoryCache) : base(configuration, repository, memoryCache) { }

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
    }
}