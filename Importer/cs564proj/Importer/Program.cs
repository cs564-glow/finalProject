using System;
using FileHelpers;

namespace Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new FileHelperEngine<Movie>();
            var movies = engine.ReadFile("W:\\source\\testFiles\\cs564\\ml-latest-small\\movies.csv");
            foreach (var movie in movies)
            {
                Console.Write(movie.Id + ", " + movie.Title + ", ");
                foreach (string genre in movie.GenreArray)
                {
                    Console.Write(genre + ", ");
                }
                Console.WriteLine();
            }
        }
    }
}
