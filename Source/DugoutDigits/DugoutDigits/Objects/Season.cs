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
        Team team { get; set; }   // The team associated with the season.
        short year { get; set; }   // The year of the season.

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
}