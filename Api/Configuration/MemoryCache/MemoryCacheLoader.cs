using Library.Location;
using Library.Programming;
using Library.StockMarket;
using Library.User;
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

            await Task.WhenAll(
                IUserStartup.LoadMemoryCacheAsync(scope.ServiceProvider),
                ILocationStartup.LoadMemoryCacheAsync(scope.ServiceProvider),
                IWeatherStartup.LoadMemoryCacheAsync(scope.ServiceProvider),
                IStockMarketStartup.LoadMemoryCacheAsync(scope.ServiceProvider),
                IProgrammingStartup.LoadMemoryCacheAsync(scope.ServiceProvider)
            );
        }

        public async Task StopAsync(CancellationToken cancellationToken) =>
            await Task.CompletedTask;
    }
}