using System.Collections.Generic;

namespace Library.Programming.Contract
{
    public class ProgrammingConfigurationContract
    {
        public List<ProgrammingLanguageContract> Languages { get; internal set; } = default!;
        public List<ProgrammingJobContract> Jobs { get; internal set; } = default!;
        public List<ProgrammingTechnologyStackContract> TechnologyStacks { get; internal set; } = default!;
        public List<ProgrammingProjectTypeContract> ProjectTypes { get; internal set; } = default!;
    }
}