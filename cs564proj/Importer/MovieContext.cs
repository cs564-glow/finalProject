using Microsoft.EntityFrameworkCore;

namespace Importer
{
    class MovieContext : DbContext
    {
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
        // https://stackoverflow.com/a/49594698
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MovieGenre>()
                .HasKey(m => new { m.MovieId, m.GenreId });
        }

        public DbSet<Movie> Movie { get; set; }
        public DbSet<Genre> Genre { get; set; }
        public DbSet<MovieGenre> MovieGenre { get; set; }

    }
}
