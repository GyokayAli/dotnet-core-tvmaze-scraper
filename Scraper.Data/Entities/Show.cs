﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scraper.Data.Entities
{
    public class Show
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<PersonShow> PeopleShows { get; set; }
    }
}
