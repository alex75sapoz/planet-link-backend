global using Library.Base;
global using Library.StockMarket.Contract;
global using Library.StockMarket.Entity;
global using Library.StockMarket.Enum;
global using Library.StockMarket.Job;
global using Library.StockMarket.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.StockMarket
{
    public interface IStockMarketStartup
    {
        public static void ConfigureServices(IServiceCollection services, StockMarketConfiguration configuration, string databaseConnection) =>
            StockMarketStartup.ConfigureServices(services, configuration, databaseConnection);

        public static object GetStatus() =>
            StockMarketStartup.GetStatus();
    }

    static class StockMarketStartup
    {
        public static bool IsReady { get; private set; }

        public static void ConfigureServices(IServiceCollection services, StockMarketConfiguration configuration, string databaseConnection)
        {
            if (IsReady) return;

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
                .AddHostedService<StockMarketProcessMemoryCacheJob>()
                .AddHostedService<StockMarketProcessQuotesJob>()
                .AddHostedService<StockMarketProcessQuoteUserAlertsJob>();

            IsReady = true;
        }

        public static object GetStatus() => new
        {
            IsReady,
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
                    nameof(IStockMarketMemoryCache),
                    nameof(IStockMarketService)
                },
                Job = new[]
                {
                    nameof(StockMarketProcessMemoryCacheJob),
                    nameof(StockMarketProcessQuotesJob),
                    nameof(StockMarketProcessQuoteUserAlertsJob)
                }
            },
            MemoryCache = new
            {
                TotalAlertCompletedTypes = IStockMarketMemoryCache.AlertCompletedTypes.Count,
                TotalAlertTypes = IStockMarketMemoryCache.AlertTypes.Count,
                TotalEmotions = IStockMarketMemoryCache.Emotions.Count,
                TotalExchanges = IStockMarketMemoryCache.Exchanges.Count,
                TotalQuotes = IStockMarketMemoryCache.Quotes.Count,
                TotalQuoteUserAlerts = IStockMarketMemoryCache.QuoteUserAlerts.Count,
                TotalQuoteUserEmotions = IStockMarketMemoryCache.QuoteUserEmotions.Count,
                TotalTimeframes = IStockMarketMemoryCache.Timeframes.Count
            }
        };
    }
}