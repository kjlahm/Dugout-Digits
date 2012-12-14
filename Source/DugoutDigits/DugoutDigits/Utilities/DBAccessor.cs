﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using DugoutDigits.Objects;

namespace DugoutDigits.Utilities
{
    //http://community.discountasp.net/showthread.php?t=11948

    /// <summary>
    /// Object used to interact with the database. It's currently setup to use 
    /// my CSE MySQL database.
    /// </summary>
    public class DBAccessor
    {
        private MySqlConnection connection;
        private MySqlCommand command;

        public DBAccessor()
        {
            String conStr = "";
            conStr += "SERVER=" + AppConstants.MYSQL_SERVER + ";";
            conStr += "DATABASE=" + AppConstants.MYSQL_DB_NAME + ";";
            conStr += "UID=" + AppConstants.MYSQL_USERNAME + ";";
            conStr += "PASSWORD=" + AppConstants.MYSQL_PASSWORD;

            connection = new MySqlConnection(conStr);
            command = connection.CreateCommand();
        }

        public String TestConnection(String query)
        {
            MySqlDataReader dr = null;
            String result = "";

            try
            {
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();

                while (dr.Read())
                {
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        result += dr.GetValue(i).ToString() + ",";
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        /// <summary>
        /// This methods checks if the given email is found in the database.
        /// </summary>
        /// <param name="email">The email to look for.</param>
        /// <returns>If the email is found or not.</returns>
        public bool CheckEmail(String email) {
            MySqlDataReader dr = null;
            String query = "SELECT * FROM DD_Person WHERE email='" + email + "'";
            bool available = false;

            try {
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();
                available = !dr.HasRows;
            }
            catch (Exception ex) {
            }
            finally {
                connection.Close();
            }

            return available;
        }

        /// <summary>
        /// Adds the given user to the database. This only adds base (Person) information 
        /// to the database and sets the player type flag.
        /// </summary>
        /// <param name="newUser">The user to be added to the database.</param>
        /// <param name="playerType">The player type.</param>
        /// <returns>If the adition to the database was successful.</returns>
        public bool AddNewUser(Person newUser) {
            String query = "INSERT INTO ";
            query += AppConstants.MYSQL_TABLE_PERSON;
            query += " (firstName, lastName, email, password) VALUES (";
            query += "'" + newUser.firstName + "', ";
            query += "'" + newUser.lastName + "', ";
            query += "'" + newUser.email + "', ";
            query += "'" + newUser.getPassword() + "')";
            bool addUser = ExecuteInsert(query);

            query = "INSERT INTO ";
            query += AppConstants.MYSQL_TABLE_PERMISSIONS;
            query += " (personID) VALUES (";
            query += GetPersonID(newUser.email) + ")";
            return addUser && ExecuteInsert(query);
        }

        /// <summary>
        /// Method used to insert data into a table. This method is useful when no 
        /// response is needed from the database.
        /// </summary>
        /// <param name="query">The query to be executed.</param>
        /// <returns>The success of the attempted query.</returns>
        private bool ExecuteInsert(String query)
        {
            MySqlDataReader dr = null;
            bool success = true;

            try
            {
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                success = false;
            }
            finally
            {
                connection.Close();
            }
            return success;
        }

        /// <summary>
        /// Checks if the given email/password combo is in the database. There are four 
        /// possible resulting messages. If an error occurs during the process a simple 
        /// "error" is returned. If multiple entries are found for the email "email 
        /// mismatch" is returned. Assuming a single match is found for the email, if 
        /// the password doesn't match "password mismatch" is returned. If the email 
        /// and password both match the person type is returned.
        /// </summary>
        /// <param name="email">The email to look up.</param>
        /// <param name="password">The password to match to the email.</param>
        /// <returns>The resulting message after an attempted lookup.</returns>
        public String CheckLoginCredentials(String email, String password) {
            MySqlDataReader dr = null;
            String returnVal = "error";

            String encryptedPass = PasswordEncryptor.encrypt_password(password, AppConstants.PASSWORD_KEY);

            String query = "SELECT password, firstName, lastName FROM ";
            query += AppConstants.MYSQL_TABLE_PERSON;
            query += " WHERE email='" + email + "'";

            try {
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();

                int resultCount = 0;
                String matchPass = "";
                while (dr.Read()) {
                    returnVal = dr["firstName"] + " " + dr["lastName"];
                    matchPass = dr["password"] +"";
                    resultCount++;
                }

                if (resultCount == 1) {
                    if (matchPass.Equals(encryptedPass)) {
                        returnVal = "success|" + returnVal;
                    }
                    else {
                        returnVal = "password mismatch";
                    }
                }
                else {
                    if (resultCount > 1) {
                        returnVal = "multiple emails found";
                    }
                    else {
                        returnVal = "email not found";
                    }
                }

            } catch (Exception ex) {
                returnVal = ex.Message;
            } finally {
                dr.Close();
                connection.Close();
            }

            return returnVal;
        }

        /// <summary>
        /// Get's the user's information of the associated email.
        /// </summary>
        /// <param name="email">The email to look up.</param>
        /// <returns>The Player user associated with the given email.</returns>
        public Person GetPersonInformation(String email) {
            MySqlDataReader dr = null;
            Person returnVal = null;
            Permissions permissions = null;
            String query = "SELECT * FROM ";
            query += AppConstants.MYSQL_TABLE_PERSON + " person JOIN " + AppConstants.MYSQL_TABLE_PERMISSIONS + " permissions";
            query += " ON person.personID = permissions.personID";
            query += " WHERE email='" + email + "'";

            try {
                returnVal = new Person();
                permissions = new Permissions();
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();

                dr.Read();
                returnVal.ID = dr.GetInt64("personID");
                returnVal.firstName = dr.GetString("firstName");
                returnVal.lastName = dr.GetString("lastName");
                returnVal.email = dr.GetString("email");
                returnVal.setPassword(dr.GetString("password"));
                //returnVal.imageURL = dr.GetString("imageURL");
                //returnVal.height = dr.GetInt16("height");
                //returnVal.weight = dr.GetInt16("weight");

                permissions.coachEnabled = !(dr.GetInt16("coachEnabled") == 0);

                returnVal.permissions = permissions;
            }
            catch (Exception ex) {
            }
            finally {
                connection.Close();
            }

            return returnVal;
        }

        /// <summary>
        /// Updates the given Player's information.
        /// </summary>
        /// <param name="email">The entry's email to update. This is necesary if the email value is updated.</param>
        /// <param name="user">The Player with updated information to store.</param>
        /// <returns>If the update was successful.</returns>
        public bool UpdateUserInformation(String email, Person user) {
            String query = "UPDATE " + AppConstants.MYSQL_TABLE_PERSON + " SET ";
            query += "firstName='" + user.firstName + "', ";
            query += "lastName='" + user.lastName + "', ";
            query += "email='" + user.email + "', ";
            query += "height=" + user.height + ", ";
            query += "weight=" + user.weight + " ";
            query += "WHERE email='" + email + "'";
            return ExecuteInsert(query);
        }

        /// <summary>
        /// Updates the user's - matching the given email - password.
        /// </summary>
        /// <param name="email">The email of the user to update.</param>
        /// <param name="password">The new password to set.</param>
        /// <returns>If the update was successful.</returns>
        public bool UpdateUserPassword(String email, String password) {
            String encryptedPass = PasswordEncryptor.encrypt_password(password, AppConstants.PASSWORD_KEY);
            String query = "UPDATE " + AppConstants.MYSQL_TABLE_PERSON + " SET password='" + encryptedPass + "' WHERE email='" + email + "'";
            return ExecuteInsert(query);
        }

        /// <summary>
        /// Returns the personID associated with the given email address.
        /// </summary>
        /// <param name="email">The email address to lookup.</param>
        /// <returns>The associated personID.</returns>
        public long GetPersonID(String email) {
            MySqlDataReader dr = null;
            long returnID = -1;

            String query = "SELECT personID FROM ";
            query += AppConstants.MYSQL_TABLE_PERSON;
            query += " WHERE email='" + email + "'";

            try {
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();

                dr.Read();
                returnID = dr.GetInt64("personID");
            }
            catch (Exception ex) {
            }
            finally {
                connection.Close();
            }

            return returnID;
        }

        /// <summary>
        /// Get's the ID of the coach of the team with the given team ID.
        /// </summary>
        /// <param name="teamID">The team ID to look up.</param>
        /// <returns>The ID of the coach.</returns>
        public long GetCoachID(long teamID) {
            MySqlDataReader dr = null;
            long returnID = -1;

            String query = "SELECT coach FROM ";
            query += AppConstants.MYSQL_TABLE_TEAM;
            query += " WHERE teamID='" + teamID + "'";

            try {
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();

                dr.Read();
                returnID = dr.GetInt64("coach");
            }
            catch (Exception ex) {
            }
            finally {
                connection.Close();
            }

            return returnID;
        }

        /// <summary>
        /// Adds a team to the database.
        /// </summary>
        /// <param name="teamName">The name of the new team.</param>
        /// <param name="coachID">The ID of the associated coach.</param>
        /// <returns>The success of the insert.</returns>
        public bool AddNewTeam(String teamName, long coachID) {
            String query = "INSERT INTO ";
            query += AppConstants.MYSQL_TABLE_TEAM;
            query += " (name, coach) VALUES (";
            query += "'" + teamName + "', ";
            query += coachID + ")";
            return ExecuteInsert(query);
        }

        /// <summary>
        /// Get's the details of the team matching the given team ID.
        /// </summary>
        /// <param name="id">The team ID to look up.</param>
        /// <returns>The team in the DB that matches the ID.</returns>
        public Team GetTeamDetails(long id) {
            MySqlDataReader dr = null;
            Team returnVal = null;

            String query = "SELECT * FROM ";
            query += AppConstants.MYSQL_TABLE_TEAM + " JOIN " + AppConstants.MYSQL_TABLE_PERSON;
            query += " ON coach = personID";
            query += " WHERE teamID=" + id;

            bool needToClose = false;
            try {
                // Try to open a connection if one hasn't been opened.
                try {
                    connection.Open();
                    needToClose = true;
                }
                catch (Exception ex) {
                }

                command.CommandText = query;
                dr = command.ExecuteReader();

                dr.Read();
                
                Person tempPerson = new Person(dr.GetString("firstName"), dr.GetString("lastName"));
                tempPerson.ID = dr.GetInt64("personID");
                tempPerson.email = dr.GetString("email");
                tempPerson.setPassword(dr.GetString("password"));
                //coach.imageURL = dr.GetString("imageURL");
                //tempPerson.height = dr.GetInt16("height");
                //tempPerson.weight = dr.GetInt16("weight");

                returnVal = new Team(dr.GetString("name"), tempPerson);
                returnVal.ID = id;
            }
            catch (Exception ex) {
            }
            finally {
                if (needToClose) {
                    connection.Close();
                }
            }

            return returnVal;
        }

        /// <summary>
        /// Gets the names of the teams associated with the given email.
        /// </summary>
        /// <param name="email">The email of the associated coach.</param>
        /// <returns>The list of teams associated with the email.</returns>
        public List<Team> GetTeamListCoach(String email) {
            List<Team> teamList = new List<Team>();
            MySqlDataReader dr = null;

            // Get the team's the user is a coach of
            String query = "SELECT name, firstName, lastName, email, teamID FROM ";
            query += AppConstants.MYSQL_TABLE_TEAM + " team JOIN " + AppConstants.MYSQL_TABLE_PERSON + " person";
            query += " ON team.coach = person.personID ";
            query += "WHERE email='" + email + "'";

            try {
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();

                Person tempPerson;
                Team tempTeam;
                while (dr.Read()) {
                    tempPerson = new Person(dr.GetString("firstName"), dr.GetString("lastName"));
                    tempPerson.email = dr.GetString("email");
                    tempTeam = new Team(dr.GetString("name"), tempPerson);
                    tempTeam.ID = dr.GetInt64("teamID");
                    teamList.Add(tempTeam);
                }
            } catch (Exception ex) {
            } finally {
                connection.Close();
            }

            return teamList;
        }

        /// <summary>
        /// Gets the associated team information for persons with the given email 
        /// of which the person is a member and NOT a coach.
        /// </summary>
        /// <param name="email">The email of the person to match.</param>
        /// <returns>A list of the teams the matching person is a member of.</returns>
        public List<Team> GetTeamListMember(String email) {
            List<Team> teamList = new List<Team>();
            MySqlDataReader dr = null;
            
            string query = "SELECT name, firstName, lastName, email, team.teamID FROM ";
            query += AppConstants.MYSQL_TABLE_TEAMPERSON + " link JOIN " + AppConstants.MYSQL_TABLE_PERSON + " person";
            query += " ON link.personID = person.personID JOIN ";
            query += AppConstants.MYSQL_TABLE_TEAM + " team ON link.teamID = team.teamID ";
            query += "WHERE email='" + email + "'";

            try {
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();

                Person tempPerson;
                Team tempTeam;
                while (dr.Read()) {
                    tempPerson = new Person(dr.GetString("firstName"), dr.GetString("lastName"));
                    tempPerson.email = dr.GetString("email");
                    tempTeam = new Team(dr.GetString("name"), tempPerson);
                    tempTeam.ID = dr.GetInt64("teamID");
                    teamList.Add(tempTeam);
                }

                connection.Close();
                // Go through and get team information
                foreach (Team team in teamList) {
                    tempTeam = GetTeamDetails(team.ID);
                    if (tempTeam != null) {
                        team.ID = tempTeam.ID;
                        team.name = tempTeam.name;
                        team.coach = tempTeam.coach;
                    }
                }

            } catch (Exception ex) {
            } finally {
                try {
                    connection.Close();
                }
                catch (Exception ex) {
                }
            }

            return teamList;
        }

        /// <summary>
        /// Gets the full name of the user associated with the given email.
        /// </summary>
        /// <param name="email">The email to look up.</param>
        /// <returns>The full name ("firstName lastName") associated with the given email.</returns>
        public string GetUserName(String email) {
            string returnVal = "";
            MySqlDataReader dr = null;

            String query = "SELECT firstName, lastName FROM ";
            query += AppConstants.MYSQL_TABLE_PERSON;
            query += " WHERE email='" + email + "'";

            try {
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();

                dr.Read();
                returnVal = dr.GetString("firstName") + " " + dr.GetString("lastName");
            }
            catch (Exception ex) {
                returnVal = ex.Message;
            }
            finally {
                connection.Close();
            }

            return returnVal;
        }

        /// <summary>
        /// Returns a list of teams matching the given search pattern.
        /// </summary>
        /// <param name="searchName">The search pattern to use.</param>
        /// <returns>A list of teams matching the given search pattern.</returns>
        public List<Team> SearchTeams(String searchName) {
            List<Team> returnVal = new List<Team>();
            MySqlDataReader dr = null;

            String query = "SELECT name, firstName, lastName, email, teamID FROM ";
            query += AppConstants.MYSQL_TABLE_TEAM + " JOIN " + AppConstants.MYSQL_TABLE_PERSON;
            query += " ON " + AppConstants.MYSQL_TABLE_TEAM + ".coach = " + AppConstants.MYSQL_TABLE_PERSON + ".personID ";
            query += "WHERE name LIKE '%" + searchName + "%'";

            try {
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();

                Person tempPerson;
                Team tempTeam;
                while (dr.Read()) {
                    tempPerson = new Person(dr.GetString("firstName"), dr.GetString("lastName"));
                    tempPerson.email = dr.GetString("email");
                    tempTeam = new Team(dr.GetString("name"), tempPerson);
                    tempTeam.ID = dr.GetInt64("teamID");
                    returnVal.Add(tempTeam);
                }
            } catch (Exception ex) {
                returnVal = null;
            } finally {
                connection.Close();
            }

            return returnVal;
        }

        /// <summary>
        /// Gets the players on the team with the given team ID.
        /// </summary>
        /// <param name="teamID">The team ID of the team in question.</param>
        /// <returns>A list of players on the given team.</returns>
        public List<Person> GetTeamPlayers(long teamID) {
            List<Person> players = new List<Person>();
            MySqlDataReader dr = null;

            String query = "SELECT * FROM ";
            query += AppConstants.MYSQL_TABLE_TEAMPERSON + " link JOIN " + AppConstants.MYSQL_TABLE_PERSON + " person ";
            query += "ON link.personID = person.personID ";
            query += "WHERE teamID = " + teamID;

            try {
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();

                Person tempPlayer;
                while (dr.Read()) {
                    tempPlayer = new Person(dr.GetString("firstName"), dr.GetString("lastName"));
                    tempPlayer.email = dr.GetString("email");
                    tempPlayer.ID = (long)dr.GetUInt64("personID");
                    //tempPlayer.imageURL = dr.GetString("imageURL");
                    //tempPlayer.height = dr.GetUInt16("height");
                    //tempPlayer.weight = dr.GetUInt16("weight");
                    players.Add(tempPlayer);
                }
            }
            catch {
                players = null;
            }
            finally {
                connection.Close();
            }

            return players;
        }

        /// <summary>
        /// Adds a request entry to the requests table.
        /// </summary>
        /// <param name="requesteeEmail">The email of the person requesting to join the team.</param>
        /// <param name="teamID">The ID of the team being requested to join.</param>
        /// <returns>Success of the insert.</returns>
        public bool AddNewRequest(long requesteeID, long teamID) {
            // Get the assocated coach ID for the given team ID
            long coachID = GetCoachID(teamID);

            // Check for a current request in the system
            MySqlDataReader dr = null;
            String query = "SELECT requestID FROM ";
            query += AppConstants.MYSQL_TABLE_REQUESTS;
            query += " WHERE personID = " + requesteeID + " AND teamID = " + teamID + " AND coachID = " + coachID;

            try {
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();
                connection.Close();

                // If an entry is found a request is already pending, return true.
                if (dr.HasRows) {
                    return true;
                }
            }
            catch {
                connection.Close();
            }

            // Add the entry to the DB
            query = "INSERT INTO ";
            query += AppConstants.MYSQL_TABLE_REQUESTS;
            query += " (personID, teamID, coachID) VALUES (";
            query += "'" + requesteeID + "', ";
            query += "'" + teamID + "', ";
            query += coachID + ")";
            return ExecuteInsert(query);
        }

        /// <summary>
        /// Removes a request entry from the database if the request ID
        /// and coach ID are found in the database.
        /// </summary>
        /// <param name="coachID">The user requesting the entry be deleted.</param>
        /// <param name="requestID">The ID of the entry to be deleted</param>
        /// <returns>Success of the removal.</returns>
        public bool RemoveRequest(long coachID, long requestID) {
            // Add the entry to the DB
            String query = "DELETE FROM ";
            query += AppConstants.MYSQL_TABLE_REQUESTS;
            query += " WHERE requestID=" + requestID;
            query += " AND coachID=" + coachID;
            return ExecuteInsert(query);
        }

        /// <summary>
        /// Removes an invite entry from the database if the invite ID 
        /// and invitee email are found in the database.
        /// </summary>
        /// <param name="inviteeEmail">The email of the person invited to join a team.</param>
        /// <param name="inviteID">The ID of the invite.</param>
        /// <returns>Success of the removal.</returns>
        public bool RemoveInvite(string inviteeEmail, long inviteID) {
            // Add the entry to the DB
            String query = "DELETE FROM ";
            query += AppConstants.MYSQL_TABLE_INVITES;
            query += " WHERE inviteID=" + inviteID;
            query += " AND invitee='" + inviteeEmail + "'";
            return ExecuteInsert(query);
        }

        /// <summary>
        /// Gets the details of a single request matching the given request ID.
        /// </summary>
        /// <param name="requestID">The request ID to search for.</param>
        /// <returns>The details matching the request ID.</returns>
        public Request GetRequest(long requestID) {
            Request returnVal = null;
            MySqlDataReader dr = null;

            String query = "SELECT * FROM ";
            query += AppConstants.MYSQL_TABLE_PERSON + " person, " + AppConstants.MYSQL_TABLE_TEAM + " team, " + AppConstants.MYSQL_TABLE_REQUESTS + " request ";
            query += "WHERE person.personID=request.personID AND team.teamID=request.teamID AND request.requestID=" + requestID;

            try {
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();

                Person requestee;
                Team team;
                dr.Read();
                requestee = new Person(dr.GetString("firstName"), dr.GetString("lastName"));
                requestee.ID = dr.GetInt64("personID");
                requestee.email = dr.GetString("email");
                //requestee.imageURL = dr.GetString("imageURL");
                //requestee.height = dr.GetInt16("height");
                //requestee.weight = dr.GetInt16("weight");
                //requestee.birthday = dr.GetDateTime("birthday");

                team = new Team();
                team.ID = dr.GetInt64("teamID");
                team.name = dr.GetString("name");
                
                returnVal = new Request(requestee, team);
                returnVal.ID = requestID;
            } catch (Exception ex) {
                returnVal = null;
            } finally {
                connection.Close();
            }

            return returnVal;
        }

        /// <summary>
        /// Gets the requests from the database that are connected to the teams 
        /// the given user coaches.
        /// </summary>
        /// <param name="email">The email address of the coach.</param>
        /// <returns>A list of invites corresponding to the coaches' teams.</returns>
        public List<Request> GetRequests(String email) {
            List<Request> returnVal = new List<Request>();
            MySqlDataReader dr = null;

            Person coach = GetPersonInformation(email);

            String query = "SELECT * FROM ";
            query += AppConstants.MYSQL_TABLE_PERSON + " person, " + AppConstants.MYSQL_TABLE_TEAM + " team, " + AppConstants.MYSQL_TABLE_REQUESTS + " request ";
            query += "WHERE person.personID=request.personID AND team.teamID=request.teamID AND request.coachID=" + coach.ID;

            try {
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();

                Person requestee;
                Team team;
                Request request;
                while (dr.Read()) {
                    requestee = new Person(dr.GetString("firstName"), dr.GetString("lastName"));
                    requestee.ID = dr.GetInt64("personID");
                    requestee.email = dr.GetString("email");
                    //requestee.setPassword(dr.GetString("password"));
                    //requestee.imageURL = dr.GetString("imageURL");
                    //requestee.birthday = dr.GetDateTime("birthday");
                    //requestee.height = dr.GetInt16("height");
                    //requestee.weight = dr.GetInt16("weight");

                    team = new Team();
                    team.name = dr.GetString("name");
                    team.coach = coach;

                    request = new Request(requestee, team);
                    request.ID = dr.GetInt64("requestID");
                    returnVal.Add(request);
                }
            } catch (Exception ex) {
                returnVal = null;
            } finally {
                connection.Close();
            }

            return returnVal;
        }

        /// <summary>
        /// Gets requests tied to a specific user and team ID.
        /// </summary>
        /// <param name="teamID">The ID of the team.</param>
        /// <param name="userID">The ID of the requestee.</param>
        /// <returns>A list of requests matching the given team and requestee ID.</returns>
        public List<Request> GetRequests(long teamID, long userID) {
            List<Request> returnVal = new List<Request>();
            MySqlDataReader dr = null;

            String query = "SELECT * FROM ";
            query += AppConstants.MYSQL_TABLE_PERSON + " person, " + AppConstants.MYSQL_TABLE_TEAM + " team, " + AppConstants.MYSQL_TABLE_REQUESTS + " request ";
            query += "WHERE person.personID=request.personID AND team.teamID=request.teamID AND request.teamID=" + teamID + " AND request.personID=" + userID;

            try {
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();

                Person requestee;
                Team team;
                Request request;
                while (dr.Read()) {
                    requestee = new Person(dr.GetString("firstName"), dr.GetString("lastName"));
                    requestee.ID = dr.GetInt64("personID");
                    requestee.email = dr.GetString("email");
                    //requestee.setPassword(dr.GetString("password"));
                    //requestee.imageURL = dr.GetString("imageURL");
                    //requestee.birthday = dr.GetDateTime("birthday");
                    //requestee.height = dr.GetInt16("height");
                    //requestee.weight = dr.GetInt16("weight");

                    team = new Team();
                    team.name = dr.GetString("name");

                    request = new Request(requestee, team);
                    request.ID = dr.GetInt64("requestID");
                    returnVal.Add(request);
                }
            }
            catch (Exception ex) {
                returnVal = null;
            }
            finally {
                connection.Close();
            }

            return returnVal;
        }

        /// <summary>
        /// Gets the details of a single invitation matching the given invitation ID.
        /// </summary>
        /// <param name="inviteID">The invite ID to search for.</param>
        /// <returns>The details matching the invite ID.</returns>
        public Invitation GetInvite(long inviteID) {
            Invitation returnVal = null;
            MySqlDataReader dr = null;

            String query = "SELECT * FROM ";
            query += AppConstants.MYSQL_TABLE_PERSON + " person, " + AppConstants.MYSQL_TABLE_TEAM + " team, " + AppConstants.MYSQL_TABLE_INVITES + " invite ";
            query += "WHERE person.personID=invite.personID AND team.teamID=invite.teamID AND invite.inviteID=" + inviteID;

            try {
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();

                Person invitor;
                Team team;
                dr.Read();
                invitor = new Person(dr.GetString("firstName"), dr.GetString("lastName"));
                invitor.ID = dr.GetInt64("personID");
                invitor.email = dr.GetString("email");
                //invitor.imageURL = dr.GetString("imageURL");
                //invitor.height = dr.GetInt16("height");
                //invitor.weight = dr.GetInt16("weight");
                //invitor.birthday = dr.GetDateTime("birthday");

                team = new Team();
                team.ID = dr.GetInt64("teamID");
                team.name = dr.GetString("name");
                team.coach = invitor;

                returnVal = new Invitation(inviteID, team);
            }
            catch (Exception ex) {
                returnVal = null;
            }
            finally {
                connection.Close();
            }

            return returnVal;
        }

        /// <summary>
        /// Gets the invitations extended to the user with the given email.
        /// </summary>
        /// <param name="email">Email of the user of interest.</param>
        /// <returns>A list of invitations to the user with the given email.</returns>
        public List<Invitation> GetInvitations(String email) {
            List<Invitation> returnVal = new List<Invitation>();
            MySqlDataReader dr = null;

            String query = "SELECT * FROM ";
            query += AppConstants.MYSQL_TABLE_PERSON + " person, " + AppConstants.MYSQL_TABLE_TEAM + " team, " + AppConstants.MYSQL_TABLE_INVITES + " invite ";
            query += "WHERE person.personID=invite.personID AND team.teamID=invite.teamID AND invite.invitee='" + email + "'";

            try {
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();

                Person invitor;
                Team team;
                Invitation invitation;
                while (dr.Read()) {
                    invitor = new Person(dr.GetString("firstName"), dr.GetString("lastName"));
                    invitor.ID = dr.GetInt64("personID");
                    invitor.email = dr.GetString("email");
                    //invitor.setPassword(dr.GetString("password"));
                    //invitor.imageURL = dr.GetString("imageURL");
                    //invitor.birthday = dr.GetDateTime("birthday");
                    //invitor.height = dr.GetInt16("height");
                    //invitor.weight = dr.GetInt16("weight");

                    team = new Team();
                    team.name = dr.GetString("name");
                    team.coach = invitor;

                    invitation = new Invitation(dr.GetInt64("inviteID"), team);
                    returnVal.Add(invitation);
                }
            }
            catch (Exception) {
                returnVal = null;
            }
            finally {
                connection.Close();
            }

            return returnVal;
        }

        /// <summary>
        /// Adds an entry to the Team/Person database table linking the player
        /// to the given team.
        /// </summary>
        /// <param name="playerID">The ID of the player to link.</param>
        /// <param name="teamID">The ID of the team they're being linked to.</param>
        /// <returns>Success of the link.</returns>
        public bool AddPlayerToTeam(long playerID, long teamID) {
            // Add the entry to the DB
            String query = "INSERT INTO ";
            query += AppConstants.MYSQL_TABLE_TEAMPERSON;
            query += " (personID, teamID) VALUES (";
            query += playerID + ", ";
            query += teamID + ")";
            return ExecuteInsert(query);
        }

        /// <summary>
        /// Adds an entry to the invite database if one doesn't exist for the given 
        /// information or it gets the ID of the invite that does exist. The ID of 
        /// the associated invite is returned.
        /// </summary>
        /// <param name="invitee">The email of the person being invited.</param>
        /// <param name="invitor">The ID of the person inviting.</param>
        /// <param name="teamID">The ID of the team they're being invited to.</param>
        /// <returns>The ID of the invite that was added.</returns>
        public long AddInvite(string invitee, long invitor, long teamID) {
            // See if an entry exists with the given data
            long inviteID = 0;
            MySqlDataReader dr = null;

            String query = "SELECT inviteID FROM ";
            query += AppConstants.MYSQL_TABLE_INVITES;
            query += " WHERE invitee='" + invitee + "' AND ";
            query += "personID=" + invitor + " AND ";
            query += "teamID=" + teamID;

            bool inviteFound = false;
            try {
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();
                dr.Read();
                inviteID = dr.GetInt64("inviteID");
                inviteFound = true;
            }
            catch (Exception ex) {
            }
            finally {
                connection.Close();
            }

            if (!inviteFound) {
                // Insert the invite entry into the database if there isn't one
                String query2 = "INSERT INTO ";
                query2 += AppConstants.MYSQL_TABLE_INVITES;
                query2 += " (invitee, personID, teamID) VALUE ('";
                query2 += invitee + "', ";
                query2 += invitor + ", ";
                query2 += teamID + ")";
                bool insertSuccess = ExecuteInsert(query2);

                // Get the ID for the new entry into the database
                try {
                    connection.Open();
                    command.CommandText = query;
                    dr = command.ExecuteReader();
                    dr.Read();
                    inviteID = dr.GetInt64("inviteID");
                }
                catch (Exception ex) {
                }
                finally {
                    connection.Close();
                }
            }
            return inviteID;
        }

        /// <summary>
        /// Mark the invite as accepted (remove from DB) and add the invited player 
        /// to the team they were invited to.
        /// </summary>
        /// <param name="email">Email address of the invited player.</param>
        /// <param name="inviteID">The ID of the invitation.</param>
        /// <returns>The team information the player was invited to join.</returns>
        public Team AcceptInvite(string email, long inviteID) {
            // Get the team ID from the invite table
            long teamID, personID;
            MySqlDataReader dr = null;

            String query = "SELECT teamID, person.personID FROM ";
            query += AppConstants.MYSQL_TABLE_INVITES + " invites JOIN " + AppConstants.MYSQL_TABLE_PERSON + " person";
            query += " ON invites.invitee = person.email ";
            query += " WHERE inviteID=" + inviteID;

            try {
                connection.Open();
                command.CommandText = query;
                dr = command.ExecuteReader();
                dr.Read();
                teamID = dr.GetInt64("teamID");
                personID = dr.GetInt64("personID");
                connection.Close();
            }
            catch (Exception ex) {
                teamID = 0;
                personID = 0;
                connection.Close();
                return null;
            }
            finally {
                connection.Close();
            }

            // Remove the invite from the database
            query = "DELETE FROM ";
            query += AppConstants.MYSQL_TABLE_INVITES;
            query += " WHERE inviteID=" + inviteID;
            bool removeInvite = ExecuteInsert(query);

            // Add the player to the team he was invited to
            bool addSuccess = false;
            if (removeInvite) {
                addSuccess = AddPlayerToTeam(personID, teamID);
            }

            if (addSuccess) {
                return GetTeamDetails(teamID);
            }
            return null;
        }
    }
}