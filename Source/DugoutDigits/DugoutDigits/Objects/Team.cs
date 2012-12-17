using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DugoutDigits.Objects
{
    /// <summary>
    /// Object used to hold a team's information.
    /// </summary>
    public class Team
    {
        public String name { get; set; }   // The team's name.
        public short wins { get; set; }   // The number of wins the team has.
        public short loses { get; set; }   // The number of loses the team has.
        public short ties { get; set; }   // The number of ties the team has.
        public List<Person> coaches { get; set; }   // The coach of the team.
        public long ID { get; set; }    // The ID of the team as found in the DB.

        private List<Person> players;   // The players on the team.

        public string CoachesDisplay {
            get {
                string coachList = "";
                if (coaches.Any()) {
                    coachList += coaches[0].firstName + " " + coaches[0].lastName;
                    for (int i = 1; i < coaches.Count; i++) {
                        coachList += ", " + coaches[i].firstName + " " + coaches[i].lastName;
                    }
                }
                return coachList;
            }
        }

        public Team(String newName, List<Person> newCoaches)
        {
            name = newName;
            ID = 0;
            coaches = newCoaches;
            wins = 0;
            loses = 0;
            ties = 0;
            players = new List<Person>();
        }

        public Team() : this("default team name", new List<Person>()) { }

        /// <summary>
        /// Returns the number of players on the team.
        /// </summary>
        /// <returns>The number of players on the team.</returns>
        public int getTeamSize()
        {
            return players.Count;
        }
    }
}