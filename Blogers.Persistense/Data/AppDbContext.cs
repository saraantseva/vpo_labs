using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.Persistense.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
            Database.EnsureCreated();
        }

        public DbSet<Bloger> Blogers { get; set; }
        public DbSet<Story> Stories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bloger>().
                OwnsOne<Person>(t => t.PersonalData); // дабы не создавать таблицу персона

            modelBuilder.Entity<Bloger>()
                .HasIndex(b => b.Nickname).IsUnique(); // для ускорения поиска по нику
            
        }

    }
}
