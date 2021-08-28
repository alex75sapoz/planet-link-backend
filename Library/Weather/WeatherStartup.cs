global using Library.Base;
global using Library.Weather.Contract;
global using Library.Weather.Entity;
global using Library.Weather.Enum;
global using Library.Weather.Job;
global using Library.Weather.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Weather
{
    public interface IWeatherStartup
    {
        public static void ConfigureServices(IServiceCollection services, WeatherConfiguration configuration, string databaseConnection) =>
            WeatherStartup.ConfigureServices(services, configuration, databaseConnection);

        public static object GetStatus() =>
            WeatherStartup.GetStatus();
    }

    static class WeatherStartup
    {
        public static bool IsReady { get; private set; }

        public static void ConfigureServices(IServiceCollection services, WeatherConfiguration configuration, string databaseConnection)
        {
            if (IsReady) return;

            services
                //Internal
                .AddDbContext<WeatherContext>(options => options.UseSqlServer(databaseConnection))
                .AddTransient<WeatherRepository>()
                .AddTransient<WeatherService>()
                .AddSingleton(configuration)
                //Public
                .AddTransient<IWeatherRepository, WeatherRepository>()
                .AddTransient<IWeatherService, WeatherService>()
                //Job
                .AddHostedService<WeatherProcessMemoryCacheJob>();

            IsReady = true;
        }

        public static object GetStatus() => new
        {
            IsReady,
            RegisteredTypes = new
            {
                Internal = new[]
                {
                    nameof(WeatherContext),
                    nameof(WeatherRepository),
                    nameof(WeatherService),
                    nameof(WeatherConfiguration)
                },
                Public = new[]
                {
                    nameof(IWeatherRepository),
                    nameof(IWeatherMemoryCache),
                    nameof(IWeatherService)
                },
                Job = new[]
                {
                    nameof(WeatherProcessMemoryCacheJob)
                }
            },
            MemoryCache = new
            {
                TotalEmotions = IWeatherMemoryCache.Emotions.Count,
                TotalCityUserEmotions = IWeatherMemoryCache.CityUserEmotions.Count
            }
        };
    }
}