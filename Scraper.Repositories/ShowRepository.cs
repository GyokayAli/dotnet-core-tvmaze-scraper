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

namespace Scraper.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly ILogger<ShowRepository> _logger;
        private readonly ApplicationDbContext _dbContext;

        public ShowRepository(ILogger<ShowRepository> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
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
                            await StoreCastAsync(newShow, show.Cast);
                    }
                }
                catch (Exception ex)
                {
                    string message = $"Something went wrong while trying to store Show {show.Id}.";

                    _logger.LogError(ex, message);
                    throw new Exception(message, ex);
                }
            }
        }

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
                string message = $"Something went wrong while trying to store Show {show.Id}.";

                _logger.LogError(ex, message);
                throw new Exception(message, ex);
            }
        }

        /// <summary>
        /// Saves the Person in the storage
        /// </summary>
        /// <param name="person">The Person to be stored</param>
        /// <returns></returns>
        private async Task<Person> AddPersonAsync(TVmazePerson person)
        {
            try
            {
                var newPerson = new Person()
                {
                    Id = person.Id,
                    Name = person.Name,
                    Birthday = person?.Birthday
                };

                await _dbContext.People.AddAsync(newPerson);
                await _dbContext.SaveChangesAsync();

                return newPerson;
            }
            catch (Exception ex)
            {
                string message = $"Something went wrong while tring to store Person ({person.Id}).";

                _logger.LogError(ex, message);
                throw new Exception(message, ex);
            }
        }

        /// <summary>
        /// Stores the Cast of a Show in the storage
        /// </summary>
        /// <param name="show">The Show entity</param>
        /// <param name="cast">The collection of Cast</param>
        /// <returns></returns>
        private async Task StoreCastAsync(Show show, ICollection<TVmazePerson> cast)
        {
            foreach (var person in cast)
            {
                var storedPerson = await _dbContext.People.FirstOrDefaultAsync(x => x.Id == person.Id);
                if (storedPerson == null)
                {
                    var newPerson = await AddPersonAsync(person);
                    await HandleEntityRelations(newPerson, show);
                }
                // check if there is already a relation between show and person
                else if (!_dbContext.PeopleShows.Any(x => x.ShowId == show.Id && x.PersonId == storedPerson.Id))
                {
                    await HandleEntityRelations(storedPerson, show);
                }
            }
        }

        /// <summary>
        /// Stores the relationship between a Show and Cast member
        /// </summary>
        /// <param name="person">The Person entity</param>
        /// <param name="show">The Show entity</param>
        /// <returns></returns>
        private async Task HandleEntityRelations(Person person, Show show)
        {
            try
            {
                var relation = new PersonShow()
                {
                    Person = person,
                    PersonId = person.Id,
                    Show = show,
                    ShowId = show.Id
                };

                await _dbContext.PeopleShows.AddAsync(relation);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string message = $"Something went wrong while tring to update the Person ({person.Id}) - Show ({show.Id}) relation.";

                _logger.LogError(ex, message);
                throw new Exception(message, ex);
            }
        }
    }
}