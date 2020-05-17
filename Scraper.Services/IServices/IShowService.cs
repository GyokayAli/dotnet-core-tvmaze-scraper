using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scraper.Data.Entities;

namespace Scraper.Services.IServices
{
    public interface IShowService
    {
        /// <summary>
        /// Returns paginated shows
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The number of items to return per page</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns></returns>
        Task<ICollection<Show>> GetShows(int page, int pageSize, CancellationToken ct);
    }
}