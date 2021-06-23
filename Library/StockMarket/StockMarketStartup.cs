global using Library.Base;
global using Library.StockMarket.Contract;
global using Library.StockMarket.Entity;
global using Library.StockMarket.Enum;
global using Library.StockMarket.Job;
global using Library.StockMarket.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Library.StockMarket
{
    public interface IStockMarketStartup
    {
        public static bool IsMemoryCacheReady =>
            StockMarketStartup.IsMemoryCacheReady;

        public static void Startup(IServiceCollection services, StockMarketConfiguration configuration, string databaseConnection) =>
            StockMarketStartup.Startup(services, configuration, databaseConnection);

        public static async Task RefreshMemoryCacheAsync(IServiceProvider serviceProvider) =>
            await StockMarketStartup.RefreshMemoryCacheAsync(serviceProvider.GetRequiredService<StockMarketRepository>());

        public static object GetStatus() =>
            StockMarketStartup.GetStatus();
    }

    internal static class StockMarketStartup
    {
        public static bool IsStarted { get; set; }
        public static bool IsMemoryCacheReady { get; set; }

        public static void Startup(IServiceCollection services, StockMarketConfiguration configuration, string databaseConnection)
        {
            IsStarted = false;

            services
                //Internal
                .AddDbContext<StockMarketContext>(options => options.UseSqlServer(databaseConnection))
                .AddTransient<StockMarketRepository>()
                .AddTransient<StockMarketService>()
                .AddSingleton(configuration)
                //Public
                .AddTransient<IStockMarketRepository, StockMarketRepository>()
                .AddTransient<IStockMarketService, StockMarketService>()
                //Job
                .AddHostedService<StockMarketProcessQuotesJob>()
                .AddHostedService<StockMarketProcessQuoteUserAlertsInProgressJob>();

            IsStarted = true;
        }

        public static async Task RefreshMemoryCacheAsync(StockMarketRepository repository)
        {
            IsMemoryCacheReady = false;

            var exchangeEntities = await repository.GetExchangesAsync();
            var timeframeEntities = await repository.GetTimeframesAsync();
            var alertTypeEntities = await repository.GetAlertTypesAsync();
            var alertCompletedTypeEntities = await repository.GetAlertCompletedTypesAsync();
            var emotionEntities = await repository.GetEmotionsAsync();
            var quoteEntities = await repository.GetQuotesAsync();
            var quoteUserAlertEntities = await repository.GetQuoteUserAlertsAsync();
            var quoteUserEmotionEntities = await repository.GetQuoteUserEmotionsAsync(DateTimeOffset.Now.AddDays(-1));

            StockMarketMemoryCache.StockMarketExchanges.Clear();
            foreach (var exchange in exchangeEntities.Select(exchangeEntity => exchangeEntity.MaptToExchangeContract()))
                StockMarketMemoryCache.StockMarketExchanges.TryAdd(exchange.ExchangeId, exchange);

            StockMarketMemoryCache.StockMarketTimeframes.Clear();
            foreach (var timeframe in timeframeEntities.Select(timeframeEntity => timeframeEntity.MaptToTimeframeContract()))
                StockMarketMemoryCache.StockMarketTimeframes.TryAdd(timeframe.TimeframeId, timeframe);

            StockMarketMemoryCache.StockMarketAlertTypes.Clear();
            foreach (var alertType in alertTypeEntities.Select(alertTypeEntity => alertTypeEntity.MaptToAlertTypeContract()))
                StockMarketMemoryCache.StockMarketAlertTypes.TryAdd(alertType.AlertTypeId, alertType);

            StockMarketMemoryCache.StockMarketAlertCompletedTypes.Clear();
            foreach (var alertCompletedType in alertCompletedTypeEntities.Select(alertCompletedTypeEntity => alertCompletedTypeEntity.MaptToAlertCompletedTypeContract()))
                StockMarketMemoryCache.StockMarketAlertCompletedTypes.TryAdd(alertCompletedType.AlertCompletedTypeId, alertCompletedType);

            StockMarketMemoryCache.StockMarketEmotions.Clear();
            foreach (var emotion in emotionEntities.Select(emotionEntity => emotionEntity.MapToEmotionContract()))
                StockMarketMemoryCache.StockMarketEmotions.TryAdd(emotion.EmotionId, emotion);

            StockMarketMemoryCache.StockMarketQuotes.Clear();
            foreach (var quote in quoteEntities.Select(quoteEntity => quoteEntity.MapToQuoteContract()))
                StockMarketMemoryCache.StockMarketQuotes.TryAdd(quote.QuoteId, quote);

            StockMarketMemoryCache.StockMarketQuoteUserAlerts.Clear();
            foreach (var quoteUserAlert in quoteUserAlertEntities.Select(quoteUserAlertEntity => quoteUserAlertEntity.MapToQuoteUserAlertContract()))
                StockMarketMemoryCache.StockMarketQuoteUserAlerts.TryAdd(quoteUserAlert.QuoteUserAlertId, quoteUserAlert);

            StockMarketMemoryCache.StockMarketQuoteUserEmotions.Clear();
            foreach (var quoteUserEmotion in quoteUserEmotionEntities.Select(quoteUserEmotionEntity => quoteUserEmotionEntity.MapToQuoteUserEmotionContract()))
                StockMarketMemoryCache.StockMarketQuoteUserEmotions.TryAdd(quoteUserEmotion.QuoteUserEmotionId, quoteUserEmotion);

            IsMemoryCacheReady = true;
        }

        public static object GetStatus() => new
        {
            IsStarted,
            IsMemoryCacheReady,
            RegisteredType = new
            {
                Internal = new[]
                {
                    nameof(StockMarketContext),
                    nameof(StockMarketRepository),
                    nameof(StockMarketService),
                    nameof(StockMarketConfiguration)
                },
                Public = new[]
                {
                    nameof(IStockMarketRepository),
                    nameof(IStockMarketService)
                },
                Job = new[]
                {
                    nameof(StockMarketProcessQuotesJob),
                    nameof(StockMarketProcessQuoteUserAlertsInProgressJob)
                }
            },
            MemoryCache = new
            {
                TotalAlertCompletedTypes = StockMarketMemoryCache.StockMarketAlertCompletedTypes.Count,
                TotalAlertTypes = StockMarketMemoryCache.StockMarketAlertTypes.Count,
                TotalEmotions = StockMarketMemoryCache.StockMarketEmotions.Count,
                TotalExchanges = StockMarketMemoryCache.StockMarketExchanges.Count,
                TotalQuotes = StockMarketMemoryCache.StockMarketQuotes.Count,
                TotalQuoteUserAlerts = StockMarketMemoryCache.StockMarketQuoteUserAlerts.Count,
                TotalQuoteUserEmotions = StockMarketMemoryCache.StockMarketQuoteUserEmotions.Count,
                TotalTimeframes = StockMarketMemoryCache.StockMarketTimeframes.Count
            }
        };
    }
}