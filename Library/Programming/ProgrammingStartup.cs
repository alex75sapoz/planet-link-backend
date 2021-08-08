global using Library.Base;
global using Library.Programming.Contract;
global using Library.Programming.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Library.Programming
{
    public interface IProgrammingStartup
    {
        public static void ConfigureServices(IServiceCollection services, ProgrammingConfiguration configuration, string databaseConnection) =>
            ProgrammingStartup.ConfigureServices(services, configuration, databaseConnection);

        public static async Task LoadMemoryCacheAsync(IServiceProvider serviceProvider) =>
            await ProgrammingMemoryCache.LoadAsync(serviceProvider.GetRequiredService<ProgrammingRepository>());

        public static object GetStatus() =>
            ProgrammingStartup.GetStatus();
    }

    static class ProgrammingStartup
    {
        public static bool IsReady { get; private set; }

        public static void ConfigureServices(IServiceCollection services, ProgrammingConfiguration configuration, string databaseConnection)
        {
            if (IsReady) return;

            services
                //Internal
                .AddDbContext<ProgrammingContext>(options => options.UseSqlServer(databaseConnection))
                .AddTransient<ProgrammingRepository>()
                .AddTransient<ProgrammingService>()
                .AddSingleton(configuration)
                //Public
                .AddTransient<IProgrammingRepository, ProgrammingRepository>()
                .AddTransient<IProgrammingService, ProgrammingService>();

            IsReady = true;
        }

        public static object GetStatus() => new
        {
            IsReady,
            IsMemoryCacheReady = ProgrammingMemoryCache.IsReady,
            RegisteredTypes = new
            {
                Internal = new[]
                {
                    nameof(ProgrammingContext),
                    nameof(ProgrammingRepository),
                    nameof(ProgrammingService),
                    nameof(ProgrammingMemoryCache),
                    nameof(ProgrammingConfiguration)
                },
                Public = new[]
                {
                    nameof(IProgrammingRepository),
                    nameof(IProgrammingService)
                }
            },
            MemoryCache = new
            {
                TotalJobs = IProgrammingService.Jobs.Count,
                TotalLanguages = IProgrammingService.Languages.Count,
                TotalProjects = IProgrammingService.Projects.Count,
                TotalProjectTypes = IProgrammingService.ProjectTypes.Count,
                TotalTechnologyStacks = IProgrammingService.TechnologyStacks.Count,
            }
        };
    }
}