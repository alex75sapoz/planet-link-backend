using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Programming
{
    public interface IProgrammingRepository
    {
        Task<List<ProgrammingJobEntity>> GetJobsAsync();
        Task<List<ProgrammingLanguageEntity>> GetLanguagesAsync();
        Task<List<ProgrammingProjectEntity>> GetProjectsAsync();
        Task<List<ProgrammingProjectTypeEntity>> GetProjectTypesAsync();
        Task<List<ProgrammingTechnologyStackEntity>> GetTechnologyStacksAsync();
    }

    class ProgrammingRepository : BaseRepository<ProgrammingContext>, IProgrammingRepository
    {
        public ProgrammingRepository(ProgrammingContext context) : base(context) { }

        public async Task<List<ProgrammingProjectTypeEntity>> GetProjectTypesAsync() =>
            await _context.ProjectTypes
                .ToListAsync();

        public async Task<List<ProgrammingJobEntity>> GetJobsAsync() =>
            await _context.Jobs
                .ToListAsync();

        public async Task<List<ProgrammingLanguageEntity>> GetLanguagesAsync() =>
            await _context.Languages
                .ToListAsync();

        public async Task<List<ProgrammingTechnologyStackEntity>> GetTechnologyStacksAsync() =>
            await _context.TechnologyStacks
                .ToListAsync();

        public async Task<List<ProgrammingProjectEntity>> GetProjectsAsync() =>
            await _context.Projects
                .Include(project => project.ProjectType)
                .Include(project => project.Job)
                .Include(project => project.TechnologyStack)
                .Include(project => project.ProjectLanguages)
                .ToListAsync();
    }
}