global using Library.Base;
global using Library.StockMarket.Contract;
global using Library.StockMarket.Entity;
global using Library.StockMarket.Enum;
global using Library.StockMarket.Job;
global using Library.StockMarket.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Library.StockMarket
{
    public interface IStockMarketStartup
    {
        public static void ConfigureServices(IServiceCollection services, StockMarketConfiguration configuration, string databaseConnection) =>
            StockMarketStartup.ConfigureServices(services, configuration, databaseConnection);

        public static async Task LoadMemoryCacheAsync(IServiceProvider serviceProvider) =>
            await StockMarketMemoryCache.LoadAsync(serviceProvider.GetRequiredService<StockMarketRepository>());

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
                TotalAlertCompletedTypes = IStockMarketService.AlertCompletedTypes.Count,
                TotalAlertTypes = IStockMarketService.AlertTypes.Count,
                TotalEmotions = IStockMarketService.Emotions.Count,
                TotalExchanges = IStockMarketService.Exchanges.Count,
                TotalQuotes = IStockMarketService.Quotes.Count,
                TotalQuoteUserAlerts = IStockMarketService.QuoteUserAlerts.Count,
                TotalQuoteUserEmotions = IStockMarketService.QuoteUserEmotions.Count,
                TotalTimeframes = IStockMarketService.Timeframes.Count
            }
        };
    }
}