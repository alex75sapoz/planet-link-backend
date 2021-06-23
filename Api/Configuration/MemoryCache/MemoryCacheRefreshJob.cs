using Library.Base;
using Library.Error;
using Library.Error.Contract;
using Library.Location;
using Library.Programming;
using Library.StockMarket;
using Library.User;
using Library.Weather;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Configuration.MemoryCache
{
    internal class MemoryCacheRefreshJob : BaseJob
    {
        public MemoryCacheRefreshJob(IServiceProvider serviceProvider) : base(serviceProvider,
            (
                delay: TimeSpan.Zero,
                interval: TimeSpan.FromDays(1),
                isDependentOnCache: false
            ))
        { }

        protected override async Task StartAsync()
        {
            await Task.WhenAll(ILibraryMemoryCache.Jobs.Where(job => job.Value.IsDependentOnCache).Select(job => job.Value.PauseAsync()));

            using var scope = _serviceProvider.CreateScope();

            await IUserStartup.RefreshMemoryCacheAsync(scope.ServiceProvider);
            await ILocationStartup.RefreshMemoryCacheAsync(scope.ServiceProvider);
            await IWeatherStartup.RefreshMemoryCacheAsync(scope.ServiceProvider);
            await IStockMarketStartup.RefreshMemoryCacheAsync(scope.ServiceProvider);
            await IProgrammingStartup.RefreshMemoryCacheAsync(scope.ServiceProvider);

            await Task.WhenAll(ILibraryMemoryCache.Jobs.Where(job => job.Value.IsDependentOnCache).Select(job => job.Value.ResumeAsync()));
        }

        protected override async Task ErrorAsync(System.Exception exception)
        {
            using var scope = _serviceProvider.CreateScope();
            var errorService = scope.ServiceProvider.GetRequiredService<IErrorService>();

            await errorService.CreateErrorProcessingAsync(new ErrorProcessingCreateContract
            (
                className: nameof(MemoryCacheRefreshJob),
                classMethodName: nameof(MemoryCacheRefreshJob.StartAsync),
                exceptionType: exception.GetType().Name,
                exceptionMessage: exception.GetFullMessage()
            ));
        }
    }
}