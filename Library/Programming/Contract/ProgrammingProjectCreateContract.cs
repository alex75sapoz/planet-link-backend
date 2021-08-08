using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Library.Programming.Contract
{
    public class ProgrammingProjectCreateContract
    {
        public ProgrammingProjectCreateContract(int projectTypeId, int jobId, int technologyStackId, List<int> languageIds, string name, string tag, string description, bool isImportant)
        {
            ProjectTypeId = projectTypeId;
            JobId = jobId;
            TechnologyStackId = technologyStackId;
            LanguageIds = languageIds.Any()
                ? languageIds
                : null!;
            Name = name;
            Tag = tag;
            Description = description;
            IsImportant = isImportant;
        }

        [Range(1, int.MaxValue)]
        public int ProjectTypeId { get; set; }
        [Range(1, int.MaxValue)]
        public int JobId { get; set; }
        [Range(1, int.MaxValue)]
        public int TechnologyStackId { get; set; }
        [Required]
        public List<int> LanguageIds { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(20)]
        public string Tag { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [Required]
        public bool IsImportant { get; set; }
    }
}