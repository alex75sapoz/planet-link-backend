global using Library.Base;
global using Library.Programming.Contract;
global using Library.Programming.Entity;
global using Library.Programming.Job;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Programming
{
    public interface IProgrammingStartup
    {
        public static void Startup(IServiceCollection services, ProgrammingConfiguration configuration, string databaseConnection) =>
            ProgrammingStartup.Startup(services, configuration, databaseConnection);

        public static object GetStatus() =>
            ProgrammingStartup.GetStatus();
    }

    static class ProgrammingStartup
    {
        public static bool IsReady { get; private set; }

        public static void Startup(IServiceCollection services, ProgrammingConfiguration configuration, string databaseConnection)
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
                .AddTransient<IProgrammingService, ProgrammingService>()
                //Job
                .AddHostedService<ProgrammingProcessMemoryCacheJob>();

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
                    nameof(IProgrammingService),
                    nameof(IProgrammingMemoryCache)
                },
                Job = new[]
                {
                    nameof(ProgrammingProcessMemoryCacheJob)
                }
            },
            MemoryCache = new
            {
                TotalJobs = ProgrammingMemoryCache.ProgrammingJobs.Count,
                TotalLanguages = ProgrammingMemoryCache.ProgrammingLanguages.Count,
                TotalProjects = ProgrammingMemoryCache.ProgrammingProjects.Count,
                TotalProjectTypes = ProgrammingMemoryCache.ProgrammingProjectTypes.Count,
                TotalTechnologyStacks = ProgrammingMemoryCache.ProgrammingTechnologyStacks.Count,
            }
        };
    }
}