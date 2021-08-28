using Library.Account;
using Library.Location;
using Microsoft.Extensions.Caching.Memory;
using NodaTime;
using RestSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Weather
{
    class WeatherService : BaseService<WeatherConfiguration, WeatherRepository>, IWeatherService
    {
        public WeatherService(WeatherConfiguration configuration, WeatherRepository repository, IMemoryCache cache) : base(configuration, repository, cache)
        {
            _openWeatherApi = new RestClient(_configuration.OpenWeatherApi.Server);
        }

        private readonly IRestClient _openWeatherApi;

        #region Memory Cache

        internal static ConcurrentDictionary<int, WeatherEmotionContract> _emotions = new();
        internal static ConcurrentDictionary<int, WeatherCityUserEmotionContract> _cityUserEmotions = new();

        public async Task MemoryCacheRefreshAsync(MemoryCacheDictionary? dictionary = null, int? id = null)
        {
            if (!dictionary.HasValue || dictionary.Value == MemoryCacheDictionary.Emotions)
            {
                if (!id.HasValue)
                    _emotions = new((await _repository.GetEmotionsAsync()).Select(emotionEntity => emotionEntity.MapToEmotionContract()).ToDictionary(emotion => emotion.EmotionId));
                else
                    _emotions[id.Value] = (await _repository.GetEmotionAsync(id.Value) ?? throw new BadRequestException($"{nameof(id)} is invalid")).MapToEmotionContract();
            }

            if (!dictionary.HasValue || dictionary.Value == MemoryCacheDictionary.CityUserEmotions)
            {
                if (!id.HasValue)
                    _cityUserEmotions = new((await _repository.GetCityUserEmotionsAsync(DateTimeOffset.Now.AddDays(-1))).Select(cityUserEmotionEntity => cityUserEmotionEntity.MapToCityUserEmotionContract()).ToDictionary(cityUserEmotion => cityUserEmotion.CityUserEmotionId));
                else
                    _cityUserEmotions[id.Value] = (await _repository.GetCityUserEmotionAsync(id.Value) ?? throw new BadRequestException($"{nameof(id)} is invalid")).MapToCityUserEmotionContract();
            }
        }

        public async Task MemoryCacheTrimAsync()
        {
            foreach (var cityUserEmotion in IWeatherMemoryCache.CityUserEmotions.Where(cityUserEmotion => cityUserEmotion.Value.CreatedOn < DateTimeOffset.Now.AddDays(-1)).ToList())
                _cityUserEmotions.TryRemove(cityUserEmotion);

            await Task.CompletedTask;
        }

        #endregion

        #region Get

        public async Task<WeatherCityObservationContract> GetCityObservationAsync(int cityId)
        {
            var memoryCacheKey = $"{{Weather}}.{{WeatherCityObservationContract}}.{{CityId}}={cityId}";

            if (_memoryCache.TryGetValue(memoryCacheKey, out WeatherCityObservationContract cityObservation))
                return cityObservation;

            var city = ILocationService.GetCity(cityId);

            return _memoryCache.Set(
                memoryCacheKey,
                (await GetCityObservationResponseAsync(city.OpenWeatherId))
                    .MapToCityObservationContract(),
                TimeSpan.FromSeconds(_configuration.Duration.CityObservationCacheDurationInSeconds)
            );
        }

        public async Task<List<WeatherCityForecastContract>> GetCityForecastsAsync(int cityId)
        {
            var memoryCacheKey = $"{{Weather}}.{{List<WeatherCityForecastContract>}}.{{CityId}}={cityId}";

            if (_memoryCache.TryGetValue(memoryCacheKey, out List<WeatherCityForecastContract> cityForecasts))
                return cityForecasts;

            var city = ILocationService.GetCity(cityId);

            return _memoryCache.Set(
                memoryCacheKey,
                (await GetCityForecastsResponseAsync(city.OpenWeatherId))
                    .MapToCityForecastContracts(),
                TimeSpan.FromSeconds(_configuration.Duration.CityForecastsCacheDurationInSeconds)
            );
        }

        public List<WeatherCityEmotionCountContract> GetCityEmotionCounts(int cityId, DateTimeZone timezone)
        {
            var city = ILocationService.GetCity(cityId);

            return IWeatherMemoryCache.CityUserEmotions.GetCityUserEmotionsAtTimezoneToday(timezone)
                .GroupBy(cityUserEmotion => cityUserEmotion.EmotionId)
                .Select(cityUserEmotionGroup => new WeatherCityEmotionCountContract
                {
                    EmotionId = cityUserEmotionGroup.Key,
                    CityCount = cityUserEmotionGroup.Where(cityUserEmotion => cityUserEmotion.CityId == city.CityId).Count(),
                    GlobalCount = cityUserEmotionGroup.Count()
                })
                .ToList();
        }

        public WeatherCityUserConfigurationContract GetCityUserConfiguration(int userId, int cityId, DateTimeZone timezone)
        {
            var user = IAccountService.GetUser(userId);
            var city = ILocationService.GetCity(cityId);

            var cityUserEmotions = IWeatherMemoryCache.CityUserEmotions.GetCityUserEmotionsAtTimezoneToday(timezone, user.UserId);

            return new WeatherCityUserConfigurationContract
            {
                EmotionId = cityUserEmotions.SingleOrDefault(cityUserEmotion => cityUserEmotion.CityId == city.CityId)?.EmotionId,
                SelectionsToday = cityUserEmotions.Count,
                LimitToday = _configuration.Limit.CreateCityUserEmotionLimit
            };
        }

        public WeatherConfigurationContract GetConfiguration() => new()
        {
            Emotions = IWeatherMemoryCache.Emotions.Select(emotion => emotion.Value).ToList()
        };

        #endregion

        #region Create

        public async Task<WeatherCityUserEmotionContract> CreateCityUserEmotionAsync(int userId, int cityId, int emotionId, DateTimeZone timezone)
        {
            var user = IAccountService.GetUser(userId);
            var city = ILocationService.GetCity(cityId);
            var emotion = IWeatherMemoryCache.GetEmotion(emotionId);

            var cityUserEmotions = IWeatherMemoryCache.CityUserEmotions.GetCityUserEmotionsAtTimezoneToday(timezone, user.UserId);

            if (cityUserEmotions.Any(cityUserEmotion => cityUserEmotion.CityId == city.CityId))
                throw new BadRequestException("You already selected an emotion");

            if (cityUserEmotions.Count >= _configuration.Limit.CreateCityUserEmotionLimit)
                throw new BadRequestException("You have reached your daily limit. Come back tomorrow!");

            var cityUserEmotion = (await _repository.AddAndSaveChangesAsync(new WeatherCityUserEmotionEntity
            {
                CityId = city.CityId,
                UserId = user.UserId,
                EmotionId = emotion.EmotionId,
                CreatedOn = DateTimeOffset.Now.AtTimezone(timezone)
            })).MapToCityUserEmotionContract();

            _cityUserEmotions.TryAdd(cityUserEmotion.CityUserEmotionId, cityUserEmotion);

            return cityUserEmotion;
        }

        #endregion

        #region Open Weather Api

        private async Task<WeatherCityObservationResponse> GetCityObservationResponseAsync(int openWeatherId) =>
            (await _openWeatherApi.ExecuteGetAsync<WeatherCityObservationResponse>(
                new RestRequest("data/2.5/weather")
                    .AddQueryParameter("APPID", _configuration.OpenWeatherApi.AuthenticationKey)
                    .AddQueryParameter("units", "imperial")
                    .AddParameter("id", openWeatherId)
            )).GetData(isSuccess: (response) =>
            {
                if (response is null || response.Conditions is null || response.Temperature is null || response.City is null)
                    return false;

                if (response.Wind is null)
                    response.Wind = new WeatherWindResponse
                    {
                        Degrees = 0,
                        Speed = 0
                    };

                if (response.Cloud is null)
                    response.Cloud = new WeatherCloudResponse
                    {
                        Cloudiness = 0
                    };

                if (response.Rain is null)
                    response.Rain = new WeatherRainResponse
                    {
                        OneHourVolume = 0,
                        ThreeHourVolume = 0
                    };

                if (response.Snow is null)
                    response.Snow = new WeatherSnowResponse
                    {
                        OneHourVolume = 0,
                        ThreeHourVolume = 0
                    };

                return true;
            });

        private async Task<WeatherForecastResponse> GetCityForecastsResponseAsync(int openWeatherId) =>
            (await _openWeatherApi.ExecuteGetAsync<WeatherForecastResponse>(
                new RestRequest("data/2.5/forecast")
                    .AddQueryParameter("APPID", _configuration.OpenWeatherApi.AuthenticationKey)
                    .AddQueryParameter("units", "imperial")
                    .AddParameter("id", openWeatherId)
            )).GetData(isSuccess: (response) =>
            {
                if (response is null || response.City is null || response.Forecasts is null || response.Forecasts.Any(forecast => forecast.Temperature is null || forecast.Conditions is null))
                    return false;

                response.Forecasts.ForEach(forecast =>
                {
                    if (forecast.Cloud is null)
                        forecast.Cloud = new WeatherCloudResponse
                        {
                            Cloudiness = 0
                        };

                    if (forecast.Wind is null)
                        forecast.Wind = new WeatherWindResponse
                        {
                            Degrees = 0,
                            Speed = 0
                        };

                    if (forecast.Rain is null)
                        forecast.Rain = new WeatherRainResponse
                        {
                            OneHourVolume = 0,
                            ThreeHourVolume = 0
                        };

                    if (forecast.Snow is null)
                        forecast.Snow = new WeatherSnowResponse
                        {
                            OneHourVolume = 0,
                            ThreeHourVolume = 0
                        };
                });

                return true;
            });

        #endregion
    }

    public interface IWeatherMemoryCache
    {
        Task MemoryCacheRefreshAsync(MemoryCacheDictionary? dictionary = null, int? id = null);
        Task MemoryCacheTrimAsync();

        public static IReadOnlyDictionary<int, WeatherEmotionContract> Emotions => WeatherService._emotions;
        public static IReadOnlyDictionary<int, WeatherCityUserEmotionContract> CityUserEmotions => WeatherService._cityUserEmotions;

        public static WeatherEmotionContract GetEmotion(int emotionId) =>
            Emotions.TryGetValue(emotionId, out WeatherEmotionContract? emotion)
                ? emotion
                : throw new BadRequestException($"{nameof(emotionId)} is invalid");
        public static WeatherCityUserEmotionContract GetCityUserEmotion(int cityUserEmotionId) =>
           CityUserEmotions.TryGetValue(cityUserEmotionId, out WeatherCityUserEmotionContract? cityUserEmotion)
               ? cityUserEmotion
               : throw new BadRequestException($"{nameof(cityUserEmotionId)} is invalid");
    }

    public interface IWeatherService : IWeatherMemoryCache
    {
        Task<WeatherCityUserEmotionContract> CreateCityUserEmotionAsync(int userId, int cityId, int emotionId, DateTimeZone timezone);
        List<WeatherCityEmotionCountContract> GetCityEmotionCounts(int cityId, DateTimeZone timezone);
        Task<List<WeatherCityForecastContract>> GetCityForecastsAsync(int cityId);
        Task<WeatherCityObservationContract> GetCityObservationAsync(int cityId);
        WeatherCityUserConfigurationContract GetCityUserConfiguration(int userId, int cityId, DateTimeZone timezone);
        WeatherConfigurationContract GetConfiguration();
    }
}