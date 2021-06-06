using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Library.Programming.Contract
{
    public class ProgrammingProjectContract
    {
        public ProgrammingProjectContract()
        {
            LanguageIds = new List<int>();
        }

        [JsonIgnore]
        public int TypeId { get; internal set; }
        [JsonIgnore]
        public int JobId { get; internal set; }
        [JsonIgnore]
        public int TechnologyStackId { get; internal set; }
        [JsonIgnore]
        public List<int> LanguageIds { get; internal set; }

        public int ProjectId { get; internal set; }
        public string Name { get; internal set; }
        public string Tag { get; internal set; }
        public string Description { get; internal set; }
        public bool IsImportant { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }

        public ProgrammingProjectTypeContract Type => IProgrammingMemoryCache.ProgrammingProjectTypes[TypeId];
        public ProgrammingJobContract Job => IProgrammingMemoryCache.ProgrammingJobs[JobId];
        public ProgrammingTechnologyStackContract TechnologyStack => IProgrammingMemoryCache.ProgrammingTechnologyStacks[TechnologyStackId];
        public List<ProgrammingLanguageContract> Languages => LanguageIds.Select(languageId => IProgrammingMemoryCache.ProgrammingLanguages[languageId]).ToList();
    }
}