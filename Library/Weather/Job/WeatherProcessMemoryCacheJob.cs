using Library.Error;
using Library.Error.Contract;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Library.Weather.Job
{
    class WeatherProcessMemoryCacheJob : BaseJob
    {
        public WeatherProcessMemoryCacheJob(IServiceProvider serviceProvider) : base(serviceProvider,
        (
            delay: TimeSpan.Zero,
            interval: TimeSpan.FromDays(1),
            state: JobState.Finished
        ))
        { }

        protected override async Task StartAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<WeatherRepository>();

            await WeatherMemoryCache.RefreshAsync(repository);
        }

        protected override async Task ErrorAsync(Exception exception)
        {
            using var scope = _serviceProvider.CreateScope();
            var errorService = scope.ServiceProvider.GetRequiredService<IErrorService>();

            await errorService.CreateErrorProcessingAsync(new ErrorProcessingCreateContract
            (
                className: nameof(WeatherProcessMemoryCacheJob),
                classMethodName: nameof(WeatherProcessMemoryCacheJob.StartAsync),
                exceptionType: exception.GetType().Name,
                exceptionMessage: exception.GetFullMessage()
            ));
        }
    }
}