using Library.Error;
using Library.Error.Contract;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Library.StockMarket.Job
{
    class StockMarketProcessMemoryCacheJob : BaseJob
    {
        public StockMarketProcessMemoryCacheJob(IServiceProvider serviceProvider) : base(serviceProvider,
        (
            delay: TimeSpan.Zero,
            interval: TimeSpan.FromDays(1),
            state: JobState.Finished
        ))
        { }

        private static readonly string[] _dependentJobKeys = new string[]
        {
            nameof(StockMarketProcessQuotesJob),
            nameof(StockMarketProcessQuoteUserAlertsJob)
        };

        protected override async Task StartAsync()
        {
            await Task.WhenAll(_dependentJobKeys.Select(dependentJobKey => ILibraryMemoryCache.Jobs[dependentJobKey].PauseAsync()));

            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<StockMarketRepository>();

            await StockMarketMemoryCache.RefreshAsync(repository);

            await Task.WhenAll(_dependentJobKeys.Select(dependentJobKey => ILibraryMemoryCache.Jobs[dependentJobKey].ResumeAsync()));
        }

        protected override async Task ErrorAsync(Exception exception)
        {
            using var scope = _serviceProvider.CreateScope();
            var errorService = scope.ServiceProvider.GetRequiredService<IErrorService>();

            await errorService.CreateErrorProcessingAsync(new ErrorProcessingCreateContract
            (
                className: nameof(StockMarketProcessMemoryCacheJob),
                classMethodName: nameof(StockMarketProcessMemoryCacheJob.StartAsync),
                exceptionType: exception.GetType().Name,
                exceptionMessage: exception.GetFullMessage()
            ));
        }
    }
}