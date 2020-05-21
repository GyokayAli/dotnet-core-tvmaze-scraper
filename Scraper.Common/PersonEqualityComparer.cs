using Scraper.Common.TVmaze;
using System.Collections.Generic;

namespace Scraper.Common
{
    public class PersonEqualityComparer : IEqualityComparer<TVmazePerson>
    {
        public bool Equals(TVmazePerson x, TVmazePerson y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(TVmazePerson obj)
        {
            return obj.Id;
        }
    }
}