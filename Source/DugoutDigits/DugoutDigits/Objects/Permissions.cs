using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DugoutDigits.Objects
{
    public class Permissions
    {
        public Boolean coachEnabled { get; set; }
        public Boolean siteAdmin { get; set; }

        public Permissions(bool coach, bool admin)
        {
            coachEnabled = coach;
            siteAdmin = admin;
        }

        public Permissions() : this(false, false) { }
    }
}