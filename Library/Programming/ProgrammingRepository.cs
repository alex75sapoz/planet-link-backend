using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Programming
{
    class ProgrammingRepository : BaseRepository<ProgrammingContext>, IProgrammingRepository
    {
        public ProgrammingRepository(ProgrammingContext context) : base(context) { }

        public async Task<List<ProgrammingProjectTypeEntity>> GetProjectTypesAsync() =>
            await _context.ProjectTypes
                .ToListAsync();

        public async Task<ProgrammingProjectTypeEntity> GetProjectTypeAsync(int projectTypeId) =>
            await _context.ProjectTypes
                .FindAsync(projectTypeId);

        public async Task<List<ProgrammingJobEntity>> GetJobsAsync() =>
            await _context.Jobs
                .ToListAsync();

        public async Task<ProgrammingJobEntity> GetJobAsync(int jobId) =>
            await _context.Jobs
                .FindAsync(jobId);

        public async Task<List<ProgrammingLanguageEntity>> GetLanguagesAsync() =>
            await _context.Languages
                .ToListAsync();

        public async Task<ProgrammingLanguageEntity> GetLanguageAsync(int languageId) =>
            await _context.Languages
                .FindAsync(languageId);

        public async Task<List<ProgrammingTechnologyStackEntity>> GetTechnologyStacksAsync() =>
            await _context.TechnologyStacks
                .ToListAsync();

        public async Task<ProgrammingTechnologyStackEntity> GetTechnologyStackAsync(int technologyStackId) =>
            await _context.TechnologyStacks
                .FindAsync(technologyStackId);

        public async Task<List<ProgrammingProjectEntity>> GetProjectsAsync() =>
            await _context.Projects
                .Include(project => project.ProjectType)
                .Include(project => project.Job)
                .Include(project => project.TechnologyStack)
                .Include(project => project.ProjectLanguages)
                .ToListAsync();

        public async Task<ProgrammingProjectEntity> GetProjectAsync(int projectId) =>
            await _context.Projects
                .Include(project => project.ProjectType)
                .Include(project => project.Job)
                .Include(project => project.TechnologyStack)
                .Include(project => project.ProjectLanguages)
                .SingleOrDefaultAsync(project => project.ProjectId == projectId);
    }

    public interface IProgrammingRepository
    {
        Task<List<ProgrammingJobEntity>> GetJobsAsync();
        Task<ProgrammingJobEntity> GetJobAsync(int jobId);
        Task<List<ProgrammingLanguageEntity>> GetLanguagesAsync();
        Task<ProgrammingLanguageEntity> GetLanguageAsync(int languageId);
        Task<List<ProgrammingProjectEntity>> GetProjectsAsync();
        Task<ProgrammingProjectEntity> GetProjectAsync(int projectId);
        Task<List<ProgrammingProjectTypeEntity>> GetProjectTypesAsync();
        Task<ProgrammingProjectTypeEntity> GetProjectTypeAsync(int projectTypeId);
        Task<List<ProgrammingTechnologyStackEntity>> GetTechnologyStacksAsync();
        Task<ProgrammingTechnologyStackEntity> GetTechnologyStackAsync(int technologyStackId);
    }
}