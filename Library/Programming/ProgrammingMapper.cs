﻿using System.Linq;

namespace Library.Programming
{
    static class ProgrammingMapper
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
            ProjectTypeId = src.ProjectTypeId,
            Name = src.Name
        };

        public static ProgrammingProjectContract MapToProjectContract(this ProgrammingProjectEntity src) => new()
        {
            ProjectTypeId = src.ProjectTypeId,
            JobId = src.JobId,
            TechnologyStackId = src.TechnologyStackId,
            LanguageIds = src.ProjectLanguages.Select(projectLanguage => projectLanguage.LanguageId).ToList(),
            ProjectId = src.ProjectId,
            Name = src.Name,
            Tag = src.Tag,
            Description = src.Description,
            IsImportant = src.IsImportant,
            CreatedOn = src.CreatedOn
        };
    }
}