using Library.Error;
using Library.Error.Contract;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Library.User.Job
{
    class UserProcessMemoryCacheJob : BaseJob
    {
        public UserProcessMemoryCacheJob(IServiceProvider serviceProvider) : base(serviceProvider,
        (
            delay: TimeSpan.Zero,
            interval: TimeSpan.FromDays(1),
            state: JobState.Finished
        ))
        { }

        protected override async Task StartAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<UserRepository>();

            await UserMemoryCache.RefreshAsync(repository);
        }

        protected override async Task ErrorAsync(Exception exception)
        {
            using var scope = _serviceProvider.CreateScope();
            var errorService = scope.ServiceProvider.GetRequiredService<IErrorService>();

            await errorService.CreateErrorProcessingAsync(new ErrorProcessingCreateContract
            (
                className: nameof(UserProcessMemoryCacheJob),
                classMethodName: nameof(UserProcessMemoryCacheJob.StartAsync),
                exceptionType: exception.GetType().Name,
                exceptionMessage: exception.GetFullMessage()
            ));
        }
    }
}