using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scraper.Data;
using Scraper.Repositories.IRepositories;
using Scraper.Data.Entities;
using Scraper.Common.TVmaze;
using Scraper.Common;
using EFCore.BulkExtensions;
using AutoMapper;

namespace Scraper.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly ILogger<ShowRepository> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ShowRepository(ILogger<ShowRepository> logger, ApplicationDbContext dbContext, IMapper mapper)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns a collection of paginated Shows
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The number of items to return per page</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns></returns>
        public async Task<ICollection<Show>> GetShows(int page, int pageSize, CancellationToken ct)
        {
            return await _dbContext.Shows
                .Include(s => s.PeopleShows).ThenInclude(ps => ps.Person)
                .OrderBy(s => s.Id)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        /// <summary>
        /// Returns the ID of the Show added latest to the storage
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetLastShowId()
        {
            var lastShow = await _dbContext.Shows.LastOrDefaultAsync();
            return lastShow?.Id ?? 0;
        }

        /// <summary>
        /// Saves the Shows in the storage
        /// </summary>
        /// <param name="shows">The Shows including Cast to be saved</param>
        /// <returns></returns>
        public async Task StoreShows(ICollection<TVmazeShow> shows)
        {
            foreach (var show in shows)
            {
                try
                {
                    var storedShow = await _dbContext.Shows.AnyAsync(x => x.Id == show.Id);
                    if (!storedShow)
                    {
                        var newShow = await AddShowAsync(show);
                        if (show.Cast != null)
                        {
                            // Get rid off duplicate actors
                            List<TVmazePerson> uniqueCast = show.Cast.Distinct(new PersonEqualityComparer()).ToList();
                            await StoreCastAsync(newShow, uniqueCast);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Something went wrong while trying to store Show {show.Id}.");
                }
            }
        }

        #region "Helpers"

        /// <summary>
        /// Saves the Show in the storage
        /// </summary>
        /// <param name="show">The Show including Cast to be stored</param>
        /// <returns></returns>
        private async Task<Show> AddShowAsync(TVmazeShow show)
        {
            try
            {
                // Create a fresh object to prevent issues 
                // with another instance with the same key value already being tracked.
                var newShow = new Show()
                {
                    Id = show.Id,
                    Name = show.Name
                };

                await _dbContext.Shows.AddAsync(newShow);
                await _dbContext.SaveChangesAsync();

                return newShow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong while trying to store Show {show.Id}.");
                throw;
            }
        }

        /// <summary>
        /// Stores the Cast of a Show in the storage
        /// </summary>
        /// <param name="show">The Show entity</param>
        /// <param name="cast">The collection of Cast</param>
        /// <returns></returns>
        private async Task StoreCastAsync(Show show, List<TVmazePerson> cast)
        {
            try
            {
                // Make cast data ready for upsert
                var normalizedCast = _mapper.Map<List<TVmazePerson>, List<Person>>(cast);
                // Bulk upsert
                await _dbContext.BulkInsertOrUpdateAsync<Person>(normalizedCast);

                await HandleEntityRelations(show, normalizedCast);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong while trying to store the cast for Show ({show.Id}).");
            }
        }

        /// <summary>
        /// Stores the relationship between a Show and Cast member
        /// </summary>
        /// <param name="show">The Show entity</param>
        /// <param name="cast">The collection of cast for Show</param>
        /// <returns></returns>
        private async Task HandleEntityRelations(Show show, List<Person> cast)
        {
            try
            {
                //Make show/ cast relations data ready for upsert
                var normalizedRelations = cast.Select(x => new PersonShow
                {
                    Person = x,
                    PersonId = x.Id,
                    Show = show,
                    ShowId = show.Id
                }).ToList();
                // Bulk upsert
                await _dbContext.BulkInsertOrUpdateAsync<PersonShow>(normalizedRelations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong while trying to store the Show/Cast relation - Show ({show.Id}).");
            }
        }
        #endregion
    }
}