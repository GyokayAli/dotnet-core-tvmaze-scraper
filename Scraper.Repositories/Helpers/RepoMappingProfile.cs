using AutoMapper;
using Scraper.Common.TVmaze;
using Scraper.Data.Entities;

namespace Scraper.Repositories.Helpers
{
    public class RepoMappingProfile : Profile
    {
        public RepoMappingProfile()
        {
            CreateMap<TVmazePerson, Person>();
        }
    }
}
