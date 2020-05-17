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
    }
}