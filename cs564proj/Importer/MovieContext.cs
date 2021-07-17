using Microsoft.EntityFrameworkCore;
// ReSharper disable UnusedMember.Global

namespace Importer
{
    public class MovieContext : DbContext
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
            modelBuilder.Entity<UserTag>()
                .HasKey(m => new { m.UserId, m.MovieId, m.TagId });
            modelBuilder.Entity<UserRating>()
                .HasKey(m => new { m.UserId, m.MovieId });
            modelBuilder.Entity<Directs>()
                .HasKey(m => new { m.MovieId, m.CastCrewId });
            modelBuilder.Entity<ActsIn>()
                .HasKey(m => new { m.MovieId, m.CastCrewId });
        }

        public DbSet<Movie> Movie { get; set; }
        public DbSet<Genre> Genre { get; set; }
        public DbSet<MovieGenre> MovieGenre { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<UserTag> UserTag { get; set; }
        public DbSet<UserRating> UserRating { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Directs> Directs { get; set; }
        public DbSet<ActsIn> ActsIn { get; set; }
        public DbSet<CastCrew> CastCrew { get; set; }
    }
}