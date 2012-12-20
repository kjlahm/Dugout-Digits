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
        public String logoURL { get; set; }
        public short wins { get; set; }   // The number of wins the team has.
        public short loses { get; set; }   // The number of loses the team has.
        public short ties { get; set; }   // The number of ties the team has.
        public List<Person> coaches { get; set; }   // The coach of the team.
        public List<Person> players { get; set; }   // The players on the team.
        public long ID { get; set; }    // The ID of the team as found in the DB.

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

        public Team(String newName, String newLogoUrl, List<Person> newCoaches)
        {
            name = newName;
            logoURL = newLogoUrl;
            ID = 0;
            coaches = newCoaches;
            wins = 0;
            loses = 0;
            ties = 0;
            players = new List<Person>();
        }

        public Team() : this("default team name", "http://i.imgur.com/wvqlV.jpg", new List<Person>()) { }

        /// <summary>
        /// Returns the number of players on the team.
        /// </summary>
        /// <returns>The number of players on the team.</returns>
        public int getTeamSize()
        {
            return players.Count;
        }
    }

    // Custom comparer for the Team class 
    class TeamComparer : IEqualityComparer<Team> {
        // Products are equal if their names and product numbers are equal. 
        public bool Equals(Team x, Team y) {

            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal. 
            return x.ID == y.ID;
        }

        // If Equals() returns true for a pair of objects  
        // then GetHashCode() must return the same value for these objects. 

        public int GetHashCode(Team team) {
            //Check whether the object is null 
            if (Object.ReferenceEquals(team, null)) return 0;

            //Get hash code for the email field if it is not null. 
            int hashTeamID = team.ID == null ? 0 : team.ID.GetHashCode();

            //Calculate the hash code for the ID. 
            return hashTeamID;
        }
    }
}