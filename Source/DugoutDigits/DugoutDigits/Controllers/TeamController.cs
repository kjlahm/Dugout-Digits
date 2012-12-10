using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DugoutDigits.Utilities;
using DugoutDigits.Objects;

namespace DugoutDigits.Controllers
{
    public class TeamController : Controller
    {
        /// <summary>
        /// Gets the list of teams (in the form of an ul) associated with the 
        /// user who is currently logged in.
        /// </summary>
        /// <returns>List of teams (name only).</returns>
        public ActionResult AJAX_GetTeams() {
            // Get the teams associated with the user who's logged in
            DBAccessor dba = new DBAccessor();
            List<Team> teamsCoach = dba.GetTeamListCoach(User.Identity.Name);
            List<Team> teamsMember = dba.GetTeamListMember(User.Identity.Name);

            // Create ul to hold the team names
            string result = "<ul>\n";
            foreach (Team team in teamsCoach) {
                result += "<li>" + team.name + "</li>\n";
            }
            foreach (Team team in teamsMember) {
                result += "<li>" + team.name + "</li>\n";
            }
            result += "</ul>\n";

            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Returns an HTML drop down containing a list of teams the logged in 
        /// user is associated with. The association parameter allows you to 
        /// pick if you want teams the user coaches (0), is a member of (1) or 
        /// both (2).
        /// </summary>
        /// <param name="association">The desired associativity of the user to the teams.</param>
        /// <returns>An HTML drop down the teams the user is associated with.</returns>
        public ActionResult AJAX_GetTeamsDropDown(int association) {
            // Get the teams associated with the user who's logged in
            DBAccessor dba = new DBAccessor();
            List<Team> teamsCoach = new List<Team>();
            List<Team> teamsMember = new List<Team>();

            if (association == 0 || association == 2) {
                teamsCoach = dba.GetTeamListCoach(User.Identity.Name);
            }
            if (association == 1 || association == 2) {
                teamsMember = dba.GetTeamListMember(User.Identity.Name);
            }

            // Create dropdown to hold the teams
            string result = "<select id='teamdropdown'>\n";
            foreach (Team team in teamsCoach) {
                result += "<option value='" + team.ID + "'>" + team.name + "</option>\n";
            }
            foreach (Team team in teamsMember) {
                result += "<option value='" + team.ID + "'>" + team.name + "</option>\n";
            }
            result += "</select>\n";

            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Returns a list of teams associated with the authenticated user.
        /// </summary>
        /// <returns>HTML table containing a list of teams.</returns>
        public ActionResult AJAX_GetTeamsTable() {
            // Get the teams associated with the user who's logged in
            DBAccessor dba = new DBAccessor();
            List<Team> teamsCoached = dba.GetTeamListCoach(User.Identity.Name);
            List<Team> teamsMember = dba.GetTeamListMember(User.Identity.Name);

            // Create table to hold the team names
            string result = "<div id='team-tables'><h3>My Teams</h3>\n";
            if (teamsCoached.Count > 0) {
                result += "<table>\n";
                result += "<tr><th>Team Name</th><th>Coach's Name</th></tr>\n";
                
                foreach (Team team in teamsCoached) {
                    result += "<tr><td>" + team.name + "</td><td>You</td></tr>\n";
                }

                foreach (Team team in teamsMember) {
                    result += "<tr><td>" + team.name + "</td><td>" + team.coach.firstName + " " + team.coach.lastName +"</td></tr>\n";
                }

                result += "</table>\n";
            } else {
                result += "<p>You haven't added or joined any teams. Add or search for a team using the controls above.</p>";
            }
            result += "</div><div id='team-requests'></div>";

            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Returns a list of the pending requests associated with the authenticated user.
        /// </summary>
        /// <returns>HTML table containing a list of requests.</returns>
        public ActionResult AJAX_GetRequestTable() {
            string result = "<p>Could not authenticate the request.</p>\n";
            if (Request.IsAuthenticated) {
                result = "<h3>Pending Requests</h3>\n";

                // Call the database to get pending requests
                DBAccessor dba = new DBAccessor();
                List<Request> requests = dba.GetRequests(User.Identity.Name);

                // Form an HTML table with the retrieved information
                if (requests.Count > 0) {
                    result += "<table>\n";
                    result += "<tr><th>Request From</th><th>Requests to Join</th><th>Action</th></tr>\n";
                    foreach (Request request in requests) {
                        string playerName = request.requestee.firstName + " " + request.requestee.lastName;
                        result += "<tr><td>" + playerName + "</td><td>" + request.team.name + "</td><td><div onClick='action_acceptrequest(" + request.ID + ")'>Accept</div><div onClick='action_declinerequest(" + request.ID + ")'>Decline</div></td></tr>";
                    }
                    result += "</table>\n";
                } else {
                    result += "<p>There are no pending requests at this time.</p>\n";
                }
            }

            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Adds a team to the database with the given name and the current logged 
        /// in user as the coach.
        /// </summary>
        /// <returns>Success of the addition.</returns>
        public ActionResult AJAX_AddTeam(string teamName) {
            // Make sure the user is authenticated
            string result = "Request not authenticated.";
            if (Request.IsAuthenticated) {

                // Get the person id for the user currently logged in
                DBAccessor dba = new DBAccessor();
                long coachID = dba.GetPersonID(User.Identity.Name);

                // Add the team to the database
                result = "Error adding the team.";
                if (dba.AddNewTeam(teamName, coachID)) {
                    result = teamName + " successfully added.";
                }
            }

            // Return the success message of the addition
            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Searches the database for teams matching the given search term.
        /// </summary>
        /// <param name="teamName">Team name or partial name to search for.</param>
        /// <returns>A list of matching teams.</returns>
        public ActionResult AJAX_SearchTeams(string teamName) {
            // Make sure the user is authenticated
            string result = "Request not authenticated.";
            if (Request.IsAuthenticated) {
                
                // Search the DB using the DBA
                DBAccessor dba = new DBAccessor();
                List<Team> matchedTeams = dba.SearchTeams(teamName);

                if (matchedTeams != null) {
                    if (matchedTeams.Count >= 1) {
                        // Form an html table with the results
                        result = "<h4>Team Matches</h4>\n";
                        result += "<table>\n";
                        result += "<tr><th>Team Name</th><th>Coach's Name</th><th>Request to Join</th></tr>\n";
                        foreach (Team team in matchedTeams) {
                            string coachName = team.coach.firstName + " " + team.coach.lastName;
                            string joinCell = "<div id='RQ_" + team.ID + "' onClick='action_requestjoin(" + team.ID + ")'>Join</div>";
                            if (team.coach.email.Equals(User.Identity.Name)) {
                                coachName = "You";
                                joinCell = "N/A";
                            }
                            result += "<tr><td>" + team.name + "</td><td>" + coachName + "</td><td>" + joinCell + "</td></tr>\n";
                        }
                        result += "</table>\n";
                    } else {
                        result = "No matches were found.";
                    }
                } else {
                    result = "The query to the database failed.";
                }
            }

            // Return the success message of the addition
            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Adds a request entry to the database.
        /// </summary>
        /// <param name="teamID">The ID of the team being requested to join.</param>
        /// <returns>Success message of the request.</returns>
        public ActionResult AJAX_AddRequest(long teamID) {
            // Make sure the user is authenticated
            string result = "Request not authenticated.";
            
            if (Request.IsAuthenticated) {
                // Get the person id for the user currently logged in
                DBAccessor dba = new DBAccessor();
                long requesteeID = dba.GetPersonID(User.Identity.Name);

                // Add the request to the database
                result = "Error making the request.";
                if (dba.AddNewRequest(requesteeID, teamID)) {
                    result = "Request sent.";
                }
            }

            // Return the success message of the addition
            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Removes a request entry from the database.
        /// </summary>
        /// <param name="requestID">The ID of the request entry to remove.</param>
        /// <returns>Success message of the request removal.</returns>
        public ActionResult AJAX_RemoveRequest(long requestID) {
            // Make sure the user is authenticated
            string result = "Request not authenticated.";

            if (Request.IsAuthenticated) {
                // Get the person id for the user currently logged in
                DBAccessor dba = new DBAccessor();
                long requesteeID = dba.GetPersonID(User.Identity.Name);

                // Remove the request to the database
                result = "Error making the request.";
                if (dba.RemoveRequest(requesteeID, requestID)) {
                    result = "Request removed.";
                }
            }

            // Return the success message of the removal
            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Accepts the given request to join a team. The player that requested to 
        /// join the team is linked as a player of that team. The entry in the 
        /// request table is then deleted.
        /// </summary>
        /// <param name="requestID">The ID of the request which was accepted.</param>
        /// <returns>The result of the accept.</returns>
        public ActionResult AJAX_AcceptRequest(long requestID) {
            // Make sure the user is authenticated
            string result = "Request not authenticated.";

            if (Request.IsAuthenticated) {
                DBAccessor dba = new DBAccessor();

                // Get the player and team IDs from the database
                Request request = dba.GetRequest(requestID);

                // Ensure the get request call worked
                if (request == null) {
                    result = "Error finding the request in the database.";

                } else {
                    // Link the player to the team
                    if (dba.AddPlayerToTeam(request.requestee.ID, request.team.ID)) {

                        // Remove the request entry from the database
                        long requesteeID = dba.GetPersonID(User.Identity.Name);
                        if (dba.RemoveRequest(requesteeID, requestID)) {
                            result = request.requestee.firstName + " " + request.requestee.lastName + " added to " + request.team.name + " successfully.";
                        
                        // Indicate the accept went through but the request wasn't removed
                        } else {
                            result = request.requestee.firstName + " " + request.requestee.lastName + " added to " + request.team.name + " but the request wasn't removed.";
                        }

                    // If the link failed set an appropriate message
                    } else {
                        result = "Error adding " + request.requestee.firstName + " " + request.requestee.lastName + " to " + request.team.name;
                    }
                }
            }

            // Return the success message of the accept
            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }
    }
}
