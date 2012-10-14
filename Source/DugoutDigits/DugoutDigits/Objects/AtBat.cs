using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DugoutDigits.Objects
{
    /// <summary>
    /// Object to hold information about an at bat. This is simply a list 
    /// of pitches. The intention of this class is to eliminate a two dimensional
    /// array of pitches inside the half inning class.
    /// </summary>
    public class AtBat
    {
        private List<Pitch> pitches;    // The pitches of the at bat.

        private AtBat()
        {
            pitches = new List<Pitch>();
        }

        /// <summary>
        /// Adds the given pitch to the list of pitches for this at bat.
        /// </summary>
        /// <param name="pitch">The pitch to be added.</param>
        public void addPitch(Pitch pitch)
        {
            pitches.Add(pitch);
        }

        /// <summary>
        /// Returns the number of pitches associated with this at bat.
        /// </summary>
        /// <returns>The number of pitches in the at bat.</returns>
        public int pitchCount()
        {
            return pitches.Count;
        }
    }
}