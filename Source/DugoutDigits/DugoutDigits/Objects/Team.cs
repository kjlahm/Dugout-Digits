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
        public Person coach { get; set; }   // The coach of the team.
        public long ID { get; set; }    // The ID of the team as found in the DB.

        private List<Person> players;   // The players on the team.

        public Team(String newName, Person newCoach)
        {
            name = newName;
            ID = 0;
            coach = newCoach;
            wins = 0;
            loses = 0;
            ties = 0;
            players = new List<Person>();
        }

        public Team() : this("default team name", new Person()) { }

        /// <summary>
        /// Adds the given player to the team's list of players.
        /// </summary>
        /// <param name="newPlayer">The new player to be added to the team.</param>
        public void addPlayer(Person newPlayer)
        {
            players.Add(newPlayer);
        }

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