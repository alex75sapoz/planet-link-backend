using Library.Application;
using Library.Application.Contract;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Library.StockMarket.Job
{
    class StockMarketProcessQuoteUserAlertsJob : BaseJob
    {
        public StockMarketProcessQuoteUserAlertsJob(IServiceProvider serviceProvider) : base(serviceProvider, (
            delay: TimeSpan.FromMinutes(30),
            interval: TimeSpan.FromHours(1),
            state: JobState.Finished
        ))
        { }

        protected override async Task StartAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IStockMarketService>();

            await service.ProcessQuoteUserAlertsReverseSplitsAsync();
            await service.ProcessQuoteUserAlertsInProgressAsync((int)AlertCompletedType.Automatic);
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