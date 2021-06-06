using Api.Configuration.Authorization;
using Library.StockMarket;
using Library.StockMarket.Contract;
using Library.StockMarket.Enum;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace Api.Controller
{
    [Authorization(Requirement.StockMarketMemoryCache), Authorization(Requirement.UserMemoryCache)]
    public class StockMarketController : ApiController<IStockMarketService>
    {
        public StockMarketController(IStockMarketService service) : base(service) { }

        [HttpGet("Global"), ProducesResponseType(typeof(StockMarketGlobalContract), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 59)]
        public async Task<IActionResult> GetGlobalAsync() =>
            Ok(await _service.GetGlobalAsync());

        [HttpGet("Quote"), ProducesResponseType(typeof(StockMarketQuoteContract), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 299)]
        public async Task<IActionResult> GetQuoteAsync([Required, Range(1, int.MaxValue)] int quoteId) =>
            Ok(await Task.FromResult(_service.GetQuote(quoteId)));

        [HttpGet("Quote/Search"), ProducesResponseType(typeof(List<StockMarketQuoteContract>), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 299)]
        public async Task<IActionResult> SearchQuotesAsync([Required] string keyword) =>
           Ok(await Task.FromResult(_service.SearchQuotesAsync(keyword.ToUpper())));

        [HttpGet("Quote/Price"), ProducesResponseType(typeof(StockMarketQuotePriceContract), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 9)]
        public async Task<IActionResult> GetQuotePriceAsync([Required, Range(1, int.MaxValue)] int quoteId) =>
            Ok(await _service.GetQuotePriceAsync(quoteId));

        [HttpGet("Quote/Company"), ProducesResponseType(typeof(StockMarketQuoteCompanyContract), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 299)]
        public async Task<IActionResult> GetQuoteCompanyAsync([Required, Range(1, int.MaxValue)] int quoteId) =>
            Ok(await _service.GetQuoteCompanyAsync(quoteId));

        [HttpGet("Quote/Candles"), ProducesResponseType(typeof(List<StockMarketQuoteCandleContract>), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 59)]
        public async Task<IActionResult> GetQuoteCandlesAsync([Required, Range(1, int.MaxValue)] int quoteId, [Required, Range(1, int.MaxValue)] Timeframe timeframeId) =>
            Ok(await _service.GetQuoteCandlesAsync(quoteId, (int)timeframeId));

        [HttpGet("Quote/Emotion/Counts"), ProducesResponseType(typeof(List<StockMarketQuoteEmotionCountContract>), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 4)]
        public async Task<IActionResult> GetQuoteEmotionCountsAsync([Required, Range(1, int.MaxValue)] int quoteId) =>
            Ok(await Task.FromResult(_service.GetQuoteEmotionCounts(quoteId, Timezone)));

        [HttpPost("Quote/UserEmotion"), ProducesResponseType(typeof(StockMarketQuoteUserEmotionContract), (int)HttpStatusCode.OK)]
        [Authorization(Requirement.UserTypeStocktwits)]
        public async Task<IActionResult> CreateQuoteUserEmotionAsync([Required, Range(1, int.MaxValue)] int quoteId, [Required, Range(1, int.MaxValue)] Emotion emotionId) =>
            Ok(await _service.CreateQuoteUserEmotionAsync(UserId.Value, quoteId, (int)emotionId, Timezone));

        [HttpGet("Quote/UserAlert/Search"), ProducesResponseType(typeof(List<StockMarketQuoteUserAlertContract>), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 9)]
        public async Task<IActionResult> SearchQuoteUserAlertsAsync([Required, Range(1, int.MaxValue)] AlertType alertTypeId, [Range(1, int.MaxValue)] int? userId, [Range(1, int.MaxValue)] int? quoteId) =>
            Ok(await Task.FromResult(_service.SearchQuoteUserAlerts((int)alertTypeId, userId, quoteId)));

        [HttpPost("Quote/UserAlert"), ProducesResponseType(typeof(StockMarketQuoteUserAlertContract), (int)HttpStatusCode.OK)]
        [Authorization(Requirement.UserTypeStocktwits)]
        public async Task<IActionResult> CreateQuoteUserAlertAsync([Required, Range(1, int.MaxValue)] int quoteId, [Required, Range(0.000001, int.MaxValue)] decimal sell, [Required, Range(0.000001, int.MaxValue)] decimal stopLoss) =>
            Ok(await _service.CreateQuoteUserAlertAsync(UserId.Value, quoteId, (sell, stopLoss), Timezone));

        [HttpPut("Quote/UserAlert"), ProducesResponseType(typeof(StockMarketQuoteUserAlertContract), (int)HttpStatusCode.OK)]
        [Authorization(Requirement.UserTypeStocktwits)]
        public async Task<IActionResult> CompleteQuoteUserAlertAsync([Required, Range(1, int.MaxValue)] int quoteUserAlertId) =>
            Ok(await _service.CompleteQuoteUserAlertAsync(UserId.Value, quoteUserAlertId));

        [HttpGet("Quote/UserConfiguration"), ProducesResponseType(typeof(StockMarketQuoteUserConfigurationContract), (int)HttpStatusCode.OK)]
        [Authorization(Requirement.UserTypeStocktwits)]
        public async Task<IActionResult> GetQuoteUserConfigurationAsync([Required, Range(1, int.MaxValue)] int quoteId) =>
            Ok(await Task.FromResult(_service.GetQuoteUserConfiguration(UserId.Value, quoteId, Timezone)));

        [HttpGet("Configuration"), ProducesResponseType(typeof(StockMarketConfigurationContract), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 299)]
        public async Task<IActionResult> GetConfigurationAsync() =>
            Ok(await Task.FromResult(_service.GetConfiguration()));

        [HttpGet("User"), ProducesResponseType(typeof(StockMarketUserContract), (int)HttpStatusCode.OK)]
        [ResponseCache(Duration = 59)]
        public async Task<IActionResult> GetUserAsync(int userId) =>
            Ok(await Task.FromResult(_service.GetUser(userId)));
    }
}