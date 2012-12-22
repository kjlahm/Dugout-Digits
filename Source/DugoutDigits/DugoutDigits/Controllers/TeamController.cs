using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DugoutDigits.Utilities;
using DugoutDigits.Objects;
using System.Net.Mail;
using System.Net;
using System.Globalization;

namespace DugoutDigits.Controllers
{
    public class TeamController : Controller
    {
        public ActionResult Overview() {
            // If the request isn't logged in redirect to Logon.
            if (Request.IsAuthenticated) {

                // Try to get the team ID from the URL
                long teamID = 0;
                try {
                    teamID = Convert.ToInt64(Request.QueryString["teamID"]);
                }
                catch {
                    return RedirectToAction("Index", "Home");
                }

                // Get the team information
                DBAccessor dba = new DBAccessor();
                Team team = dba.GetTeamDetails(teamID);
                Person user = new Person();
                user.email = User.Identity.Name;

                ViewBag.Title = team.name;
                ViewBag.Name = team.name;
                ViewBag.LogoURL = team.logoURL;
                ViewBag.TeamID = team.ID;
                ViewBag.IsCoach = team.coaches.Contains(user, new PersonComparer());

                return View();
            }
            return RedirectToAction("LogOn", "Account");
        }

        public ActionResult Schedule() {
            // If the request isn't logged in redirect to Logon.
            if (Request.IsAuthenticated) {

                // Try to get the team ID from the URL
                long teamID = 0;
                try {
                    teamID = Convert.ToInt64(Request.QueryString["teamID"]);
                }
                catch {
                    return RedirectToAction("Index", "Home");
                }

                // Get the team information
                DBAccessor dba = new DBAccessor();
                Team team = dba.GetTeamDetails(teamID);
                Person user = new Person();
                user.email = User.Identity.Name;

                ViewBag.Title = team.name;
                ViewBag.Name = team.name;
                ViewBag.LogoURL = team.logoURL;
                ViewBag.TeamID = team.ID;
                ViewBag.IsCoach = team.coaches.Contains(user, new PersonComparer());

                return View();
            }
            return RedirectToAction("LogOn", "Account");
        }

        public ActionResult Roster() {
            // If the request isn't logged in redirect to Logon.
            if (Request.IsAuthenticated) {

                // Try to get the team ID from the URL
                long teamID = 0;
                try {
                    teamID = Convert.ToInt64(Request.QueryString["teamID"]);
                }
                catch {
                    return RedirectToAction("Index", "Home");
                }

                // Get the team information
                DBAccessor dba = new DBAccessor();
                Team team = dba.GetTeamDetails(teamID);
                Person user = new Person();
                user.email = User.Identity.Name;

                ViewBag.Title = team.name;
                ViewBag.Name = team.name;
                ViewBag.LogoURL = team.logoURL;
                ViewBag.TeamID = team.ID;
                ViewBag.IsCoach = team.coaches.Contains(user, new PersonComparer());

                return View();
            }
            return RedirectToAction("LogOn", "Account");
        }

        public ActionResult Stats() {
            // If the request isn't logged in redirect to Logon.
            if (Request.IsAuthenticated) {

                // Try to get the team ID from the URL
                long teamID = 0;
                try {
                    teamID = Convert.ToInt64(Request.QueryString["teamID"]);
                }
                catch {
                    return RedirectToAction("Index", "Home");
                }

                // Get the team information
                DBAccessor dba = new DBAccessor();
                Team team = dba.GetTeamDetails(teamID);
                Person user = new Person();
                user.email = User.Identity.Name;

                ViewBag.Title = team.name;
                ViewBag.Name = team.name;
                ViewBag.LogoURL = team.logoURL;
                ViewBag.TeamID = team.ID;
                ViewBag.IsCoach = team.coaches.Contains(user, new PersonComparer());

                return View();
            }
            return RedirectToAction("LogOn", "Account");
        }

        public ActionResult Messages() {
            // If the request isn't logged in redirect to Logon.
            if (Request.IsAuthenticated) {

                // Try to get the team ID from the URL
                long teamID = 0;
                try {
                    teamID = Convert.ToInt64(Request.QueryString["teamID"]);
                }
                catch {
                    return RedirectToAction("Index", "Home");
                }

                // Get the team information
                DBAccessor dba = new DBAccessor();
                Team team = dba.GetTeamDetails(teamID);
                Person user = new Person();
                user.email = User.Identity.Name;

                ViewBag.Title = team.name;
                ViewBag.Name = team.name;
                ViewBag.LogoURL = team.logoURL;
                ViewBag.TeamID = team.ID;
                ViewBag.IsCoach = team.coaches.Contains(user, new PersonComparer());

                return View();
            }
            return RedirectToAction("LogOn", "Account");
        }

        public ActionResult Join() {
            if (Request.IsAuthenticated) {

                string inviteEmail = Request.QueryString["email"];
                if (User.Identity.Name == inviteEmail) {

                    try {
                        // Get the user information
                        DBAccessor dba = new DBAccessor();
                        Person user = dba.GetPersonInformation(User.Identity.Name);
                        ViewBag.FirstName = user.firstName;
                        ViewBag.LastName = user.lastName;
                        ViewBag.ImageURL = user.imageURL;

                        // Perform the addition of the team member
                        long inviteID = Convert.ToInt64(Request.QueryString["id"]);
                        Team team = dba.AcceptInvite(User.Identity.Name, inviteID);
                        ViewBag.TeamName = team.name;
                    }
                    catch { }

                    return View();
                }
                else {
                    return RedirectToAction("Logoff", "Account");
                }
            }
            else {
                return RedirectToAction("Logon", "Account");
            }
        }

        /// <summary>
        /// Gets the list of teams (in the form of an ul) associated with the 
        /// user who is currently logged in.
        /// </summary>
        /// <returns>List of teams (name only).</returns>
        public ActionResult AJAX_GetTeams() {
            // Get the teams associated with the user who's logged in
            DBAccessor dba = new DBAccessor();
            List<Team> teams = dba.GetTeamListCoach(User.Identity.Name);
            teams.AddRange(dba.GetTeamListMember(User.Identity.Name));

            // Create ul to hold the team names
            string result = "<ul>\n";
            foreach (Team team in teams) {
                result += "<li><div onClick='action_gototeam(" + team.ID + ")'>" + team.name + "</div></li>\n";
            }
            result += "</ul>\n";

            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        public ActionResult AJAX_GetTeams2(long teamID) {
            // Get the teams associated with the user who's logged in
            DBAccessor dba = new DBAccessor();
            List<Team> teams = dba.GetTeamListCoach(User.Identity.Name);
            teams.AddRange(dba.GetTeamListMember(User.Identity.Name));

            // See if a team to match was given, if so, move it to the front
            int i = teams.FindIndex(t => t.ID.Equals(teamID));
            Team temp = teams[i];
            teams[i] = teams[0];
            teams[0] = temp;

            // Create ul to hold the team names
            string result = "<ul>\n";
            foreach (Team team in teams) {
                result += "<li><div onClick='action_gototeam(" + team.ID + ")'>" + team.name + "</div></li>\n";
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
            string result = "<select  class='editor-field' id='teamdropdown'>\n";
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
            string result = "<p>Could not authenticate the request.</p>\n";
            if (Request.IsAuthenticated) {
                result = "";

                // Get the teams associated with the user who's logged in
                DBAccessor dba = new DBAccessor();
                List<Team> teamsCoached = dba.GetTeamListCoach(User.Identity.Name);
                List<Team> teamsMember = dba.GetTeamListMember(User.Identity.Name);

                // Create table to hold the team names
                if (teamsCoached.Any() || teamsMember.Any()) {
                    result += "<table>\n";
                    result += "<tr><th>Team Name</th><th>Coached By</th><th>Action</th></tr>\n";

                    foreach (Team team in teamsCoached) {
                        result += "<tr><td><div class='clickable-text'>" + team.name + "</div></td><td>You</td><td><div onClick='action_detailsremoveteam(" + team.ID + ")' class='clickable-text'>Remove</div></td></tr>\n";
                    }

                    foreach (Team team in teamsMember) {
                        string coachNames = team.coaches[0].firstName + " " + team.coaches[0].lastName;
                        for (int i = 1; i < team.coaches.Count; i++) {
                            coachNames += ", " + team.coaches[i].firstName + " " + team.coaches[i].lastName;
                        }
                        result += "<tr><td><div class='clickable-text'>" + team.name + "</div></td><td>" + coachNames + "</td><td><div onClick='action_leaveteam(" + team.ID + ")' class='clickable-text'>Leave</div></td></tr>\n";
                    }

                    result += "</table>\n";
                }
                else {
                    result += "<p>You haven't added or joined any teams. Add or search for a team using the controls above.</p>";
                }
            }

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

                DBAccessor dba = new DBAccessor();
                Person person = dba.GetPersonInformation(User.Identity.Name);

                if (person.permissions.coachEnabled) {
                    result = "";

                    // Call the database to get pending requests
                    List<Request> requests = dba.GetRequests(User.Identity.Name);

                    // Form an HTML table with the retrieved information
                    if (requests.Any()) {
                        result += "<table>\n";
                        result += "<tr><th>Request From</th><th>Requests to Join</th><th>Action</th></tr>\n";
                        foreach (Request request in requests) {
                            string playerName = request.requestee.firstName + " " + request.requestee.lastName;
                            result += "<tr><td>" + playerName + "</td><td>" + request.team.name + "</td>";
                            result += "<td><img src='./../Content/images/accept.png' height='20' width='20' class='request-action-image' alt='accept' onClick='action_acceptrequest(" + request.ID + ")' />";
                            result += "<img src='./../Content/images/decline.png' height='20' width='20' class='request-action-image' margin-right='5px' alt='decline' onClick='action_declinerequest(" + request.ID + ")' />";
                            result += "<div onClick='action_detailsrequest(" + request.ID + ")' class='request-action-text clickable-text'>Details</div></td></tr>";
                        }
                        result += "</table>\n";
                    }
                    else {
                        result += "<p>There are no pending requests at this time.</p>\n";
                    }
                }
                else {
                    result = "";
                }
            }

            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Returns an HTML table of the requests that the authenticated user has initiated 
        /// and are still open.
        /// </summary>
        /// <returns></returns>
        public ActionResult AJAX_GetOpenRequestTable() {
            string result = "<p>Could not authenticate the request.</p>\n";
            if (Request.IsAuthenticated) {

                DBAccessor dba = new DBAccessor();

                result = "";

                // Call the database to get pending requests
                List<Request> requests = dba.GetMyOpenRequests(User.Identity.Name);

                // Form an HTML table with the retrieved information
                if (requests.Any()) {
                    result += "<table>\n";
                    result += "<tr><th>Requested to Join</th><th>Sent On</th><th>Remove</th></tr>\n";
                    foreach (Request request in requests) {
                        result += "<tr><td>" + request.team.name + "</td><td>" + request.timestamp.ToString("m") + "</td>";
                        result += "<td><div onClick='action_removerequest(" + request.ID + ")' class='request-action-text clickable-text'>Remove</div></td></tr>";
                    }
                    result += "</table>\n";
                }
                else {
                    result += "<p>You have no open requests at this time.</p>\n";
                }
            }

            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Returns a list of the pending invites associated with the authenticated user.
        /// </summary>
        /// <returns></returns>
        public ActionResult AJAX_GetInviteTable() {
            string result = "<p>Could not authenticate the request.</p>\n";
            if (Request.IsAuthenticated) {

                DBAccessor dba = new DBAccessor();
                Person person = dba.GetPersonInformation(User.Identity.Name);

                result = "";

                // Call the database to get pending requests
                List<Invitation> invitations = dba.GetInvites(User.Identity.Name);

                // Form an HTML table with the retrieved information
                if (invitations.Any()) {
                    result += "<table>\n";
                    result += "<tr><th>Invitation From</th><th>Invitation to Join</th><th>Action</th></tr>\n";
                    foreach (Invitation invitation in invitations) {
                        string playerName = invitation.invitor.firstName + " " + invitation.invitor.lastName;
                        result += "<tr><td>" + playerName + "</td><td>" + invitation.team.name + "</td>";
                        result += "<td><img src='./../Content/images/accept.png' height='20' width='20' class='request-action-image' alt='accept' onClick='action_acceptinvite(" + invitation.ID + ")' />";
                        result += "<img src='./../Content/images/decline.png' height='20' width='20' class='request-action-image' margin-right='5px' alt='decline' onClick='action_declineinvite(" + invitation.ID + ")' />";
                        result += "<div onClick='action_detailsinvite(" + invitation.ID + ")' class='request-action-text clickable-text'>Details</div></td></tr>";
                    }
                    result += "</table>\n";
                }
                else {
                    result += "<p>There are no pending invites at this time.</p>\n";
                }
            }

            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Returns an HTML table of the invites that the authenticated user has initiated 
        /// and are still open.
        /// </summary>
        /// <returns></returns>
        public ActionResult AJAX_GetOpenInviteTable() {
            string result = "<p>Could not authenticate the request.</p>\n";
            if (Request.IsAuthenticated) {

                DBAccessor dba = new DBAccessor();
                Person user = dba.GetPersonInformation(User.Identity.Name);

                if (user.permissions.coachEnabled) {
                    result = "";

                    // Call the database to get pending requests
                    List<Invitation> invitations = dba.GetMyOpenInvites(User.Identity.Name);

                    // Form an HTML table with the retrieved information
                    if (invitations.Any()) {
                        result += "<table>\n";
                        result += "<tr><th>Invited</th><th>To Join</th><th>Sent On</th><th>Remove</th></tr>\n";
                        foreach (Invitation invitation in invitations) {
                            result += "<tr><td>" + invitation.invitee + "</td><td>" + invitation.team.name + "</td><td>" + invitation.timestamp.ToString("m") + "</td>";
                            result += "<td><div onClick='action_removeinvite(" + invitation.ID + ")' class='request-action-text clickable-text'>Remove</div></td></tr>";
                        }
                        result += "</table>\n";
                    }
                    else {
                        result += "<p>You have no open invites at this time.</p>\n";
                    }
                }
                else {
                    result = "";
                }
            }

            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Returns the details for a single request denoted by the given request ID.
        /// </summary>
        /// <param name="requestID">The ID of the request in interest.</param>
        /// <returns>Returns an HTML format with the details of the request.</returns>
        public ActionResult AJAX_GetRequest(long requestID) {
            string result = "<p>Could not authenticate the request.</p>\n";
            if (Request.IsAuthenticated) {

                DBAccessor dba = new DBAccessor();
                Request request = dba.GetRequest(requestID);

                // Form the player information
                result = "<div class='lightbox-content-close clickable-text' onclick='action_hidedetails()'>Close</div>";
                result += "<h3>Pending Request</h3>\n";
                result += "<div id='request-details-left'>\n<img src='" + request.requestee.imageURL + "' alt='player picture' />\n</div>\n";
                result += "<div id='request-details-right'>\n";
                result += "<p>" + request.requestee.firstName + " " + request.requestee.lastName + "</p>\n";
                result += "<p>" + request.requestee.email + "</p>\n";
                result += "<p> Requests to join the " + request.team.name + "</p>\n";
                result += "<img src='./../Content/images/accept.png' height='20' width='20' class='request-action-image' alt='accept' onClick='action_acceptrequest(" + request.ID + ")' />";
                result += "<img src='./../Content/images/decline.png' height='20' width='20' class='request-action-image' margin-right='5px' alt='decline' onClick='action_declinerequest(" + request.ID + ")' />";
                result += "</div>\n";
            }

            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Returns the details for a single invite denoted by the given invite ID.
        /// </summary>
        /// <param name="inviteID">The ID of the invite in interest.</param>
        /// <returns>Returns an HTML format with the details of the invite.</returns>
        public ActionResult AJAX_GetInvite(long inviteID) {
            string result = "<p>Could not authenticate the request.</p>\n";
            if (Request.IsAuthenticated) {

                DBAccessor dba = new DBAccessor();
                Invitation invite = dba.GetInvite(inviteID);

                // Form the player information
                result = "<div class='lightbox-content-close clickable-text' onclick='action_hidedetails()'>Close</div>";
                result += "<h3>Pending Invite</h3>\n";
                result += "<div id='invite-details-left'>\n<img src='" + invite.invitor.imageURL + "' alt='coach picture' />\n</div>\n";
                result += "<div id='invite-details-right'>\n";
                result += "<p>Invited by " + invite.invitor.firstName + " " + invite.invitor.lastName + "</p>\n";
                result += "<p>Invitation to join the " + invite.team.name + "</p>\n";
                result += "<img src='./../Content/images/accept.png' height='20' width='20' class='request-action-image' alt='accept' onClick='action_acceptinvite(" + invite.ID + ")' />";
                result += "<img src='./../Content/images/decline.png' height='20' width='20' class='request-action-image' margin-right='5px' alt='decline' onClick='action_declineinvite(" + invite.ID + ")' />";
                result += "</div>\n";
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

                if (teamName != null && !teamName.Equals("")) {

                    // Get the person id for the user currently logged in
                    DBAccessor dba = new DBAccessor();
                    long coachID = dba.GetPersonID(User.Identity.Name);

                    // Add the team to the database
                    result = "Error adding the team.";
                    Team newTeam = new Team();
                    newTeam.name = teamName;
                    if (dba.AddNewTeam(newTeam, coachID)) {
                        result = teamName + " successfully added.";
                    }
                }
                else {
                    result = "Please enter a name for the new team.";
                }
            }

            // Return the success message of the addition
            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Removes a team from the database with the matching teamID.
        /// </summary>
        /// <param name="teamID">The ID of the team to remove from the database.</param>
        /// <returns>A message telling if the removal was successful.</returns>
        public ActionResult AJAX_RemoveTeam(long teamID) {
            string result = "Request not authenticated.";

            if (Request.IsAuthenticated) {
                // Get the team that is to be removed to validate the authenticated user can remove it
                DBAccessor dba = new DBAccessor();
                Team team = dba.GetTeamDetails(teamID);
                Person user = new Person();
                user.email = User.Identity.Name;

                if (team.coaches.Contains(user, new PersonComparer())) {
                    if (dba.RemoveTeam(teamID)) {
                        result = "Team removed successfully.";
                    }
                    else {
                        result = "Error removing the team from the database.";
                    }
                }
                else {
                    result = "Invalid attempt to remove team.";
                    dba.LogInvalidRequest(User.Identity.Name, "Attempt to remove the team " + team.name + "(ID: " + team.ID + ").");
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
                
                // Make sure a value was bound for team name
                if (teamName != null && !teamName.Equals("")) {

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
                                // Get the players of the team
                                List<Person> teamMembers = dba.GetTeamPlayers(team.ID);
                                Person user = new Person();
                                user.email = User.Identity.Name;

                                string coachName = team.CoachesDisplay;
                                string joinCell = "<div id='RQ_" + team.ID + "' class='clickable-text' onClick='action_requestjoin(" + team.ID + ")'>Send Request</div>";
                                if (team.coaches.Contains(user, new PersonComparer())) {
                                    coachName = "You";
                                    joinCell = "N/A";
                                }
                                else if (teamMembers.Contains(user, new PersonComparer())) {
                                    joinCell = "You're on the Team";
                                }
                                result += "<tr><td>" + team.name + "</td><td>" + coachName + "</td><td>" + joinCell + "</td></tr>\n";
                            }
                            result += "</table>\n";
                        }
                        else {
                            result = "No matches were found.";
                        }
                    }
                    else {
                        result = "The query to the database failed.";
                    }
                }
                else {
                    result = "Please enter a name or part of a name to search.";
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

                // Get any previous requests to join the team
                List<Request> requests = dba.GetRequests(teamID, requesteeID);

                if (requests.Any()) {
                    result = "A request has already been sent.";
                }
                else {
                    // Add the request to the database
                    result = "Error making the request.";
                    if (dba.AddNewRequest(requesteeID, teamID)) {
                        result = "Request sent.";
                    }
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
                Person requestee = dba.GetPersonInformation(User.Identity.Name);

                // Get the request that's trying to be removed
                Request request = dba.GetRequest(requestID);

                if (request.requestee.email == requestee.email || request.team.coaches.Contains(requestee, new PersonComparer())) {
                    // Remove the request to the database
                    result = "Error making the request.";
                    if (dba.RemoveRequest(requestID)) {
                        result = "Request removed.";
                    }
                } else {
                    result = "Invalid attempt to remove request.";
                    String message = "Attempt to remove request from " + request.requestee.firstName + " " + request.requestee.lastName + " (ID " + request.requestee.ID + ") ";
                    message += "to join " + request.team.name + " (ID " + request.team.ID + ").";
                    dba.LogInvalidRequest(User.Identity.Name, message);
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
                    Person user = new Person();
                    user.email = User.Identity.Name;
                    if (request.team.coaches.Contains(user, new PersonComparer())) {

                        // Link the player to the team
                        if (dba.AddPlayerToTeam(request.requestee.ID, request.team.ID)) {

                            // Remove the request entry from the database
                            long requesteeID = dba.GetPersonID(User.Identity.Name);
                            if (dba.RemoveRequest(requestID)) {
                                result = request.requestee.firstName + " " + request.requestee.lastName + " added to " + request.team.name + " successfully.";

                                // Indicate the accept went through but the request wasn't removed
                            }
                            else {
                                result = request.requestee.firstName + " " + request.requestee.lastName + " added to " + request.team.name + " but the request wasn't removed.";
                            }

                            // If the link failed set an appropriate message
                        }
                        else {
                            result = "Error adding " + request.requestee.firstName + " " + request.requestee.lastName + " to " + request.team.name;
                        }
                    }
                    else {
                        result = "Invalid attempt to accept request.";
                        String message = "Attempt to accept request from " + request.requestee.firstName + " " + request.requestee.lastName + " (ID " + request.requestee.ID + ") ";
                        message += "to join " + request.team.name + " (ID " + request.team.ID + ").";
                        dba.LogInvalidRequest(User.Identity.Name, message);
                    }
                }
            }

            // Return the success message of the accept
            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Removes the invite matching the given invite ID from the database.
        /// </summary>
        /// <param name="inviteID">The ID of the invite to remove.</param>
        /// <returns>Success message of the invite removal.</returns>
        public ActionResult AJAX_RemoveInvite(long inviteID) {
            // Make sure the user is authenticated
            string result = "Request not authenticated.";

            if (Request.IsAuthenticated) {
                DBAccessor dba = new DBAccessor();
                Invitation invite = dba.GetInvite(inviteID);

                if (invite.invitor.email.Equals(User.Identity.Name) || invite.invitee == User.Identity.Name) {
                    // Remove the request to the database
                    result = "Error making the request.";
                    if (dba.RemoveInvite(inviteID)) {
                        result = "Invitation removed.";
                    }
                }
                else {
                    result = "Invalid request to remove invite.";
                    dba.LogInvalidRequest(User.Identity.Name, "Attempt to remove invite (ID " + invite.ID + ").");
                }
            }

            // Return the success message of the removal
            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Accepts the given invite to join a team. The player invited to join 
        /// the team is linked as a player of that team. The entry in the invites 
        /// table is then deleted.
        /// </summary>
        /// <param name="inviteID">The ID of the invite which was accepted.</param>
        /// <returns>The result of the accept.</returns>
        public ActionResult AJAX_AcceptInvite(long inviteID) {
            // Make sure the user is authenticated
            string result = "Request not authenticated.";

            if (Request.IsAuthenticated) {
                DBAccessor dba = new DBAccessor();

                // Get the invite from the database
                Invitation invite = dba.GetInvite(inviteID);

                // Get the current user's ID from the database
                long userID = dba.GetPersonID(User.Identity.Name);

                // Ensure the get invite call worked
                if (invite == null) {
                    result = "Error finding the invite in the database.";
                }
                else {
                    if (invite.invitee.Equals(User.Identity.Name)) {

                        // Link the player to the team
                        if (dba.AddPlayerToTeam(userID, invite.team.ID)) {

                            // Remove the invite entry from the database
                            if (dba.RemoveInvite(inviteID)) {
                                result = "You've been added to " + invite.team.name + " successfully.";

                                // Indicate the accept went through but the request wasn't removed
                            }
                            else {
                                result = "You've been added to " + invite.team.name + " but the invite wasn't removed.";
                            }

                            // If the link failed set an appropriate message
                        }
                        else {
                            result = "An error occured adding you to " + invite.team.name + ".";
                        }
                    }
                    else {
                        result = "Invalid attempt to accept an invite.";
                        dba.LogInvalidRequest(User.Identity.Name, "Attempt to accept invite (ID " + invite.ID + ").");
                    }
                }
            }

            // Return the success message of the accept
            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Sends an invite email to the given email with the given message.
        /// </summary>
        /// <param name="inviteEmail">The email of the person to invite.</param>
        /// <param name="inviteMessage">The message to send with the invitation.</param>
        /// <returns>Success of the call.</returns>
        public ActionResult AJAX_InviteUser(string inviteEmail, string inviteMessage, long teamID) {
            string successMessage = "Message sent to " + inviteEmail;

            // Make sure the request is authenticated
            if (Request.IsAuthenticated) {

                // Make sure the invite email is bound
                if (inviteEmail != null && !inviteEmail.Equals("")) {

                    // Validate the request
                    DBAccessor dba = new DBAccessor();
                    Person user = dba.GetPersonInformation(User.Identity.Name);
                    string name = user.firstName + " " + user.lastName;
                    Team team = dba.GetTeamDetails(teamID);

                    if (team.coaches.Contains(user, new PersonComparer())) {

                        try {
                            // Add the invite to the database
                            long inviteID = dba.AddInvite(inviteEmail, user.ID, teamID);

                            // Form an email
                            String body = "";
                            if (inviteMessage != null && !inviteMessage.Equals("")) {
                                body += "See " + name + "'s message below:\n\n" + inviteMessage + "\n\n";
                            }
                            body += "To join the " + team.name + " visit http://dugoutdigits.com/Team/Join?id=" + inviteID + "&email=" + inviteEmail + " and follow the instructions.";
                            MailMessage newMessage = new MailMessage();
                            SmtpClient mailService = new SmtpClient();

                            //set the addresses
                            newMessage.From = new MailAddress(AppConstants.EMAIL_ADMIN);
                            newMessage.To.Add(inviteEmail);

                            //set the content
                            newMessage.Subject = name + " has invited you to join the " + team.name;
                            newMessage.Body = body;

                            //send the message
                            mailService.UseDefaultCredentials = false;
                            mailService.DeliveryMethod = SmtpDeliveryMethod.Network;
                            mailService.Host = AppConstants.EMAIL_SMTP_ADDRESS;
                            mailService.Credentials = new NetworkCredential(AppConstants.EMAIL_SMTP_USERNAME, AppConstants.EMAIL_SMTP_PASSWORD);
                            mailService.Send(newMessage);
                        }
                        catch (Exception) {
                            successMessage = "Error sending email to " + inviteEmail;
                        }
                    }
                    else {
                        successMessage = "Invalid attempt to invite user.";
                        dba.LogInvalidRequest(User.Identity.Name, "Attempt to invite "+inviteEmail+" to join "+team.name+" (ID "+team.ID+").");
                    }
                }
                else {
                    successMessage = "Please enter the email of the person you are trying to invite.";
                }
            }
            else {
                successMessage = "The request was not authenticated.";
            }

            // Return the success message of the addition
            return Json(
                new { message = successMessage },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Removes the authenticated user from the team specified by the given ID.
        /// </summary>
        /// <param name="teamID">ID of the team to remove the player from.</param>
        /// <returns>Success message of the removal.</returns>
        public ActionResult AJAX_LeaveTeam(long teamID) {
            // Make sure the user is authenticated
            string result = "Request not authenticated.";

            if (Request.IsAuthenticated) {
                // Get the person id for the user currently logged in
                DBAccessor dba = new DBAccessor();
                long userID = dba.GetPersonID(User.Identity.Name);

                // Remove the player from the team
                if (dba.RemovePlayerFromTeam(userID, teamID)) {
                    result = "Succesfully removed player from the team.";
                }
                else {
                    result = "Failure to remove player from team.";
                }
            }

            // Return the success message of the removal
            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Returns a list of the players tied to the team with the given team ID in the form 
        /// of an unordered list.
        /// </summary>
        /// <param name="teamID">The ID of the team in interest.</param>
        /// <returns>An unordered list of the players on the team.</returns>
        public ActionResult AJAX_GetTeamMembers(long teamID) {
            string result = "Request is not authenticated.";
            if (Request.IsAuthenticated) {
                DBAccessor dba = new DBAccessor();
                Team team = dba.GetTeamDetails(teamID);
                Person user = new Person();
                user.email = User.Identity.Name;

                if (team.coaches.Contains(user, new PersonComparer()) || team.players.Contains(user, new PersonComparer())) {
                    result = "<ul>";
                    foreach (Person player in team.players) {
                        result += "<li>" + player.firstName + " " + player.lastName + "</li>";
                    }
                    result += "</ul>";

                }
                else {
                    result = "You must be on the team or a coach of the team to view the players.";
                    dba.LogInvalidRequest(User.Identity.Name, "Attempt to view players of " + team.name + " (" + team.ID + ").");
                }
            }

            // Return the success message of the removal
            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Returns a list of the coaches tied to the team with the given team ID in the form 
        /// of an unordered list.
        /// </summary>
        /// <param name="teamID">The ID of the team in interest.</param>
        /// <returns>An unordered list of the coaches of the team.</returns>
        public ActionResult AJAX_GetTeamCoaches(long teamID) {
            string result = "Request is not authenticated.";
            if (Request.IsAuthenticated) {
                DBAccessor dba = new DBAccessor();
                Team team = dba.GetTeamDetails(teamID);
                Person user = new Person();
                user.email = User.Identity.Name;

                if (team.coaches.Contains(user, new PersonComparer()) || team.players.Contains(user, new PersonComparer())) {
                    result = "<ul>";
                    foreach (Person coach in team.coaches) {
                        result += "<li>" + coach.firstName + " " + coach.lastName + "</li>";
                    }
                    result += "</ul>";

                }
                else {
                    result = "You must be on the team or a coach of the team to view the coaches.";
                    dba.LogInvalidRequest(User.Identity.Name, "Attempt to view coaches of " + team.name);
                }
            }

            // Return the success message of the removal
            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Renders the HTML for an add season form.
        /// </summary>
        /// <param name="teamID">The ID of the team to which the season would be added.</param>
        /// <returns>The HTML for the form.</returns>
        public ActionResult AJAX_RenderAddSeasonForm(long teamID) {
            string result = "Request is not authenticated.";
            if (Request.IsAuthenticated) {
                DBAccessor dba = new DBAccessor();
                List<Season> seasons = dba.GetSeasons(teamID);

                result = "<form><select class='editor-field' name='yearpicker' id='yearpicker'>";
                for (int i = 0; i < 5; i++) {
                    Season tempSeason = new Season();
                    tempSeason.year = (short)(DateTime.Now.Year + i);
                    if (!seasons.Contains(tempSeason, new SeasonComparer())) {
                        result += "<option value='" + tempSeason.year + "'>" + tempSeason.year + "</option>";
                    }
                }
                result += "</select>";

                result += "<div class='submit-button' onClick='action_addseason()'>Add Season</div>";
                result += "<div id='add-season-feedback'></div></form>";
            }

            // Return the success message of the removal
            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Get's the seasons tied to the team with the given ID in the form of an 
        /// unordered list.
        /// </summary>
        /// <param name="teamID">The ID of the team in interest.</param>
        /// <returns>An unordered list of the seasons.</returns>
        public ActionResult AJAX_GetSeasons(long teamID) {
            string result = "Request is not authenticated.";
            if (Request.IsAuthenticated) {
                DBAccessor dba = new DBAccessor();
                Team team = dba.GetTeamDetails(teamID);
                Person user = new Person();
                user.email = User.Identity.Name;

                if (team.coaches.Contains(user, new PersonComparer()) || team.players.Contains(user, new PersonComparer())) {
                    List<Season> seasons = dba.GetSeasons(teamID);
                    if (seasons.Any()) {
                        result = "<ul>";
                        foreach (Season season in seasons) {
                            result += "<li>" + season.year + "</li>";
                        }
                        result += "</ul>";
                    }
                    else {
                        result = "<p>There are currently no seasons for this team.</p>";
                    }
                }
                else {
                    result = "You must be on the team or a coach of the team to view the seasons.";
                    dba.LogInvalidRequest(User.Identity.Name, "Attempt to view seasons of " + team.name + " ("+team.ID+").");
                }
            }

            // Return the success message of the removal
            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Adds the given season to the team with the matching team ID.
        /// </summary>
        /// <param name="teamID">The ID of the team of interest.</param>
        /// <param name="season">The season to be added to the team of interest.</param>
        /// <returns>A message detailing the result of the addition.</returns>
        public ActionResult AJAX_AddSeason(long teamID, short season) {
            string result = "Request is not authenticated.";
            if (Request.IsAuthenticated) {
                DBAccessor dba = new DBAccessor();
                Team team = dba.GetTeamDetails(teamID);
                Person user = new Person();
                user.email = User.Identity.Name;

                if (team.coaches.Contains(user, new PersonComparer())) {
                    result = "Error adding season " + season + " to " + team.name + ".";
                    if (dba.AddSeason(teamID, season)) {
                        result = "Season " + season + " added to " + team.name + ".";
                    }
                } else {
                    result = "You must be a coach of the team to add a season.";
                    dba.LogInvalidRequest(User.Identity.Name, "Attempt to add a season to "+team.name + " (" + team.ID + ").");
                }
            }

            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Renders the HTML for an add game form.
        /// </summary>
        /// <param name="teamID">The ID of the team of interest.</param>
        /// <returns>The HTML for the form.</returns>
        public ActionResult AJAX_RenderAddGameForm(long teamID) {
            string result = "Request is not authenticated.";
            if (Request.IsAuthenticated) {
                DBAccessor dba = new DBAccessor();
                List<Season> seasons = dba.GetSeasons(teamID);

                result = "<form>";
                result += "<h4>Opponent</h4>";
                result += "<input class='editor-field' type='text' name='add-game-opponent' />\n";
                result += "<h4>Home or Away</h4>";
                result += "<input type='radio' name='add-game-homeaway' value='Home'>Home";
                result += "<input type='radio' name='add-game-homeaway' value='Away'>Away<br/>";
                result += "<h4>Location</h4>";
                result += "<input class='editor-field' type='text' name='add-game-location' />\n";
                result += "<div style='width: 200px;'><div class='leftColB'>";
                result += "<h4>Date</h4>";
                result += "<input id='add-game-datepicker' class='editor-field' type='text' name='add-game-date' readonly='readonly' style='width: 90px;' />\n";
                result += "</div><div class='rightColB'>";
                result += "<h4 style='margin-left: 10px;'>Time</h4>";
                result += "<input id='add-game-timepicker' class='editor-field' type='text' name='add-game-time' readonly='readonly' style='width: 90px; margin-left: 10px;' />\n";
                result += "</div><div class='clear'></div></div>";
                result += "<h4>Season</h4>";
                result += "<select class='editor-field' name='add-game-season' id='add-game-season'>";
                foreach (Season season in seasons) {
                    if (season.year == DateTime.Now.Year) {
                        result += "<option value='" + season.ID + "' selected='selected'>" + season.year + "</option>";
                    }
                    else {
                        result += "<option value='" + season.ID + "'>" + season.year + "</option>";
                    }
                }
                result += "</select>";
                result += "<div class='submit-button' onClick='action_addgame()'>Add Game</div>";
                result += "<div id='add-game-feedback'></div></form>";
                result += "<script>$('#add-game-datepicker').datepicker({ dateFormat: 'm/dd' });</script>";
                result += "<script>$('#add-game-timepicker').timepicker({ timeFormat: 'h:mm TT' });</script>";
                result += "<script>isHome = \"Home\"; $('input:radio[name=\"add-game-homeaway\"]').click(function(){ isHome = $(this).val() });</script>";
            }

            // Return the success message of the removal
            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Creates a game object with the given information, ties it to the team with the given 
        /// team ID and saved the game to the database.
        /// </summary>
        /// <param name="teamID">The ID of the team of interest.</param>
        /// <param name="opponent">The name of the opposing team.</param>
        /// <param name="homeOrAway">If the game is at home or away (string: "Home" or "Away").</param>
        /// <param name="location">The location of the game.</param>
        /// <param name="date">The date of the game (M/D).</param>
        /// <param name="time">The time of the game (H/MM TT).</param>
        /// <param name="seasonID">The ID of the season it is being added to.</param>
        /// <returns>A message detailing the result of the addition.</returns>
        public ActionResult AJAX_AddGame(long teamID, string opponent, string homeOrAway, string location, string date, string time, long seasonID) {
            string result = "Request is not authenticated.";
            if (Request.IsAuthenticated) {
                DBAccessor dba = new DBAccessor();
                Team team = dba.GetTeamDetails(teamID);
                Person user = new Person();
                user.email = User.Identity.Name;
                Season season = dba.GetSeason(seasonID);

                if (team.coaches.Contains(user, new PersonComparer())) {
                    try {
                        DateTime gameDate = Parser.ParseDateAndTime(date, time, season.year);
                        Game game = new Game();
                        game.isHome = homeOrAway.Equals("Home");
                        game.location = location;
                        game.opponent = opponent;
                        game.season = season;
                        game.date = gameDate;

                        if (dba.AddGame(game)) {
                            result = "Game sucessfully added to the season.";
                        }
                        else {
                            result = "Error adding the game to the season.";
                        }
                    }
                    catch {
                        result = "An invalid date was given.";
                    }
                }
                else {
                    result = "You must be a coach of the team to add a game.";
                    dba.LogInvalidRequest(User.Identity.Name, "Attempt to add a game to " + team.name + " (" + team.ID + ").");
                }
            }

            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Renders the HTML for an add practice form.
        /// </summary>
        /// <param name="teamID"></param>
        /// <returns></returns>
        public ActionResult AJAX_RenderAddPracticeForm(long teamID) {
            string result = "Request is not authenticated.";
            if (Request.IsAuthenticated) {
                DBAccessor dba = new DBAccessor();
                List<Season> seasons = dba.GetSeasons(teamID);

                result = "<form>";
                result += "<h4>Location</h4>";
                result += "<input class='editor-field' type='text' name='add-practice-location' />\n";
                result += "<div style='width: 200px;'><div class='leftColB'>";
                result += "<h4>Date</h4>";
                result += "<input id='add-practice-datepicker' class='editor-field' type='text' name='add-practice-date' readonly='readonly' style='width: 90px;' />\n";
                result += "</div><div class='rightColB'>";
                result += "<h4 style='margin-left: 10px;'>Time</h4>";
                result += "<input id='add-practice-timepicker' class='editor-field' type='text' name='add-practice-time' readonly='readonly' style='width: 90px; margin-left: 10px;' />\n";
                result += "</div><div class='clear'></div></div>";
                result += "<h4>Season</h4>";
                result += "<select class='editor-field' name='add-practice-season' id='add-practice-season'>";
                foreach (Season season in seasons) {
                    if (season.year == DateTime.Now.Year) {
                        result += "<option value='" + season.ID + "' selected='selected'>" + season.year + "</option>";
                    }
                    else {
                        result += "<option value='" + season.ID + "'>" + season.year + "</option>";
                    }
                }
                result += "</select>";
                result += "<div class='submit-button' onClick='action_addpractice()'>Add Practice</div>";
                result += "<div id='add-practice-feedback'></div></form>";
                result += "<script>$('#add-practice-datepicker').datepicker({ dateFormat: 'm/dd' });</script>";
                result += "<script>$('#add-practice-timepicker').timepicker({ timeFormat: 'h:mm TT' });</script>";
            }

            // Return the success message of the removal
            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        /// <summary>
        /// Creates a practice object with the given information, ties it to the team with 
        /// the given team ID and saved the game to the database.
        /// </summary>
        /// <param name="teamID">The ID of the team of interest.</param>
        /// <param name="location">The location of the practice.</param>
        /// <param name="date">The date of the practice (M/D).</param>
        /// <param name="time">The time of the practice (H/MM TT).</param>
        /// <param name="seasonID">The ID of the season is is being added to.</param>
        /// <returns>A message detailing the result of the addition.</returns>
        public ActionResult AJAX_AddPractice(long teamID, string location, string date, string time, long seasonID) {
            string result = "Request is not authenticated.";
            if (Request.IsAuthenticated) {
                DBAccessor dba = new DBAccessor();
                Team team = dba.GetTeamDetails(teamID);
                Person user = new Person();
                user.email = User.Identity.Name;
                Season season = dba.GetSeason(seasonID);

                if (team.coaches.Contains(user, new PersonComparer())) {
                    try {
                        DateTime practiceDate = Parser.ParseDateAndTime(date, time, season.year);
                        Practice practice = new Practice();
                        practice.location = location;
                        practice.season = season;
                        practice.date = practiceDate;

                        if (dba.AddPractice(practice)) {
                            result = "Practice sucessfully added to the season.";
                        }
                        else {
                            result = "Error adding the practice to the season.";
                        }
                    }
                    catch {
                        result = "An invalid date was given.";
                    }
                }
                else {
                    result = "You must be a coach of the team to add a practice.";
                    dba.LogInvalidRequest(User.Identity.Name, "Attempt to add a practice to " + team.name + " (" + team.ID + ").");
                }
            }

            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }
    }
}
