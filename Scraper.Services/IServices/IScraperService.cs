using Scraper.Common.TVmaze;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scraper.Services.IServices
{
    public interface IScraperService
    {
        /// <summary>
        /// Scrapes shows including cast members and persists in storage 
        /// </summary>
        /// <param name="startPage">The page to start scraping from</param>
        /// <returns></returns>
        Task ScrapeAndStoreShows(int startPage);

        // <summary>
        /// Scrapes cast for the requested show
        /// </summary>
        /// <param name="showId">The show id</param>
        /// <returns></returns>
        Task<ICollection<TVmazePerson>> ScrapeCast(int showId);
    }
}