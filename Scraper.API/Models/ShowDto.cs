﻿using System.Collections.Generic;

namespace Scraper.Api.Models
{
    public class ShowDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<PersonDto> Cast { get; set; }
    }
}
