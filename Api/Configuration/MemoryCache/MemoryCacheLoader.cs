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

        public async Task StartAsync(CancellationToken cancellationToken) =>
            await Task.WhenAll(
                Task.Run(async () => await _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IAccountService>().MemoryCacheRefreshAsync(), cancellationToken),
                Task.Run(async () => await _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ILocationService>().MemoryCacheRefreshAsync(), cancellationToken),
                Task.Run(async () => await _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IWeatherService>().MemoryCacheRefreshAsync(), cancellationToken),
                Task.Run(async () => await _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IStockMarketService>().MemoryCacheRefreshAsync(), cancellationToken),
                Task.Run(async () => await _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IProgrammingService>().MemoryCacheRefreshAsync(), cancellationToken)
            );

        public async Task StopAsync(CancellationToken cancellationToken) =>
            await Task.CompletedTask;
    }
}