using Library.Account;
using Library.Location;
using Library.Programming;
using Library.StockMarket;
using Library.Weather;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Configuration.MemoryCache
{
    class MemoryCacheLoader : IHostedService
    {
        public MemoryCacheLoader(IServiceProvider serviceProvider) =>
            _serviceProvider = serviceProvider;

        private readonly IServiceProvider _serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var weatherService = scope.ServiceProvider.GetRequiredService<IWeatherService>();
            var stockMarketService = scope.ServiceProvider.GetRequiredService<IStockMarketService>();
            var programmingService = scope.ServiceProvider.GetRequiredService<IProgrammingService>();

            await Task.WhenAll(
                IAccountStartup.LoadMemoryCacheAsync(scope.ServiceProvider),
                ILocationStartup.LoadMemoryCacheAsync(scope.ServiceProvider),
                weatherService.MemoryCacheRefreshAsync(),
                stockMarketService.MemoryCacheRefreshAsync(),
                programmingService.MemoryCacheRefreshAsync()
            );
        }

        public async Task StopAsync(CancellationToken cancellationToken) =>
            await Task.CompletedTask;
    }
}