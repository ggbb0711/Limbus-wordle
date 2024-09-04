
using Limbus_wordle.Services;

namespace Limbus_wordle.BackgroundTask
{
    public class BackgroundResetDailyIdentityMode() : IHostedService, IDisposable
    {
        private Timer _timer;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await DoWork();
        }

        private async Task DoWork()
        {
            Console.WriteLine(DateTime.Today.ToString()+" Resetting daily identity");
            await DailyIdentityGameModeService.Reset();
            ScheduleNextRun();
        }

        private void ScheduleNextRun()
        {
            var now = DateTime.Now;
            var nextMidnight = DateTime.Today.AddDays(1);
            var timeUntilNextMidnight = nextMidnight - now;

            _timer = new Timer(async state => await DoWork(), null, timeUntilNextMidnight, Timeout.InfiniteTimeSpan);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}