﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Scraper.Data;

namespace Scraper.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Scraper.Data.Entities.Person", b =>
                {
                    b.Property<int>("Id");

                    b.Property<DateTime?>("Birthday");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("People");
                });

            modelBuilder.Entity("Scraper.Data.Entities.PersonShow", b =>
                {
                    b.Property<int>("PersonId");

                    b.Property<int>("ShowId");

                    b.HasKey("PersonId", "ShowId");

                    b.HasIndex("ShowId");

                    b.ToTable("PeopleShows");
                });

            modelBuilder.Entity("Scraper.Data.Entities.Show", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Shows");
                });

            modelBuilder.Entity("Scraper.Data.Entities.PersonShow", b =>
                {
                    b.HasOne("Scraper.Data.Entities.Person", "Person")
                        .WithMany("PeopleShows")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Scraper.Data.Entities.Show", "Show")
                        .WithMany("PeopleShows")
                        .HasForeignKey("ShowId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
