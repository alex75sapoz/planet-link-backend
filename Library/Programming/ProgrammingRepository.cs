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
            await _context.ProgrammingProjectTypes
                .ToListAsync();

        public async Task<List<ProgrammingJobEntity>> GetJobsAsync() =>
            await _context.ProgrammingJobs
                .ToListAsync();

        public async Task<List<ProgrammingLanguageEntity>> GetLanguagesAsync() =>
            await _context.ProgrammingLanguages
                .ToListAsync();

        public async Task<List<ProgrammingTechnologyStackEntity>> GetTechnologyStacksAsync() =>
            await _context.ProgrammingTechnologyStacks
                .ToListAsync();

        public async Task<List<ProgrammingProjectEntity>> GetProjectsAsync() =>
            await _context.ProgrammingProjects
                .Include(project => project.Type)
                .Include(project => project.Job)
                .Include(project => project.TechnologyStack)
                .Include(project => project.ProjectLanguages)
                .ToListAsync();
    }
}