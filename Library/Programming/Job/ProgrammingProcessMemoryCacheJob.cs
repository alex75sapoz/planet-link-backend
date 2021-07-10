using Library.Error;
using Library.Error.Contract;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Library.Programming.Job
{
    class ProgrammingProcessMemoryCacheJob : BaseJob
    {
        public ProgrammingProcessMemoryCacheJob(IServiceProvider serviceProvider) : base(serviceProvider,
        (
            delay: TimeSpan.Zero,
            interval: TimeSpan.FromDays(1),
            state: JobState.Finished
        ))
        { }

        protected override async Task StartAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ProgrammingRepository>();

            await ProgrammingMemoryCache.RefreshAsync(repository);
        }

        protected override async Task ErrorAsync(Exception exception)
        {
            using var scope = _serviceProvider.CreateScope();
            var errorService = scope.ServiceProvider.GetRequiredService<IErrorService>();

            await errorService.CreateErrorProcessingAsync(new ErrorProcessingCreateContract
            (
                className: nameof(ProgrammingProcessMemoryCacheJob),
                classMethodName: nameof(ProgrammingProcessMemoryCacheJob.StartAsync),
                exceptionType: exception.GetType().Name,
                exceptionMessage: exception.GetFullMessage()
            ));
        }
    }
}