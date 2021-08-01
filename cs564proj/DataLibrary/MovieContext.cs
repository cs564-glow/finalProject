using Microsoft.EntityFrameworkCore;
// ReSharper disable UnusedMember.Global

namespace DataLibrary
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
            // https://docs.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key#indirect-many-to-many-relationships
            modelBuilder.Entity<ActsIn>()
                .HasOne(acts => acts.CastCrew)
                .WithMany(a => a.ActingRoles)
                .HasForeignKey(acts => acts.CastCrewId);
            modelBuilder.Entity<ActsIn>()
                .HasOne(mv => mv.Movie)
                .WithMany(m => m.MovieActors)
                .HasForeignKey(mv => mv.MovieId);
            modelBuilder.Entity<Directs>()
                .HasOne(d => d.CastCrew)
                .WithMany(c => c.DirectingCredits)
                .HasForeignKey(d => d.CastCrewId);
            modelBuilder.Entity<Directs>()
                .HasOne(mv => mv.Movie)
                .WithMany(m => m.MovieDirectors)
                .HasForeignKey(mv => mv.MovieId);
            modelBuilder.Entity<UserRating>()
                .HasOne(u => u.User)
                .WithMany(m => m.UserRating)
                .HasForeignKey(u => u.UserId);
            modelBuilder.Entity<UserRating>()
                .HasOne(u => u.Movie)
                .WithMany(m => m.UserRating)
                .HasForeignKey(u => u.MovieId);
            modelBuilder.Entity<UserTag>()
                .HasOne(u => u.User)
                .WithMany(m => m.UserTag)
                .HasForeignKey(u => u.UserId);
            modelBuilder.Entity<UserTag>()
                .HasOne(u => u.Movie)
                .WithMany(m => m.UserTag)
                .HasForeignKey(u => u.MovieId);
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
        public DbSet<Country> Country { get; set; }
        public DbSet<FilmLocation> FilmLocation { get; set; }
    }
}