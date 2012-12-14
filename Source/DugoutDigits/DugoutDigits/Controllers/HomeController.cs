using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DugoutDigits.Utilities;
using DugoutDigits.Objects;
using System.Web.Security;
using DugoutDigits.Models;
using System.Net.Mail;
using System.Net;

namespace DugoutDigits.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Request.IsAuthenticated) {
                DBAccessor dba = new DBAccessor();
                Person user = dba.GetPersonInformation(User.Identity.Name);

                ViewBag.FirstName = user.firstName;
                ViewBag.LastName = user.lastName;
                ViewBag.ImageURL = user.imageURL;
                ViewBag.CoachPermission = user.permissions.coachEnabled;
                //List<string> teams = dba.GetTeamListCoach(User.Identity.Name);
                //ViewBag.TeamNames = teams;
                //ViewBag.TeamCount = teams.Count;
            }
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult AccountSummary() {
            // Get the user information from the database
            DBAccessor dba = new DBAccessor();
            Person user = dba.GetPersonInformation(User.Identity.Name);

            String summaryMessage = "<h3>Account Summary</h3>";
            summaryMessage += "<p>" + user.firstName + " " + user.lastName + "</p>";
            summaryMessage += "<img src='" + user.imageURL + "' alt='profile image' />";
            summaryMessage += "</div>";

            List<Team> teamNames = dba.GetTeamListCoach(User.Identity.Name);
            summaryMessage += "<div id='sidebar-teamlist'><ul>";
            foreach (Team team in teamNames) {
                summaryMessage += "<li>" + team.name + "</li>";
            }
            summaryMessage += "</ul></div>";


            return Json(
                new { message = summaryMessage },
                JsonRequestBehavior.AllowGet
            );
        }

        public ActionResult TeamList() {
            DBAccessor dba = new DBAccessor();
            List<Team> teamNames = dba.GetTeamListCoach(User.Identity.Name);
            
            String summaryMessage = "<ul>";
            foreach (Team team in teamNames) {
                summaryMessage += "<li>" + team.name + "</li>";
            }
            summaryMessage += "</ul>";

            return Json(
                new { message = summaryMessage },
                JsonRequestBehavior.AllowGet
            );
        }

        public ActionResult TestAJAX() {
            // TODO: do something with the arguments

            return Json(
                new { message = "Thanks for sending info" },
                JsonRequestBehavior.AllowGet
            );
        }

        public ActionResult AJAX_SubmitFeedback(string message) {
            string successMessage = "Thanks for your feedback.";

            if (Request.IsAuthenticated) {
                if (message != null && !message.Equals("")) {
                    try {
                        // Form an email
                        MailMessage newMessage = new MailMessage();
                        SmtpClient mailService = new SmtpClient();

                        //set the addresses
                        newMessage.From = new MailAddress(User.Identity.Name);
                        newMessage.To.Add(AppConstants.EMAIL_FEEDBACK);

                        //set the content
                        newMessage.Subject = "Dugout Digits Site Feedback";
                        newMessage.Body = message;

                        //send the message
                        mailService.UseDefaultCredentials = false;
                        mailService.DeliveryMethod = SmtpDeliveryMethod.Network;
                        mailService.Host = AppConstants.EMAIL_SMTP_ADDRESS;
                        mailService.Credentials = new NetworkCredential(AppConstants.EMAIL_SMTP_USERNAME, AppConstants.EMAIL_SMTP_PASSWORD);
                        mailService.Send(newMessage);
                    }
                    catch (Exception) {
                        successMessage = "Error submitting feedback.";
                    }
                }
                else {
                    successMessage = "The given message is null or empty.";
                }
            }
            else {
                successMessage = "Request not authenticated.";
            }

            // Return the success message of the addition
            return Json(
                new { message = successMessage },
                JsonRequestBehavior.AllowGet
            );
        }
    }
}
