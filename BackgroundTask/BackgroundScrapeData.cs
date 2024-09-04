
using Limbus_wordle.util.WebScrapper;

namespace Limbus_wordle.BackgroundTask
{
    public class BackgroundScrapeData(ScrapeIdentities scrapeIdentities) : IHostedService, IDisposable
    {
        private readonly ScrapeIdentities _scrapeIdentities = scrapeIdentities;
        private Timer _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(async state=> await DoWork(), null, TimeSpan.Zero, TimeSpan.FromMinutes(60*24));
            return Task.CompletedTask;
        }

        private async Task DoWork()
        {
            await _scrapeIdentities.ScrapAsync();
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