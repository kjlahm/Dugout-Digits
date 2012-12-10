using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DugoutDigits.Objects
{
    public class Permissions
    {
        public Boolean coachEnabled { get; set; }

        public Permissions(bool coach)
        {
            coachEnabled = coach;
        }

        public Permissions() : this(false) { }
    }
}