using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DugoutDigits.Objects {
    public class Invitation {
        public long ID { get; set; }
        public Team team { get; set; }
        public String invitee { get; set; }
        public Person invitor { get; set; }
        public DateTime timestamp { get; set; }

        public Invitation(long newID, Team newTeam, Person newInvitor) {
            ID = newID;
            team = newTeam;
            invitor = newInvitor;
        }
    }
}