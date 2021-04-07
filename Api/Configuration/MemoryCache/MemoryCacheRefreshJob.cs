using Api.Library;
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

            //Order is very important
            //Must match project build order

            await IUserStartup.RefreshMemoryCacheAsync(userRepository);

            IsReady = true;

            await Task.WhenAll(ILibraryMemoryCache.Jobs.Where(job => job.Value.IsDependentOnCache).Select(job => job.Value.ResumeAsync()));
        }

        protected override Task ErrorAsync(System.Exception exception) =>
            Task.CompletedTask;
    }
}