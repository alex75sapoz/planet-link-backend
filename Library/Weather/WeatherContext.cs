using Library.Base;
using Library.Weather.Entity;
using Microsoft.EntityFrameworkCore;

namespace Library.Weather
{
    internal class WeatherContext : BaseContext
    {
        public WeatherContext(DbContextOptions<WeatherContext> options) : base(options) { }

        public DbSet<WeatherEmotionEntity> WeatherEmotions { get; set; }
        public DbSet<WeatherCityUserEmotionEntity> WeatherCityUserEmotions { get; set; }
    }
}