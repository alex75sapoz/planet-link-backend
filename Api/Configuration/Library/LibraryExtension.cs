using Api.Configuration.Database;
using Api.Library.Error;
using Api.Library.Location;
using Api.Library.User;
using Api.Library.Weather;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Configuration.Library
{
    internal static class LibraryExtension
    {
        public static void AddApiLibrary(this IServiceCollection services, IConfiguration configuration, DatabaseConfiguration databaseConfiguration)
        {
            configuration = configuration.GetSection($"{nameof(Api)}.{nameof(Library)}");

            IErrorStartup.Startup(services, databaseConfiguration.Connection);
            IUserStartup.Startup(services, (type) => configuration.GetSection(type.Name).Get(type), databaseConfiguration.Connection);
            ILocationStartup.Startup(services, (type) => configuration.GetSection(type.Name).Get(type), databaseConfiguration.Connection);
            IWeatherStartup.Startup(services, (type) => configuration.GetSection(type.Name).Get(type), databaseConfiguration.Connection);
        }
    }
}