﻿using System;
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
        public string opponent { get; set; }    // The home team for the game.
        public bool isHome { get; set; }
        public Season season { get; set; }    // The season associated with the game.
        public DateTime date { get; set; }    // The date and start time of the game.
        public String location { get; set; }    // The location of the game (maybe a better way to store this).

        private List<Inning> innings;           // The innings associated with this game.

        public Game(string newOpponent, bool newIsHome, Season newSeason, DateTime newDate, String newLocation)
        {
            opponent = newOpponent;
            isHome = newIsHome;
            season = newSeason;
            date = newDate;
            location = newLocation;
            innings = new List<Inning>();
        }

        public Game() : this("default opponent", true, new Season(), new DateTime(), "default game location") { }
    }
}