using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Programming
{
    public interface IProgrammingStartup
    {
        public static bool IsMemoryCacheReady =>
            ProgrammingStartup.IsMemoryCacheReady;

        public static void Startup(IServiceCollection services, string databaseConnection) =>
            ProgrammingStartup.Startup(services, new ProgrammingConfiguration(), databaseConnection);

        public static async Task RefreshMemoryCacheAsync(IServiceProvider serviceProvider) =>
            await ProgrammingStartup.RefreshMemoryCacheAsync(serviceProvider.GetRequiredService<ProgrammingRepository>());
    }

    internal static class ProgrammingStartup
    {
        public static bool IsMemoryCacheReady { get; set; }

        public static void Startup(IServiceCollection services, ProgrammingConfiguration configuration, string databaseConnection) =>
            services
                //Internal
                .AddDbContext<ProgrammingContext>(options => options.UseSqlServer(databaseConnection))
                .AddTransient<ProgrammingRepository>()
                .AddTransient<ProgrammingService>()
                .AddSingleton(configuration)
                //Public
                .AddTransient<IProgrammingRepository, ProgrammingRepository>()
                .AddTransient<IProgrammingService, ProgrammingService>();

        public static async Task RefreshMemoryCacheAsync(ProgrammingRepository repository)
        {
            IsMemoryCacheReady = false;

            var languageEntities = await repository.GetLanguagesAsync();
            var jobEntities = await repository.GetJobsAsync();
            var technologyStackEntities = await repository.GetTechnologyStacksAsync();
            var projectTypeEntities = await repository.GetProjectTypesAsync();
            var projectEntities = await repository.GetProjectsAsync();

            ProgrammingMemoryCache.ProgrammingLanguages.Clear();
            foreach (var language in languageEntities.Select(languageEntity => languageEntity.MapToLanguageContract()))
                ProgrammingMemoryCache.ProgrammingLanguages.TryAdd(language.LanguageId, language);

            ProgrammingMemoryCache.ProgrammingJobs.Clear();
            foreach (var job in jobEntities.Select(jobEntity => jobEntity.MapToJobContract()))
                ProgrammingMemoryCache.ProgrammingJobs.TryAdd(job.JobId, job);

            ProgrammingMemoryCache.ProgrammingTechnologyStacks.Clear();
            foreach (var technologyStack in technologyStackEntities.Select(technologyStackEntity => technologyStackEntity.MapToTechnologyStackContract()))
                ProgrammingMemoryCache.ProgrammingTechnologyStacks.TryAdd(technologyStack.TechnologyStackId, technologyStack);

            ProgrammingMemoryCache.ProgrammingProjectTypes.Clear();
            foreach (var projectType in projectTypeEntities.Select(projectTypeEntity => projectTypeEntity.MapToProjectTypeContract()))
                ProgrammingMemoryCache.ProgrammingProjectTypes.TryAdd(projectType.TypeId, projectType);

            ProgrammingMemoryCache.ProgrammingProjects.Clear();
            foreach (var project in projectEntities.Select(projectEntity => projectEntity.MapToProjectContract()))
                ProgrammingMemoryCache.ProgrammingProjects.TryAdd(project.ProjectId, project);

            IsMemoryCacheReady = true;
        }
    }
}