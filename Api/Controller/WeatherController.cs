using Api.Configuration.Authorization;
using Library.User.Enum;
using Library.Weather;
using Library.Weather.Contract;
using Library.Weather.Enum;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controller
{
    public class WeatherController : ApiController<IWeatherService>
    {
        public WeatherController(IWeatherService service) : base(service) { }

        [HttpGet("City/Observation"), ProducesResponseType(typeof(WeatherCityObservationContract), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 59)]
        public async Task<IActionResult> GetCityObservationAsync([Required, Range(1, int.MaxValue)] int cityId) =>
            Ok(await _service.GetCityObservationAsync(cityId));

        [HttpGet("City/Forecasts"), ProducesResponseType(typeof(List<WeatherCityObservationContract>), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 59)]
        public async Task<IActionResult> GetCityForecastsAsync([Required, Range(1, int.MaxValue)] int cityId) =>
            Ok(await _service.GetCityForecastsAsync(cityId));

        [HttpGet("City/Emotion/Counts"), ProducesResponseType(typeof(List<WeatherCityEmotionCountContract>), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 4)]
        public async Task<IActionResult> GetCityEmotionCountsAsync([Required, Range(1, int.MaxValue)] int cityId) =>
            Ok(await Task.FromResult(_service.GetCityEmotionCounts(cityId, Timezone)));

        [HttpPost("City/UserEmotion"), ProducesResponseType(typeof(WeatherCityUserEmotionContract), (int)HttpStatusCode.OK)]
        [Authorization((int)UserType.Google)]
        public async Task<IActionResult> CreateCityUserEmotionAsync([Required, Range(1, int.MaxValue)] int cityId, [Required, Range(1, int.MaxValue)] Emotion emotionId) =>
            Ok(await _service.CreateCityUserEmotionAsync(UserId.Value, cityId, (int)emotionId, Timezone));

        [HttpGet("City/UserConfiguration"), ProducesResponseType(typeof(WeatherCityUserConfigurationContract), (int)HttpStatusCode.OK)]
        [Authorization((int)UserType.Google)]
        public async Task<IActionResult> GetCityUserConfigurationAsync([Required, Range(1, int.MaxValue)] int cityId) =>
            Ok(await Task.FromResult(_service.GetCityUserConfiguration(UserId.Value, cityId, Timezone)));

        [HttpGet("Configuration"), ProducesResponseType(typeof(WeatherConfigurationContract), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 299)]
        public async Task<IActionResult> GetConfigurationAsync() =>
            Ok(await Task.FromResult(_service.GetConfiguration()));
    }
}