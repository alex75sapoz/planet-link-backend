using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Library.Programming.Contract
{
    public class ProgrammingProjectContract
    {
        [JsonIgnore]
        public int ProjectTypeId { get; internal set; }
        [JsonIgnore]
        public int JobId { get; internal set; }
        [JsonIgnore]
        public int TechnologyStackId { get; internal set; }
        [JsonIgnore]
        public List<int> LanguageIds { get; internal set; } = new List<int>();

        public int ProjectId { get; internal set; }
        public string Name { get; internal set; } = default!;
        public string Tag { get; internal set; } = default!;
        public string Description { get; internal set; } = default!;
        public bool IsImportant { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }

        public ProgrammingProjectTypeContract ProjectType => IProgrammingMemoryCache.ProjectTypes[ProjectTypeId];
        public ProgrammingJobContract Job => IProgrammingMemoryCache.Jobs[JobId];
        public ProgrammingTechnologyStackContract TechnologyStack => IProgrammingMemoryCache.TechnologyStacks[TechnologyStackId];
        public List<ProgrammingLanguageContract> Languages => LanguageIds.Select(languageId => IProgrammingMemoryCache.Languages[languageId]).ToList();
    }
}