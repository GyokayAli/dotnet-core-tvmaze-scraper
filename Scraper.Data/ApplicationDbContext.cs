using Microsoft.EntityFrameworkCore;
using Scraper.Data.Entities;

namespace Scraper.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        public DbSet<Show> Shows { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<PersonShow> PeopleShows { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PersonShow>(x => x.HasKey(ps => new { ps.PersonId, ps.ShowId }));

            builder.Entity<PersonShow>()
                .HasOne(ps => ps.Person)
                .WithMany(p => p.PeopleShows)
                .HasForeignKey(ps => ps.PersonId);

            builder.Entity<PersonShow>()
                .HasOne(ps => ps.Show)
                .WithMany(s => s.PeopleShows)
                .HasForeignKey(ps => ps.ShowId);
        }
    }
}