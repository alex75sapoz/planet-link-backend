using Library.Error;
using Library.Error.Contract;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Library.StockMarket.Job
{
    internal class StockMarketProcessQuoteUserAlertsJob : BaseJob
    {
        public StockMarketProcessQuoteUserAlertsJob(IServiceProvider serviceProvider) : base(serviceProvider, (
            delay: TimeSpan.FromMinutes(30),
            interval: TimeSpan.FromHours(1),
            isDependentOnCache: true
        ))
        { }

        protected override async Task StartAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<StockMarketService>();

            await service.ProcessQuoteUserAlertsReverseSplitsAsync();
            await service.ProcessQuoteUserAlertsInProgressAsync((int)AlertCompletedType.Automatic);
        }

        protected override async Task ErrorAsync(Exception exception)
        {
            using var scope = _serviceProvider.CreateScope();
            var errorService = scope.ServiceProvider.GetRequiredService<IErrorService>();

            await errorService.CreateErrorProcessingAsync(new ErrorProcessingCreateContract
            (
                className: nameof(StockMarketProcessQuotesJob),
                classMethodName: nameof(StockMarketProcessQuotesJob.StartAsync),
                exceptionType: exception.GetType().Name,
                exceptionMessage: exception.GetFullMessage()
            ));
        }

    }
}