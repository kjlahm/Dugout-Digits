using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DugoutDigits.Objects
{
    /// <summary>
    /// Object to hold information about an inning.
    /// </summary>
    public class Inning
    {
        HalfInning topHalf { get; set; }   // The top (away) half of the inning.
        HalfInning bottomHalf { get; set; }   // The bottom (home) half of the inning.

        public Inning(HalfInning top, HalfInning bottom)
        {
            topHalf = top;
            bottomHalf = bottom;
        }

        public Inning() : this(new HalfInning(), new HalfInning()) { }
    }
}