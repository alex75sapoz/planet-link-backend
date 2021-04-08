using Api.Library.Error;
using Api.Library.Error.Contract;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Api.Library.StockMarket.Job
{
    internal class StockMarketProcessQuotesJob : LibraryJob
    {
        public StockMarketProcessQuotesJob(IServiceProvider serviceProvider) : base(serviceProvider, (
            delay: TimeSpan.FromDays(1),
            interval: TimeSpan.FromDays(1),
            isDependentOnCache: true
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
            var errorService = scope.ServiceProvider.GetRequiredService<IErrorService>();

            await errorService.CreateErrorProcessingAsync(new ErrorProcessingContract()
            {
                ClassName = nameof(StockMarketProcessQuotesJob),
                ClassMethodName = nameof(StockMarketProcessQuotesJob.StartAsync),
                ExceptionType = exception.GetType().Name,
                ExceptionMessage = exception.GetFullMessage()
            });
        }

    }
}