using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DugoutDigits.Objects
{
    /// <summary>
    /// Object to hold information about a single game.
    /// </summary>
    public class Game
    {
        Team awayTeam { get; set; }    // The away team for the game.
        Team homeTeam { get; set; }    // The home team for the game.
        Season season { get; set; }    // The season associated with the game.
        DateTime date { get; set; }    // The date and start time of the game.
        String location { get; set; }    // The location of the game (maybe a better way to store this).

        private List<Inning> innings;           // The innings associated with this game.

        public Game(Team away, Team home, Season newSeason, DateTime newDate, String newLocation)
        {
            awayTeam = away;
            homeTeam = home;
            season = newSeason;
            date = newDate;
            location = newLocation;
            innings = new List<Inning>();
        }

        public Game() : this(new Team(), new Team(), new Season(), new DateTime(), "default game location") { }

        /// <summary>
        /// Adds the given inning to the list of innings associated with this game.
        /// </summary>
        /// <param name="newInning">The inning to be added.</param>
        public void addInning(Inning newInning)
        {
            innings.Add(newInning);
        }
    }
}