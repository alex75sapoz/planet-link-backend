using Microsoft.EntityFrameworkCore;

namespace Library.Weather
{
    class WeatherContext : BaseContext
    {
        public WeatherContext(DbContextOptions<WeatherContext> options) : base(options) { }

        public DbSet<WeatherEmotionEntity> WeatherEmotions { get; set; } = default!;
        public DbSet<WeatherCityUserEmotionEntity> WeatherCityUserEmotions { get; set; } = default!;
    }
}