using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DugoutDigits.Objects
{
    /// <summary>
    /// Object to hold information about a half inning.
    /// </summary>
    public class HalfInning
    {
        short runs { get; set; }   // The runs scored in the half inning.
        short hits { get; set; }   // The number of hits in the half inning.
        short errors { get; set; }   // The number of errors in the half inning.

        private List<AtBat> atBats;

        public HalfInning(short newRuns, short newHits, short newErrors)
        {
            runs = newRuns;
            hits = newHits;
            errors = newErrors;
            atBats = new List<AtBat>();
        }

        public HalfInning() : this(-1, -1, -1) { }

        /// <summary>
        /// Adds the given AB to the list of at bats for the half inning.
        /// </summary>
        /// <param name="AB"></param>
        public void addAtBat(AtBat AB)
        {
            atBats.Add(AB);
        }
    }
}