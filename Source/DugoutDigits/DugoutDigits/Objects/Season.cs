using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DugoutDigits.Objects
{
    /// <summary>
    /// Object to hold a season's information.
    /// </summary>
    public class Season
    {
        public Team team { get; set; }   // The team associated with the season.
        public short year { get; set; }   // The year of the season.
        public long ID { get; set; }

        private List<Game> games;       // The games played by the associated team this season.

        public Season(Team newTeam, short newYear)
        {
            team = newTeam;
            year = newYear;
            games = new List<Game>();
        }

        public Season() : this(new Team(), 1923) { }

        /// <summary>
        /// Adds the given game to the list of games for the season.
        /// </summary>
        /// <param name="newGame">The game to be added.</param>
        public void addGame(Game newGame)
        {
            games.Add(newGame);
        }

        /// <summary>
        /// Gets the number of games associated with the season.
        /// </summary>
        /// <returns>The number of games saved to the season.</returns>
        public int getNumGames()
        {
            return games.Count;
        }
    }

    // Custom comparer for the Person class 
    class SeasonComparer : IEqualityComparer<Season> {
        // Seasons are equal if their years are equal. 
        public bool Equals(Season x, Season y) {

            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal. 
            return x.year == y.year;
        }

        // If Equals() returns true for a pair of objects  
        // then GetHashCode() must return the same value for these objects. 

        public int GetHashCode(Season season) {
            //Check whether the object is null 
            if (Object.ReferenceEquals(season, null)) return 0;

            //Get hash code for the year field if it is not null. 
            int hashSeasonYear = season.year == null ? 0 : season.year.GetHashCode();

            //Calculate the hash code for the year.
            return hashSeasonYear;
        }
    }
}