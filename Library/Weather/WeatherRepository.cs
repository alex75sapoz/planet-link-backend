using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Weather
{
    class WeatherRepository : BaseRepository<WeatherContext>, IWeatherRepository
    {
        public WeatherRepository(WeatherContext context) : base(context) { }

        public async Task<List<WeatherCityUserEmotionEntity>> GetCityUserEmotionsAsync(DateTimeOffset from) =>
            await _context.CityUserEmotions
                .Where(quoteUserEmotion => quoteUserEmotion.CreatedOn >= from)
                .ToListAsync();

        public async Task<WeatherCityUserEmotionEntity?> GetCityUserEmotionAsync(int cityUserEmotionId) =>
            await _context.CityUserEmotions
                .FindAsync(cityUserEmotionId);

        public async Task<List<WeatherEmotionEntity>> GetEmotionsAsync() =>
            await _context.Emotions
                .ToListAsync();

        public async Task<WeatherEmotionEntity?> GetEmotionAsync(int emotionId) =>
            await _context.Emotions
                .FindAsync(emotionId);
    }

    public interface IWeatherRepository
    {
        Task<List<WeatherCityUserEmotionEntity>> GetCityUserEmotionsAsync(DateTimeOffset from);
        Task<List<WeatherEmotionEntity>> GetEmotionsAsync();
        Task<WeatherEmotionEntity?> GetEmotionAsync(int emotionId);
        Task<WeatherCityUserEmotionEntity?> GetCityUserEmotionAsync(int cityUserEmotionId);
    }
}