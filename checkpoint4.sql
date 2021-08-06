

/* Query to get movie id, movie title, movie release year and rating that a given user has rated ordered from the most recently rated to least recently rated */
SELECT m.movieId,
  m.Title,
  m.Year,
  ur.Rating
FROM Movie AS m
  NATURAL JOIN UserRating AS ur
WHERE ur.UserId = { UserId }
ORDER BY ur.Timestamp DESC;
/* Query to get the movie Id, movie title, movie release year, director id and director name of movies that a given user may be interested in. Potential interest is determined based on genres of movies that they rated higher than others on average and were highly rated by critics on rotten tomatoes. The query also excludes any movies that are previously rated by the user, results are sorted by descending rotten tomatoes critic rating, results limited to 5 */
SELECT DISTINCT m1.movieId,
  m1.Title,
  m1.Year,
  d1.CastCrewId,
  cc1.Name
FROM Movie AS m1
  NATURAL JOIN Directs AS d1
  NATURAL JOIN CastCrew AS cc1
  NATURAL JOIN MovieGenre AS mg1
WHERE m1.RtAllCriticsRating > 7.5
  AND mg1.GenreId IN (
    SELECT GenreId
    FROM UserRating AS ur2
      NATURAL JOIN MovieGenre AS mg2
    WHERE ur2.UserId = { UserId }
    GROUP BY mg2.GenreId
    HAVING AVG(ur2.Rating) >= (
        SELECT AVG(ur3.Rating)
        FROM UserRating AS ur3
          NATURAL JOIN Movie AS m3
        WHERE ur3.UserId = { UserId }
      )
  )
  AND m1.MovieId NOT IN (
    SELECT m4.MovieId
    FROM Movie AS m4
      NATURAL JOIN UserRating AS ur4
    WHERE ur4.UserId = { UserId }
  )
ORDER BY m1.RtAllCriticsRating DESC
LIMIT 5;
/* Query to get movie id, movie title, movie release year, director id, and cast and crew name of movies that were highly rated on rotten tomatoes by critics in descending order by the critic rating, limited to 5 results (this is for making recommendations to users that have no movies watched yet */
SELECT m.MovieId,
  m.Title,
  m.Year,
  d.CastCrewId,
  cc.Name
FROM Movie as m
  NATURAL JOIN Directs as d
  NATURAL JOIN CastCrew as cc
ORDER BY m.RtAllCriticsRating DESC
LIMIT 5;
/* Query to get all the movie data for a given movie id */
SELECT *
FROM Movie
WHERE MovieId = { MovieId };
/* Query to get the director id and name information for a given movie */
SELECT cc.CastCrewId,
  cc.Name
FROM Directs AS d
  NATURAL JOIN CastCrew AS cc
WHERE d.MovieId = { MovieId };
/* Query to get all the ids and names of the actors in a movie */
SELECT cc.CastCrewId,
  cc.Name
FROM ActsIn AS ai
  NATURAL JOIN CastCrew AS cc
WHERE ai.MovieId = { MovieId }
ORDER BY ai.Billing;
/* Query to get the five tag Ids and tag names most applied to a given movie by users, ordered by the number of times a tag was applied by the users in descending order */
SELECT t.TagId,
  t.Name
FROM UserTag AS ut
  NATURAL JOIN Tag AS t
WHERE ut.MovieId = { MovieId }
GROUP BY ut.TagId
ORDER BY count(*) DESC
LIMIT 5;
/* Query to get 10 movies with highest average rating. Only count movies that have more than 10 ratings.  */
SELECT m1.MovieId,
  m1.Title,
  m1.Year,
  m1.CountryId,
  m1.ImdbId,
  m1.RtId,
  m1.RtAllCriticsRating,
  m1.RtAllCriticsNumReviews
FROM Movie AS m1
  NATURAL JOIN UserRating AS ur
GROUP BY ur.MovieId
HAVING count(*) > 10
ORDER BY AVG(Rating) DESC
LIMIT 10
  /* Query to get movie id, movie title, movie release year, movie IMDB ID, and movie Rotten Tomatoes ID for movies that are similar to a given movie based on similarly applied tags, makes sure it doesn't return the same movie, limits results to 5 movies */
SELECT m1.MovieId,
  m1.Title,
  m1.Year,
  m1.Year,
  m1.ImdbId,
  m1.RtId
FROM Movie as m1
  NATURAL JOIN (
    SELECT ut.MovieID
    FROM UserTag AS ut
    WHERE ut.TagId IN (
        SELECT t1.TagId
        FROM UserTag AS ut1
          NATURAL JOIN Tag as t1
        WHERE ut1.MovieId = { MovieId }
        GROUP BY ut1.TagId
        ORDER BY count(*) DESC
        LIMIT 5
      )
    GROUP BY ut.MovieId,
      ut.TagID
    ORDER BY count(*) DESC
    LIMIT 5
  ) similarMovies
WHERE m1.MovieId <> { MovieId };
/* Finds top 10 actors with at least 3 movies, who have highest average movie ratings. We only count movies that have more than 10 user ratings. */
SELECT cc.CastCrewId,
  cc.Name,
  AVG(avgRating)
FROM ActsIn AS ai
  NATURAL JOIN (
    SELECT ur.MovieId,
      AVG(ur.Rating) avgRating
    FROM UserRating AS ur
    GROUP BY (ur.MovieId)
    HAVING count(*) > 10
  ) AS amr
  NATURAL JOIN CastCrew AS cc
GROUP BY cc.CastCrewId
HAVING count(*) > 3
ORDER BY AVG(avgRating) DESC
LIMIT 10;