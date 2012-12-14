using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DugoutDigits.Objects {
    public class Invitation {
        public long ID { get; set; }
        public Team team { get; set; }

        public Invitation(long newID, Team newTeam) {
            ID = newID;
            team = newTeam;
        }
    }
}