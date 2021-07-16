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
    static class LibraryExtension
    {
        public static void AddApiLibrary(this IServiceCollection services, IConfiguration configuration, DatabaseConfiguration databaseConfiguration)
        {
            configuration = configuration.GetSection($"{nameof(Api)}.{nameof(Library)}");

            IErrorStartup.ConfigureServices(services, new ErrorConfiguration(), databaseConfiguration.Connection);
            IUserStartup.ConfigureServices(services, configuration.GetSection(nameof(UserConfiguration)).Get<UserConfiguration>(), databaseConfiguration.Connection);
            ILocationStartup.ConfigureServices(services, configuration.GetSection(nameof(LocationConfiguration)).Get<LocationConfiguration>(), databaseConfiguration.Connection);
            IWeatherStartup.ConfigureServices(services, configuration.GetSection(nameof(WeatherConfiguration)).Get<WeatherConfiguration>(), databaseConfiguration.Connection);
            IStockMarketStartup.ConfigureServices(services, configuration.GetSection(nameof(StockMarketConfiguration)).Get<StockMarketConfiguration>(), databaseConfiguration.Connection);
            IProgrammingStartup.ConfigureServices(services, new ProgrammingConfiguration(), databaseConfiguration.Connection);
        }
    }
}