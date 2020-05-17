using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scraper.Repositories.IRepositories;
using Scraper.Data.Entities;
using Scraper.Services.IServices;

namespace Scraper.Services
{
    public class ShowService : IShowService
    {
        private readonly IShowRepository _showRepository;

        public ShowService(IShowRepository showRepository)
        {
            _showRepository = showRepository;
        }

        /// <summary>
        /// Returns paginated shows
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The number of items to return per page</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns></returns>
        public async Task<ICollection<Show>> GetShows(int page, int pageSize, CancellationToken ct = default(CancellationToken))
        {
            return await _showRepository.GetShows(page, pageSize, ct);
        }
    }
}