using Library.Application;
using Library.Application.Contract;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Library.StockMarket.Job
{
    class StockMarketProcessQuotesJob : BaseJob
    {
        public StockMarketProcessQuotesJob(IServiceProvider serviceProvider) : base(serviceProvider, (
            delay: TimeSpan.FromDays(1),
            interval: TimeSpan.FromDays(1),
            state: JobState.Paused
        ))
        { }

        protected override async Task StartAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<StockMarketService>();

            await service.ProcessQuotesAsync();
        }

        protected override async Task ErrorAsync(Exception exception)
        {
            using var scope = _serviceProvider.CreateScope();
            var applicationService = scope.ServiceProvider.GetRequiredService<IApplicationService>();

            await applicationService.CreateErrorProcessingAsync(new ApplicationErrorProcessingCreateContract
            (
                className: nameof(StockMarketProcessQuotesJob),
                classMethodName: nameof(StockMarketProcessQuotesJob.StartAsync),
                exceptionType: exception.GetType().Name,
                exceptionMessage: exception.GetFullMessage()
            ));
        }

    }
}