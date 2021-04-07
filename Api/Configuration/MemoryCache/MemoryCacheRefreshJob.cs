using Api.Library;
using Api.Library.Error;
using Api.Library.Error.Contract;
using Api.Library.User;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Configuration.MemoryCache
{
    internal class MemoryCacheRefreshJob : LibraryJob
    {
        public MemoryCacheRefreshJob(IServiceProvider serviceProvider) : base(serviceProvider,
            (
                delay: TimeSpan.Zero,
                interval: TimeSpan.FromDays(1),
                isDependentOnCache: false
            ))
        { }

        public static bool IsReady { get; set; }

        protected override async Task StartAsync()
        {
            await Task.WhenAll(ILibraryMemoryCache.Jobs.Where(job => job.Value.IsDependentOnCache).Select(job => job.Value.PauseAsync()));

            IsReady = false;

            using var scope = _serviceProvider.CreateScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            //Order must match project build order

            await IUserStartup.RefreshMemoryCacheAsync(userRepository);

            IsReady = true;

            await Task.WhenAll(ILibraryMemoryCache.Jobs.Where(job => job.Value.IsDependentOnCache).Select(job => job.Value.ResumeAsync()));
        }

        protected override async Task ErrorAsync(System.Exception exception)
        {
            using var scope = _serviceProvider.CreateScope();
            var errorService = scope.ServiceProvider.GetRequiredService<IErrorService>();

            await errorService.CreateErrorProcessingAsync(new ErrorProcessingContract()
            {
                ClassName = nameof(MemoryCacheRefreshJob),
                ClassMethodName = nameof(MemoryCacheRefreshJob.StartAsync),
                ExceptionType = exception.GetType().Name,
                ExceptionMessage = exception.GetFullMessage()
            });
        }
    }
}