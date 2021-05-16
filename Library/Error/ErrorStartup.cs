using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Error
{
    public interface IErrorStartup
    {
        public static void Startup(IServiceCollection services, string databaseConnection) =>
            ErrorStartup.Startup(services, new ErrorConfiguration(), databaseConnection);
    }

    internal static class ErrorStartup
    {
        public static void Startup(IServiceCollection services, ErrorConfiguration configuration, string databaseConnection) =>
            services
                //Internal
                .AddDbContext<ErrorContext>(options => options.UseSqlServer(databaseConnection))
                .AddTransient<ErrorRepository>()
                .AddTransient<ErrorService>()
                .AddSingleton(configuration)
                //Public
                .AddTransient<IErrorRepository, ErrorRepository>()
                .AddTransient<IErrorService, ErrorService>();
    }
}