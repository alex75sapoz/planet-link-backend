using Api.Configuration.Database;
using Library.Error;
using Library.Location;
using Library.Programming;
using Library.StockMarket;
using Library.User;
using Library.Weather;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Configuration.Library
{
    internal static class LibraryExtension
    {
        public static void AddApiLibrary(this IServiceCollection services, IConfiguration configuration, DatabaseConfiguration databaseConfiguration)
        {
            configuration = configuration.GetSection($"{nameof(Api)}.{nameof(Library)}");

            //Order must match project build order

            IErrorStartup.Startup(services, new ErrorConfiguration(), databaseConfiguration.Connection);
            IUserStartup.Startup(services, configuration.GetSection(nameof(UserConfiguration)).Get<UserConfiguration>(), databaseConfiguration.Connection);
            ILocationStartup.Startup(services, configuration.GetSection(nameof(LocationConfiguration)).Get<LocationConfiguration>(), databaseConfiguration.Connection);
            IWeatherStartup.Startup(services, configuration.GetSection(nameof(WeatherConfiguration)).Get<WeatherConfiguration>(), databaseConfiguration.Connection);
            IStockMarketStartup.Startup(services, configuration.GetSection(nameof(StockMarketConfiguration)).Get<StockMarketConfiguration>(), databaseConfiguration.Connection);
            IProgrammingStartup.Startup(services, new ProgrammingConfiguration(), databaseConfiguration.Connection);
        }
    }
}