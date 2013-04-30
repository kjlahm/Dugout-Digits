using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DugoutDigits.Objects {

    public enum RequestType {
        JOIN_TEAM,
        COACH_PERMISSION
    }

    public class Request {
        public RequestType type { get; set; }
        public Person requestee { get; set; }
        public Team team { get; set; }
        public long ID { get; set; }
        public DateTime timestamp { get; set; }

        public Request(RequestType newType, Person newRequestee, Team newTeam) {
            type = newType;
            requestee = newRequestee;
            team = newTeam;
        }
    }
}