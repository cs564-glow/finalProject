# Instructions for Grading of Final Submission (Windows only)
## How to Run the Website
1. Extract the final submission archive.
2. Navigate to %AppData% (ex: C:\Users\Garrett\AppData\Roaming).
3. Create a subfolder `cs564proj` (ex: C:\Users\Garrett\AppData\Roaming\cs564proj).
4. Copy/paste the provided movie.db to this `cs564proj` folder.
5. From a PowerShell or CMD terminal, `cd` to the LetterBoxDClone directory (ex: .\finalProject\cs564proj\LetterBoxDClone)
6. Run `dotnet run`
7. You should see a message like:
```
Building...
info: Microsoft.Hosting.Lifetime[0]
     Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```
8. Navigate to the address in a browser

You are now in the LetterBoxDClone!

Reference: https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-run

## How to Run the Importer to Create a Database from Scratch
1. Extract the final submission archive
2. Download the [HetRec MovieLens dataset](https://files.grouplens.org/datasets/hetrec2011/hetrec2011-movielens-2k-v2.zip) or use the copy included with the final submission (hetrec2011-movielens-2k-v2 folder)
3. From a PowerShell or CMD terminal, run Importer.exe
4. `Importer -i <path to dataset> [-x]` NOTE: `-x` flag will delete any existing movie.db database at the output location
   1. Example: `Importer -i "W:\source\repos\finalProject\testFiles\hetrec2011-movielens-2k-v2" -x`
   2. Running this command will create the SQLite database at %AppData%\cs564proj\movie.db
   3. Specify a custom output location by using the optional -d flag `[-d] <path to db>`

A fresh database is created!


# Final
## Basic Search
![alt text](https://github.com/cs564-glow/finalProject/blob/main/BasicSearch1.gif "Basic Search")
## Advanced Search, Tag and Genre Navigation
![alt text](https://github.com/cs564-glow/finalProject/blob/main/AdvancedSearch1.gif "Advanced Search")
## Movie Detail
![alt text](https://github.com/cs564-glow/finalProject/blob/main/MovieDetail.gif "Movie Detail")
## User Detail and Editing
![alt text](https://github.com/cs564-glow/finalProject/blob/main/UserDetailAndEdit.gif "User Detail and Editing")
## Cast and Crew Detail
![alt text](https://github.com/cs564-glow/finalProject/blob/main/CastAndCrew.gif "Cast and Crew Detail")
