using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Importer
{
    class MovieContext : DbContext
    {
        private readonly string _connectionString;
        //public MovieContext()
        //{

        //}

        // https://docs.microsoft.com/en-us/ef/core/dbcontext-configuration/#simple-dbcontext-initialization-with-new
        public MovieContext(DbContextOptions<MovieContext> options)
        : base(options)
        {
        }

        //public MovieContext(string connectionString)
        //{
        //    _connectionString = connectionString;
        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite(_connectionString);
        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{ 
        //}
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //}

        // https://docs.microsoft.com/en-us/ef/core/modeling/keys?tabs=data-annotations#configuring-a-primary-key
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Genre>
        //}

        public DbSet<Movie> Movie { get; set; }
        public DbSet<Genre> Genre { get; set; }

    }
}
