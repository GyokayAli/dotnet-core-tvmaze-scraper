﻿using AutoMapper;
using Scraper.API.Models;
using Scraper.Data.Entities;
using System.Linq;

namespace Scraper.API.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Show, ShowDto>()
                .ForMember(dest => dest.Cast,
                    opt => opt.MapFrom(src => src.PeopleShows.Select(x => x.Person)));
            CreateMap<Person, PersonDto>();
        }
    }
}