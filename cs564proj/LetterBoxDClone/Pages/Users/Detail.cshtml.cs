using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataLibrary;
using LetterBoxDClone.Pages.Shared;
using static LetterBoxDClone.Pages.Shared.Connection;
using Microsoft.Data.Sqlite;


namespace LetterBoxDClone.Pages
{
    public class UserModel : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public string UserId { get; set; }
        public User User { get; set; }
        public List<SeenMovieData> moviesSeen { get; set; }
        public List<MightLikeMovieData> moviesMightLike { get; set; }
        public void OnGet()
        {
            User = GetSingleUserByKey(UserId);
            moviesSeen = GetMoviesSeen(UserId);
            moviesMightLike = GetMoviesMightLike(UserId);
        }

        public static User GetSingleUserByKey(string UserId)
        {
            string query =
                $@"
                 SELECT *
                 FROM User
                 WHERE UserId = {UserId}
                 ";
            User User = Connection.GetSingleRow<User>(query, GetUserDataFromReader);

            return User;
        }

        public static User GetUserDataFromReader(SqliteDataReader reader)
        {
            User user = new User(reader.GetInt64(0));

            user.Username = reader.GetString(1);
            user.Password = reader.GetString(2);

            return user;
        }

        public static List<SeenMovieData> GetMoviesSeen(string UserId)
        {
            string query =
                $@"
                 SELECT m.movieId, m.Title, m.Year, ur.Rating
                 FROM Movie AS m NATURAL JOIN UserRating AS ur
                 WHERE ur.UserId = {UserId}
                 ORDER BY ur.Timestamp DESC
                 ";
            List<SeenMovieData> movieList = Connection.GetMultipleRows(query, GetSeenMoviesDataFromReader);
            // PaginatedList<SeenMovieData> paginatedMovieList = new PaginatedList<SeenMovieData>(movieList, movieList.Count, 1, 10);
            return movieList;
        }

        public static SeenMovieData GetSeenMoviesDataFromReader(SqliteDataReader reader)
        {
            SeenMovieData smd = new SeenMovieData();

            Movie movie = new Movie();
            movie.MovieId = reader.GetInt32(0);
            movie.Title = reader.GetString(1);
            movie.Year = reader.GetString(2);
            smd.movie = movie;

            UserRating ur = new UserRating();
            ur.Rating = reader.GetDouble(3);
            smd.rating = ur;

            return smd;
        }

        public static List<MightLikeMovieData> GetMoviesMightLike(string UserId)
        {
            string query =
                $@"
                SELECT DISTINCT m1.movieId, m1.Title, m1.Year, d1.CastCrewId, cc1.Name
                FROM Movie AS m1
                NATURAL JOIN Directs AS d1
                NATURAL JOIN CastCrew AS cc1
                NATURAL JOIN MovieGenre AS mg1
                WHERE m1.RtAllCriticsRating > 7.5 AND mg1.GenreId IN (SELECT GenreId
                                                                      FROM UserRating AS ur2 
                                                                      NATURAL JOIN MovieGenre AS mg2
                                                                      WHERE ur2.UserId = {UserId}
                                                                      GROUP BY mg2.GenreId
                                                                      HAVING AVG(ur2.Rating) > (SELECT AVG(ur3.Rating)
                                                                                                FROM UserRating AS ur3 
                                                                                                NATURAL JOIN Movie AS m3
                                                                                                WHERE ur3.UserId = {UserId})) 
                AND m1.MovieId NOT IN (SELECT m4.MovieId
                                       FROM Movie AS m4 
                                       NATURAL JOIN UserRating AS ur4
                                       WHERE ur4.UserId = {UserId})
                ORDER BY m1.RtAllCriticsRating DESC
                LIMIT 5";

            List<MightLikeMovieData> mightLikeList = GetMultipleRows(query, GetMoviesMightLikeFromReader);
            return mightLikeList;
        }

        public static MightLikeMovieData GetMoviesMightLikeFromReader(SqliteDataReader reader)
        {
            Movie movie = new Movie();
            movie.MovieId = reader.GetInt32(0);
            movie.Title = reader.GetString(1);
            movie.Year = reader.GetString(2);
            CastCrew director = new CastCrew(reader.GetString(3), reader.GetString(4));

            MightLikeMovieData mightLike = new MightLikeMovieData();
            mightLike.movie = movie;
            mightLike.director = director;
            return mightLike;
        }
    }
}
