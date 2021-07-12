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
        public static void Startup(IServiceCollection services, StockMarketConfiguration configuration, string databaseConnection) =>
            StockMarketStartup.Startup(services, configuration, databaseConnection);

        public static object GetStatus() =>
            StockMarketStartup.GetStatus();
    }

    static class StockMarketStartup
    {
        public static bool IsReady { get; private set; }

        public static void Startup(IServiceCollection services, StockMarketConfiguration configuration, string databaseConnection)
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
                .AddHostedService<StockMarketProcessQuotesJob>()
                .AddHostedService<StockMarketProcessQuoteUserAlertsJob>()
                .AddHostedService<StockMarketProcessMemoryCacheJob>();

            IsReady = true;
        }

        public static object GetStatus() => new
        {
            IsReady,
            IsMemoryCacheReady = StockMarketMemoryCache.IsReady,
            RegisteredType = new
            {
                Internal = new[]
                {
                    nameof(StockMarketContext),
                    nameof(StockMarketRepository),
                    nameof(StockMarketService),
                    nameof(StockMarketMemoryCache),
                    nameof(StockMarketConfiguration)
                },
                Public = new[]
                {
                    nameof(IStockMarketRepository),
                    nameof(IStockMarketService),
                    nameof(IStockMarketMemoryCache)
                },
                Job = new[]
                {
                    nameof(StockMarketProcessQuotesJob),
                    nameof(StockMarketProcessQuoteUserAlertsJob),
                    nameof(StockMarketProcessMemoryCacheJob)
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