using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scraper.Common.TVmaze;
using Scraper.Data.Entities;

namespace Scraper.Repositories.IRepositories
{
    public interface IShowRepository
    {
        /// <summary>
        /// Returns a collection of paginated Shows
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The number of items to return per page</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns></returns>
        Task<ICollection<Show>> GetShows(int page, int pageSize, CancellationToken ct);

        /// <summary>
        /// Returns the ID of the Show added latest to the storage
        /// </summary>
        /// <returns></returns>
        Task<int> GetLastShowId();

        /// <summary>
        /// Saves the Shows in the storage
        /// </summary>
        /// <param name="shows">The Show including Cast to be saved</param>
        /// <returns></returns>
        Task StoreShows(ICollection<TVmazeShow> shows);
    }
}