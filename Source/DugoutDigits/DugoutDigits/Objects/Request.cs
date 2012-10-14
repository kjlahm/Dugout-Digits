using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DugoutDigits.Objects {
    public class Request {
        public Person requestee { get; set; }
        public Team team { get; set; }
        public long ID { get; set; }

        public Request(Person newRequestee, Team newTeam) {
            requestee = newRequestee;
            team = newTeam;
        }
    }
}