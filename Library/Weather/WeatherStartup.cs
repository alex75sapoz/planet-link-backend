using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Weather
{
    public interface IWeatherStartup
    {
        public static bool IsMemoryCacheReady =>
            WeatherStartup.IsMemoryCacheReady;

        public static void Startup(IServiceCollection services, Func<Type, object> configuration, string databaseConnection) =>
            WeatherStartup.Startup(services, (WeatherConfiguration)configuration(typeof(WeatherConfiguration)), databaseConnection);

        public static async Task RefreshMemoryCacheAsync(IServiceProvider serviceProvider) =>
            await WeatherStartup.RefreshMemoryCacheAsync(serviceProvider.GetRequiredService<WeatherRepository>());

        public static object GetStatus() =>
            WeatherStartup.GetStatus();
    }

    internal static class WeatherStartup
    {
        public static bool IsStarted { get; set; }
        public static bool IsMemoryCacheReady { get; set; }

        public static void Startup(IServiceCollection services, WeatherConfiguration configuration, string databaseConnection)
        {
            IsStarted = false;

            services
                //Internal
                .AddDbContext<WeatherContext>(options => options.UseSqlServer(databaseConnection))
                .AddTransient<WeatherRepository>()
                .AddTransient<WeatherService>()
                .AddSingleton(configuration)
                //Public
                .AddTransient<IWeatherRepository, WeatherRepository>()
                .AddTransient<IWeatherService, WeatherService>();

            IsStarted = true;
        }

        public static async Task RefreshMemoryCacheAsync(WeatherRepository repository)
        {
            IsMemoryCacheReady = false;

            var emotionEntities = await repository.GetEmotionsAsync();
            var cityUserEmotionEntities = await repository.GetCityUserEmotionsAsync(DateTimeOffset.Now.AddDays(-1));

            WeatherMemoryCache.WeatherEmotions.Clear();
            foreach (var emotion in emotionEntities.Select(emotionEntity => emotionEntity.MapToEmotionContract()))
                WeatherMemoryCache.WeatherEmotions.TryAdd(emotion.EmotionId, emotion);

            WeatherMemoryCache.WeatherCityUserEmotions.Clear();
            foreach (var cityUserEmotion in cityUserEmotionEntities.Select(cityUserEmotionEntity => cityUserEmotionEntity.MapToCityUserEmotionContract()))
                WeatherMemoryCache.WeatherCityUserEmotions.TryAdd(cityUserEmotion.CityUserEmotionId, cityUserEmotion);

            IsMemoryCacheReady = true;
        }

        public static object GetStatus() => new
        {
            IsStarted,
            IsMemoryCacheReady,
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
                    nameof(IWeatherService)
                }
            },
            MemoryCache = new
            {
                TotalEmotions = WeatherMemoryCache.WeatherEmotions.Count,
                TotalCityUserEmotions = WeatherMemoryCache.WeatherCityUserEmotions.Count
            }
        };
    }
}