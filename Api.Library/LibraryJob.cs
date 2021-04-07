using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Library
{
    public abstract class LibraryJob : IHostedService
    {
        protected LibraryJob(IServiceProvider serviceProvider, (TimeSpan delay, TimeSpan interval, bool isDependentOnCache) options)
        {
            _serviceProvider = serviceProvider;
            Delay = options.delay;
            Interval = options.interval;
            IsDependentOnCache = options.isDependentOnCache;
        }

        protected readonly IServiceProvider _serviceProvider;

        public TimeSpan Delay { get; private set; }
        public TimeSpan Interval { get; private set; }
        public bool IsDependentOnCache { get; private set; }
        public Timer Timer { get; private set; }
        public JobState State { get; private set; }
        public DateTime NextStartOn { get; private set; }

        public enum JobState
        {
            Started = 1,
            Finished = 2,
            Paused = 3
        }

        #region Step 1

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            Timer = new Timer(async (state) =>
            {
                State = JobState.Started;
                NextStartOn += Interval;

                try
                {
                    await StartAsync();
                }
                catch (System.Exception exception)
                {
                    await ErrorAsync(exception);
                }

                State = JobState.Finished;
            }, null, Timeout.Infinite, Timeout.Infinite);
            State = JobState.Paused;
            NextStartOn = DateTime.Now + Delay;

            LibraryMemoryCache.Jobs.TryAdd(GetType().FullName, this);

            if (!IsDependentOnCache)
                await ResumeAsync();
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            await PauseAsync();
            await Timer.DisposeAsync();
        }

        #endregion

        #region Step 2

        protected abstract Task StartAsync();

        protected abstract Task ErrorAsync(Exception exception);

        #endregion

        #region Step 3

        public async Task PauseAsync()
        {
            while (State != JobState.Paused && State != JobState.Finished)
                await Task.Delay(TimeSpan.FromSeconds(1));

            Timer.Change(Timeout.Infinite, Timeout.Infinite);
            State = JobState.Paused;
        }

        public async Task ResumeAsync()
        {
            Timer.Change(
                NextStartOn < DateTime.Now
                ? TimeSpan.Zero
                : NextStartOn - DateTime.Now,
                Interval
            );
            State = JobState.Finished;
            await Task.CompletedTask;
        }

        #endregion
    }
}