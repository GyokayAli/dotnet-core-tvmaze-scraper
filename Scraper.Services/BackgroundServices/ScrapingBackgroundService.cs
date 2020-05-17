using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Scraper.Services.IServices;
using Scraper.Repositories.IRepositories;
using NCrontab;

namespace Scraper.Services.BackgroundServices
{
    public class ScrapingBackgroundService : BackgroundService
    {
        private string FirstRunSchedule => "*/2 * * * * *"; // Every 2 seconds
        private string Schedule => "0 0 0 * * * "; // Every day at midnight - 12am
        private CrontabSchedule _schedule;
        private DateTime _nextRun;

        private readonly ILogger<ScrapingBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceFactory;

        public ScrapingBackgroundService(IServiceScopeFactory servicefactory, ILogger<ScrapingBackgroundService> logger)
        {
            _logger = logger;
            _serviceFactory = servicefactory;

            // Scheduled to start processing immediately on first run
            _schedule = CrontabSchedule.Parse(FirstRunSchedule, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
        }

        /// <summary>
        /// Execute background service
        /// </summary>
        /// <param name="stoppingToken">The cancellation token</param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = DateTime.Now;
                if (now > _nextRun)
                {
                    await StartScraping();

                    // Reschedule next run to a later time
                    _schedule = CrontabSchedule.Parse(Schedule, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
                    _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                }
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        /// <summary>
        /// Starts scraping data from the TVmaze API and persist in database
        /// </summary>
        /// <returns></returns>
        private async Task StartScraping()
        {
            _logger.LogInformation($"Scraping background service has started at {DateTime.Now}.");

            try
            {
                using (var scope = _serviceFactory.CreateScope())
                {
                    var showRepository = scope.ServiceProvider.GetRequiredService<IShowRepository>();
                    var scraperService = scope.ServiceProvider.GetRequiredService<IScraperService>();

                    var lastId = await showRepository.GetLastShowId();
                    var lastPage = lastId / 250; // The shows endpoint is paginated with a maximum of 250 results per page
                    
                    // Continue scraping where left off
                    await scraperService.ScrapeAndStoreShows(lastPage);
                }
            }
            finally
            {
                _logger.LogInformation($"Scraping background service has finished at {DateTime.Now}.");
            }
        }
    }
}