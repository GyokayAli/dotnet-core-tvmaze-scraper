using System.Collections.Generic;

namespace Scraper.Common.TVmaze
{
    public class TVmazeShow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<TVmazePerson> Cast { get; set; }
    }
}
