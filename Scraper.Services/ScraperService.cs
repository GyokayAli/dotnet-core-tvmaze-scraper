using Microsoft.Extensions.Logging;
using Scraper.Common.TVmaze;
using Scraper.Repositories.IRepositories;
using Scraper.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Scraper.Services
{
    public class ScraperService : IScraperService
    {
        private const int MaxPage = 1000; // Assuming there won't be more than 1K pages of shows in the VMmaze API (192 as of now)
        private const string EndpointShows = "/shows?page={0}";
        private const string EndpointCast = "/shows/{0}/cast";

        private readonly HttpClient _httpClient;
        private readonly IShowRepository _showRepository;
        private readonly ILogger<ScraperService> _logger;

        public ScraperService(HttpClient httpClient, IShowRepository showRepository, ILogger<ScraperService> logger)
        {
            _httpClient = httpClient;
            _showRepository = showRepository;
            _logger = logger;
        }

        /// <summary>
        /// Scrapes shows including cast members and persists in storage 
        /// </summary>
        /// <param name="startPage">The page to start scraping from</param>
        /// <returns></returns>
        public async Task ScrapeAndStoreShows(int startPage)
        {
            for (int i = startPage; i < MaxPage; i++)
            {
                try
                {
                    var response = await _httpClient.GetAsync(string.Format(EndpointShows, i));

                    // StatusCode of NotFound expected after the last page available
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        break;
                    }

                    // Read shows from the response
                    var currentShows = await response.Content.ReadAsAsync<ICollection<TVmazeShow>>();

                    // Scrape cast members for each show
                    foreach (var show in currentShows)
                    {
                        show.Cast = await ScrapeCast(show.Id);
                    }

                    // Store scraped data from the current page
                    if (currentShows.Count > 0)
                        await _showRepository.StoreShows(currentShows);
                }
                catch (Exception ex)
                {
                    string message = $"Failed to scrape shows from page {startPage}";

                    _logger.LogError(ex, message);
                    throw new Exception(message, ex);
                }
            }
        }

        /// <summary>
        /// Scrapes cast for the requested show
        /// </summary>
        /// <param name="showId">The show id</param>
        /// <returns></returns>
        private async Task<ICollection<TVmazePerson>> ScrapeCast(int showId)
        {
            var response = await _httpClient.GetAsync(string.Format(EndpointCast, showId));
            response.EnsureSuccessStatusCode(); // TODO: double check
            
            // Read cast from the response
            var result = await response.Content.ReadAsAsync<ICollection<TVmazeCast>>();

            return result.Select(x => x.Person).ToList();
        }
    }
}