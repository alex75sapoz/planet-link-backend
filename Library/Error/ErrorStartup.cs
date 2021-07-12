global using Library.Base;
global using Library.Error.Contract;
global using Library.Error.Entity;
global using Library.Error.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Error
{
    public interface IErrorStartup
    {
        public static void Startup(IServiceCollection services, ErrorConfiguration configuration, string databaseConnection) =>
            ErrorStartup.Startup(services, configuration, databaseConnection);

        public static object GetStatus() =>
            ErrorStartup.GetStatus();
    }

    static class ErrorStartup
    {
        public static bool IsReady { get; private set; }

        public static void Startup(IServiceCollection services, ErrorConfiguration configuration, string databaseConnection)
        {
            if (IsReady) return;

            services
                //Internal
                .AddDbContext<ErrorContext>(options => options.UseSqlServer(databaseConnection))
                .AddTransient<ErrorRepository>()
                .AddTransient<ErrorService>()
                .AddSingleton(configuration)
                //Public
                .AddTransient<IErrorRepository, ErrorRepository>()
                .AddTransient<IErrorService, ErrorService>();

            IsReady = true;
        }

        public static object GetStatus() => new
        {
            IsReady,
            RegisteredTypes = new
            {
                Internal = new[]
                {
                    nameof(ErrorContext),
                    nameof(ErrorRepository),
                    nameof(ErrorService),
                    nameof(ErrorConfiguration)
                },
                Public = new[]
                {
                    nameof(IErrorRepository),
                    nameof(IErrorService)
                }
            }
        };
    }
}