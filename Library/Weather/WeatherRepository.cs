using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Weather
{
    public interface IWeatherRepository
    {
        Task<List<WeatherCityUserEmotionEntity>> GetCityUserEmotionsAsync(DateTimeOffset from);
        Task<List<WeatherEmotionEntity>> GetEmotionsAsync();
    }

    class WeatherRepository : BaseRepository<WeatherContext>, IWeatherRepository
    {
        public WeatherRepository(WeatherContext context) : base(context) { }

        public async Task<List<WeatherCityUserEmotionEntity>> GetCityUserEmotionsAsync(DateTimeOffset from) =>
            await _context.WeatherCityUserEmotions
                .Where(quoteUserEmotion => quoteUserEmotion.CreatedOn >= from)
                .ToListAsync();

        public async Task<List<WeatherEmotionEntity>> GetEmotionsAsync() =>
            await _context.WeatherEmotions
                .ToListAsync();
    }
}