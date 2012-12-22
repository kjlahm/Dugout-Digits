using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DugoutDigits.Objects {
    /// <summary>
    /// Object to hold information about a single game.
    /// </summary>
    public class Practice {
        public Season season { get; set; }    // The season associated with the game.
        public DateTime date { get; set; }    // The date and start time of the game.
        public String location { get; set; }    // The location of the game (maybe a better way to store this).

        private List<Inning> innings;           // The innings associated with this game.

        public Practice(Season newSeason, DateTime newDate, String newLocation) {
            season = newSeason;
            date = newDate;
            location = newLocation;
            innings = new List<Inning>();
        }

        public Practice() : this(new Season(), new DateTime(), "default practice location") { }
    }
}