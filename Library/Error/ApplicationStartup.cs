global using Library.Application.Contract;
global using Library.Application.Entity;
global using Library.Application.Enum;
global using Library.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Application
{
    public interface IApplicationStartup
    {
        public static void ConfigureServices(IServiceCollection services, ApplicationConfiguration configuration, string databaseConnection) =>
            ApplicationStartup.ConfigureServices(services, configuration, databaseConnection);

        public static object GetStatus() =>
            ApplicationStartup.GetStatus();
    }

    static class ApplicationStartup
    {
        public static bool IsReady { get; private set; }

        public static void ConfigureServices(IServiceCollection services, ApplicationConfiguration configuration, string databaseConnection)
        {
            if (IsReady) return;

            services
                //Internal
                .AddDbContext<ApplicationContext>(options => options.UseSqlServer(databaseConnection))
                .AddTransient<ApplicationRepository>()
                .AddTransient<ApplicationService>()
                .AddSingleton(configuration)
                //Public
                .AddTransient<IApplicationRepository, ApplicationRepository>()
                .AddTransient<IApplicationService, ApplicationService>();

            IsReady = true;
        }

        public static object GetStatus() => new
        {
            IsReady,
            RegisteredTypes = new
            {
                Internal = new[]
                {
                    nameof(ApplicationContext),
                    nameof(ApplicationRepository),
                    nameof(ApplicationService),
                    nameof(ApplicationConfiguration)
                },
                Public = new[]
                {
                    nameof(IApplicationRepository),
                    nameof(IApplicationService)
                }
            }
        };
    }
}