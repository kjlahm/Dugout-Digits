using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DugoutDigits.Utilities;
using DugoutDigits.Objects;
using System.Net.Mail;
using System.Net;

namespace DugoutDigits.Controllers
{
    public class TeamController : Controller
    {
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
            string result = "<p>Could not authenticate the request.</p>\n";
            if (Request.IsAuthenticated) {
                result = "<h3>My Teams</h3>";

                // Get the teams associated with the user who's logged in
                DBAccessor dba = new DBAccessor();
                List<Team> teamsCoached = dba.GetTeamListCoach(User.Identity.Name);
                List<Team> teamsMember = dba.GetTeamListMember(User.Identity.Name);

                // Create table to hold the team names
                if (teamsCoached.Any() || teamsMember.Any()) {
                    result += "<table>\n";
                    result += "<tr><th>Team Name</th><th>Coach's Name</th></tr>\n";

                    foreach (Team team in teamsCoached) {
                        result += "<tr><td>" + team.name + "</td><td>You</td></tr>\n";
                    }

                    foreach (Team team in teamsMember) {
                        result += "<tr><td>" + team.name + "</td><td>" + team.coach.firstName + " " + team.coach.lastName + "</td></tr>\n";
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
                    result = "<h3>Pending Requests</h3>\n";

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
                            result += "<div onClick='action_detailsrequest(" + request.ID + ")' class='request-action-text'>Details</div></td></tr>";
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
        /// Returns a list of the pending invites associated with the authenticated user.
        /// </summary>
        /// <returns></returns>
        public ActionResult AJAX_GetInviteTable() {
            string result = "<p>Could not authenticate the request.</p>\n";
            if (Request.IsAuthenticated) {

                DBAccessor dba = new DBAccessor();
                Person person = dba.GetPersonInformation(User.Identity.Name);

                result = "<h3>Pending Invites</h3>\n";

                // Call the database to get pending requests
                List<Invitation> invitations = dba.GetInvitations(User.Identity.Name);

                // Form an HTML table with the retrieved information
                if (invitations.Any()) {
                    result += "<table>\n";
                    result += "<tr><th>Invitation From</th><th>Invitation to Join</th><th>Action</th></tr>\n";
                    foreach (Invitation invitation in invitations) {
                        string playerName = invitation.team.coach.firstName + " " + invitation.team.coach.lastName;
                        result += "<tr><td>" + playerName + "</td><td>" + invitation.team.name + "</td>";
                        result += "<td><img src='./../Content/images/accept.png' height='20' width='20' class='request-action-image' alt='accept' onClick='action_acceptinvite(" + invitation.ID + ")' />";
                        result += "<img src='./../Content/images/decline.png' height='20' width='20' class='request-action-image' margin-right='5px' alt='decline' onClick='action_declineinvite(" + invitation.ID + ")' />";
                        result += "<div onClick='action_detailsinvite(" + invitation.ID + ")' class='request-action-text'>Details</div></td></tr>";
                    }
                    result += "</table>\n";
                }
                else {
                    result += "<p>There are no pending Invitations at this time.</p>\n";
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
                result = "<div class='lightbox-content-close' onclick='action_hidedetails()'>Close</div>";
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
                result = "<div class='lightbox-content-close' onclick='action_hidedetails()'>Close</div>";
                result += "<h3>Pending Invite</h3>\n";
                result += "<div id='invite-details-left'>\n<img src='" + invite.team.coach.imageURL + "' alt='coach picture' />\n</div>\n";
                result += "<div id='invite-details-right'>\n";
                result += "<p>Invited by " + invite.team.coach.firstName + " " + invite.team.coach.lastName + "</p>\n";
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
                            // Get the players of the team
                            List<Person> teamMembers = dba.GetTeamPlayers(team.ID);
                            Person user = new Person();
                            user.email = User.Identity.Name;

                            string coachName = team.coach.firstName + " " + team.coach.lastName;
                            string joinCell = "<div id='RQ_" + team.ID + "' onClick='action_requestjoin(" + team.ID + ")'>Send Request</div>";
                            if (team.coach.email.Equals(User.Identity.Name)) {
                                coachName = "You";
                                joinCell = "N/A";
                            } else if (teamMembers.Contains(user, new PersonComparer())) {
                                joinCell = "You're on the Team";
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

        /// <summary>
        /// Removes the invite matching the given invite ID from the database.
        /// </summary>
        /// <param name="inviteID">The ID of the invite to remove.</param>
        /// <returns>Success message of the invite removal.</returns>
        public ActionResult AJAX_RemoveInvite(long inviteID) {
            // Make sure the user is authenticated
            string result = "Request not authenticated.";

            if (Request.IsAuthenticated) {
                // Remove the request to the database
                DBAccessor dba = new DBAccessor();
                result = "Error making the request.";
                if (dba.RemoveInvite(User.Identity.Name, inviteID)) {
                    result = "Invitation removed.";
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
                    // Link the player to the team
                    if (dba.AddPlayerToTeam(userID, invite.team.ID)) {

                        // Remove the invite entry from the database
                        if (dba.RemoveInvite(User.Identity.Name, inviteID)) {
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

            try {
                // Get the name of the authenticated user
                DBAccessor dba = new DBAccessor();
                Person user = dba.GetPersonInformation(User.Identity.Name);
                string name = user.firstName + " " + user.lastName;
                Team team = dba.GetTeamDetails(teamID);

                // Add the invite to the database
                long inviteID = dba.AddInvite(inviteEmail, user.ID, teamID);

                // Form an email
                String body = "See " + name + "'s message below:\n\n" + inviteMessage;
                body += "\n\nTo join the " + team.name + " visit http://dugoutdigits.com/Team/Join?id=" + inviteID + "&email=" + inviteEmail + " and follow the instructions.";
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

            // Return the success message of the addition
            return Json(
                new { message = successMessage },
                JsonRequestBehavior.AllowGet
            );
        }
    }
}
