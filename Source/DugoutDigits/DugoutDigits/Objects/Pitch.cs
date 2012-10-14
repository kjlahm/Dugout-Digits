using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DugoutDigits.Objects
{
    /// <summary>
    /// Object to hold information about a single pitch.
    /// </summary>
    public class Pitch
    {
        Person pitcher { get; set; }   // The pitcher
        Person batter { get; set; }   // The batter
        short pitchType { get; set; }   // The type of pitch (see List in App Constants)
        int speed { get; set; }   // The speed of the pitch
        int result { get; set; }   // The result of the pitch (see List in App Constants)

        public Pitch(Person newPitcher, Person newBatter, short newPitchType, short newSpeed, short newResult)
        {
            pitcher = newPitcher;
            batter = newBatter;
            pitchType = newPitchType;
            speed = newSpeed;
            result = newResult;
        }
    }
}