using Microsoft.EntityFrameworkCore;

namespace Library.Weather
{
    class WeatherContext : BaseContext
    {
        public WeatherContext(DbContextOptions<WeatherContext> options) : base(options) { }

        public DbSet<WeatherEmotionEntity> Emotions { get; set; } = default!;
        public DbSet<WeatherCityUserEmotionEntity> CityUserEmotions { get; set; } = default!;
    }
}