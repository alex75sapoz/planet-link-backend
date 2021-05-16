using Library.Programming.Contract;
using Library.Programming.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Programming
{
    internal static class ProgrammingMapper
    {
        public static ProgrammingLanguageContract MapToLanguageContract(this ProgrammingLanguageEntity src) => new()
        {
            LanguageId = src.LanguageId,
            Name = src.Name
        };

        public static ProgrammingJobContract MapToJobContract(this ProgrammingJobEntity src) => new()
        {
            JobId = src.JobId,
            Name = src.Name
        };

        public static ProgrammingTechnologyStackContract MapToTechnologyStackContract(this ProgrammingTechnologyStackEntity src) => new()
        {
            TechnologyStackId = src.TechnologyStackId,
            Name = src.Name
        };

        public static ProgrammingProjectTypeContract MapToProjectTypeContract(this ProgrammingProjectTypeEntity src) => new()
        {
            TypeId = src.ProjectTypeId,
            Name = src.Name
        };

        public static ProgrammingProjectContract MapToProjectContract(this ProgrammingProjectEntity src) => new()
        {
            ProjectId = src.ProjectId,
            Name = src.Name,
            Tag = src.Tag,
            Description = src.Description,
            CreatedOn = src.CreatedOn,
            Type = ProgrammingMemoryCache.ProgrammingProjectTypes[src.ProjectTypeId],
            Job = ProgrammingMemoryCache.ProgrammingJobs[src.JobId],
            TechnologyStack = ProgrammingMemoryCache.ProgrammingTechnologyStacks[src.TechnologyStackId],
            Languages = src.ProjectLanguages.Select(projectLanguage => ProgrammingMemoryCache.ProgrammingLanguages[projectLanguage.LanguageId]).ToList()
        };
    }
}
