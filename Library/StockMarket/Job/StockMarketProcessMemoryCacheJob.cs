using Library.Application;
using Library.Application.Contract;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Library.StockMarket.Job
{
    class StockMarketProcessMemoryCacheJob : BaseJob
    {
        public StockMarketProcessMemoryCacheJob(IServiceProvider serviceProvider) : base(serviceProvider,
        (
            delay: TimeSpan.FromDays(1),
            interval: TimeSpan.FromDays(1),
            state: JobState.Finished
        ))
        { }

        protected override async Task StartAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<StockMarketRepository>();

            await StockMarketMemoryCache.TrimAsync(repository);
        }

        protected override async Task ErrorAsync(Exception exception)
        {
            using var scope = _serviceProvider.CreateScope();
            var applicationService = scope.ServiceProvider.GetRequiredService<IApplicationService>();

            await applicationService.CreateErrorProcessingAsync(new ApplicationErrorProcessingCreateContract
            (
                className: nameof(StockMarketProcessMemoryCacheJob),
                classMethodName: nameof(StockMarketProcessMemoryCacheJob.StartAsync),
                exceptionType: exception.GetType().Name,
                exceptionMessage: exception.GetFullMessage()
            ));
        }
    }
}