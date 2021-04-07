using Api.Configuration.Database;
using Api.Library.Error;
using Api.Library.User;
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
        }
    }
}