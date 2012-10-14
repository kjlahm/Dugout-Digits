using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DugoutDigits.Objects
{
    public class Position
    {
        int posID { get; set; }
        String abbreviation { get; set; }
        String description { get; set; }

        public Position(int newPosID, String newAbbr, String newDescr)
        {
            posID = newPosID;
            abbreviation = newAbbr;
            description = newDescr;
        }

        public Position() : this(-1, "?", "Default Position") { }
    }
}