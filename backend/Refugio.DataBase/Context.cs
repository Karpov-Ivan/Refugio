using System;
using Microsoft.EntityFrameworkCore;
using Refugio.DataBase.Models.Models;

namespace Refugio.DataBase
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
            Database.Migrate();
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Group> Groups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Refugio");

            modelBuilder.Entity<User>().HasKey(x => x.Id);

            modelBuilder.Entity<Group>().HasKey(z => z.Id);

            modelBuilder.Entity<User>()
                        .HasMany(p => p.Groups)
                        .WithMany(c => c.Users)
                        .UsingEntity(j => j.ToTable("Connection"));

            base.OnModelCreating(modelBuilder);
        }
    }
}