using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Error
{
    public interface IErrorStartup
    {
        public static void Startup(IServiceCollection services, string databaseConnection) =>
            ErrorStartup.Startup(services, databaseConnection);
    }

    internal static class ErrorStartup
    {
        public static void Startup(IServiceCollection services, string databaseConnection) =>
            services
                //Internal
                .AddDbContext<ErrorContext>(options => options.UseSqlServer(databaseConnection))
                .AddTransient<ErrorRepository>()
                .AddTransient<ErrorService>()
                .AddSingleton(new ErrorConfiguration())
                //Public
                .AddTransient<IErrorRepository, ErrorRepository>()
                .AddTransient<IErrorService, ErrorService>();
    }
}