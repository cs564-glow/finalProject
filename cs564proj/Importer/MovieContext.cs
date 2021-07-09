using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Importer
{
    public class MovieContext : DbContext
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

        public DbSet<Movie> Movies { get; set; }
    }
}
