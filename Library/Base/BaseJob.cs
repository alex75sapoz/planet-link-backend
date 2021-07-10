using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Base
{
    public abstract class BaseJob : IHostedService
    {
        protected BaseJob(IServiceProvider serviceProvider, (TimeSpan delay, TimeSpan interval, JobState state) options)
        {
            _serviceProvider = serviceProvider;
            Delay = options.delay;
            Interval = options.interval;
            State = options.state;
        }

        protected readonly IServiceProvider _serviceProvider;

        public TimeSpan Delay { get; private set; }
        public TimeSpan Interval { get; private set; }
        public Timer? Timer { get; private set; }
        public JobState State { get; private set; }
        public DateTime NextStartOn { get; private set; }

        public enum JobState
        {
            ///<summary>
            ///Initial State = Start immediately blocking the startup thread
            ///<br/>
            ///Normal State = Currently running
            ///</summary>
            Started = 1,
            ///<summary>
            ///Initial State = Start based on the schedule in the background
            ///<br/>
            ///Normal State = Currently not running
            ///</summary>
            Finished = 2,
            ///<summary>
            ///Initial State = Pause immediately
            ///<br/>
            ///Normal State = Currently not running
            ///</summary>
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
                catch (Exception exception)
                {
                    await ErrorAsync(exception);
                }

                State = JobState.Finished;
            }, null, Timeout.Infinite, Timeout.Infinite);
            NextStartOn = DateTime.Now + Delay;

            BaseMemoryCache.Jobs.TryAdd(GetType().Name, this);

            if (State == JobState.Paused)
                return;

            if (State == JobState.Finished)
                await ResumeAsync();

            if (State == JobState.Started)
            {
                try
                {
                    await StartAsync();
                }
                catch (Exception exception)
                {
                    await ErrorAsync(exception);
                }

                await ResumeAsync();
            }
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            await PauseAsync();
            await Timer!.DisposeAsync();
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

            Timer!.Change(Timeout.Infinite, Timeout.Infinite);
            State = JobState.Paused;
        }

        public async Task ResumeAsync()
        {
            Timer!.Change(
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