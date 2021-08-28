using Library.Application;
using Library.Application.Contract;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Library.Weather.Job
{
    class WeatherProcessMemoryCacheJob : BaseJob
    {
        public WeatherProcessMemoryCacheJob(IServiceProvider serviceProvider) : base(serviceProvider,
        (
            delay: TimeSpan.FromDays(1),
            interval: TimeSpan.FromDays(1),
            state: JobState.Finished
        ))
        { }

        protected override async Task StartAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IWeatherService>();
            await service.MemoryCacheTrimAsync();
        }

        protected override async Task ErrorAsync(Exception exception)
        {
            using var scope = _serviceProvider.CreateScope();
            var applicationService = scope.ServiceProvider.GetRequiredService<IApplicationService>();

            await applicationService.CreateErrorProcessingAsync(new ApplicationErrorProcessingCreateContract
            (
                className: nameof(WeatherProcessMemoryCacheJob),
                classMethodName: nameof(WeatherProcessMemoryCacheJob.StartAsync),
                exceptionType: exception.GetType().Name,
                exceptionMessage: exception.GetFullMessage()
            ));
        }
    }
}