using System;
using System.Collections.Generic;

namespace Library.Programming.Contract
{
    public class ProgrammingProjectContract
    {
        public int ProjectId { get; internal set; }
        public string Name { get; internal set; }
        public string Tag { get; internal set; }
        public string Description { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }

        public ProgrammingProjectTypeContract Type { get; internal set; }
        public ProgrammingJobContract Job { get; internal set; }
        public ProgrammingTechnologyStackContract TechnologyStack { get; internal set; }
        public List<ProgrammingLanguageContract> Languages { get; internal set; }
    }
}