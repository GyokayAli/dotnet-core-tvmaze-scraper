namespace Scraper.Data.Entities
{
    public class PersonShow
    {
        public int PersonId { get; set; }
        public int ShowId { get; set; }

        public Person Person { get; set; }
        public Show Show { get; set; }
    }
}