using Microsoft.EntityFrameworkCore;

namespace Library.Weather
{
    internal class WeatherContext : BaseContext
    {
        public WeatherContext(DbContextOptions<WeatherContext> options) : base(options)
        {
            WeatherEmotions = default!;
            WeatherCityUserEmotions = default!;
        }

        public DbSet<WeatherEmotionEntity> WeatherEmotions { get; set; }
        public DbSet<WeatherCityUserEmotionEntity> WeatherCityUserEmotions { get; set; }
    }
}