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
                            result += "<tr><td>" + playerName + "</td><td>" + request.team.name + "</td><td><div onClick='action_acceptrequest(" + request.ID + ")'>Accept</div><div onClick='action_declinerequest(" + request.ID + ")'>Decline</div></td></tr>";
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
                newMessage.Subject = name + " has invited you to join Dugout Digits";
                newMessage.Body = body;

                //send the message
                mailService.Port = 587;
                mailService.EnableSsl = true;

                mailService.UseDefaultCredentials = false;
                mailService.DeliveryMethod = SmtpDeliveryMethod.Network;

                mailService.Host = "smtp.gmail.com";

                //to change the port (default is 25), we set the port property
                mailService.Credentials = new NetworkCredential("kjlahm@gmail.com", AppConstants.EMAIL_PASS);
                mailService.Send(newMessage);
            }
            catch (Exception ex) {
                successMessage = ex.Message;
                //successMessage = "Error sending email to " + inviteEmail;
            }

            // Return the success message of the addition
            return Json(
                new { message = successMessage },
                JsonRequestBehavior.AllowGet
            );
        }
    }
}
