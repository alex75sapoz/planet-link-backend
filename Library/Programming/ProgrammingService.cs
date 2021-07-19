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
            ProgrammingMemoryCache.Projects.Where(project =>
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
            Languages = ProgrammingMemoryCache.Languages.Values.ToList(),
            Jobs = ProgrammingMemoryCache.Jobs.Values.ToList(),
            TechnologyStacks = ProgrammingMemoryCache.TechnologyStacks.Values.ToList(),
            ProjectTypes = ProgrammingMemoryCache.ProjectTypes.Values.ToList()
        };

        #endregion
    }
}