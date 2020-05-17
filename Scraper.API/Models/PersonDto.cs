using System;
using Newtonsoft.Json;

namespace Scraper.API.Models
{
    public class PersonDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public DateTime? Birthday { get; set; }

        [JsonProperty("birthday")]
        public string BirthdayString
        {
            get
            {
                return Birthday?.ToString("yyyy-MM-dd");
            }
        }
    }
}