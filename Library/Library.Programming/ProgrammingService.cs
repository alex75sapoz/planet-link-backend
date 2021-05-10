using Library.Base;
using Library.Programming.Contract;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;

namespace Library.Programming
{
    public interface IProgrammingService
    {
        ProgrammingConfigurationContract GetConfiguration();
        List<ProgrammingProjectContract> SearchProjects(List<int> projectTypeIds, List<int> languageIds, List<int> jobIds, List<int> technologyStackIds);
    }

    internal class ProgrammingService : BaseService<ProgrammingConfiguration, ProgrammingRepository>, IProgrammingService
    {
        public ProgrammingService(ProgrammingConfiguration configuration, ProgrammingRepository repository, IMemoryCache memoryCache) : base(configuration, repository, memoryCache) { }

        #region Search

        public List<ProgrammingProjectContract> SearchProjects(List<int> projectTypeIds, List<int> languageIds, List<int> jobIds, List<int> technologyStackIds)
        {
            if (projectTypeIds is not null && !projectTypeIds.Any())
                projectTypeIds = null;

            if (languageIds is not null && !languageIds.Any())
                languageIds = null;

            if (jobIds is not null && !jobIds.Any())
                jobIds = null;

            if (technologyStackIds is not null && !technologyStackIds.Any())
                technologyStackIds = null;

            return ProgrammingMemoryCache.ProgrammingProjects.Where(project =>
                (projectTypeIds is null || projectTypeIds.Contains(project.Value.Type.TypeId)) &&
                (languageIds is null || project.Value.Languages.Any(language => languageIds.Contains(language.LanguageId))) &&
                (jobIds is null || jobIds.Contains(project.Value.Job.JobId)) &&
                (technologyStackIds is null || technologyStackIds.Contains(project.Value.TechnologyStack.TechnologyStackId))
            )
            .Select(project => project.Value)
            .ToList();
        }

        #endregion

        #region Get

        public ProgrammingConfigurationContract GetConfiguration() => new()
        {
            Languages = ProgrammingMemoryCache.ProgrammingLanguages.Values.ToList(),
            Jobs = ProgrammingMemoryCache.ProgrammingJobs.Values.ToList(),
            TechnologyStacks = ProgrammingMemoryCache.ProgrammingTechnologyStacks.Values.ToList(),
            ProjectTypes = ProgrammingMemoryCache.ProgrammingProjectTypes.Values.ToList()
        };

        #endregion
    }
}