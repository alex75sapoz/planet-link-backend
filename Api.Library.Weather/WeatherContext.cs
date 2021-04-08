using Api.Library.Weather.Entity;
using Microsoft.EntityFrameworkCore;

namespace Api.Library.Weather
{
    internal class WeatherContext : LibraryContext
    {
        public WeatherContext(DbContextOptions<WeatherContext> options) : base(options) { }

        public DbSet<WeatherEmotionEntity> WeatherEmotions { get; set; }
        public DbSet<WeatherCityUserEmotionEntity> WeatherCityUserEmotions { get; set; }
    }
}