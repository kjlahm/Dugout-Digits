using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using DugoutDigits.Objects;
using DugoutDigits.Utilities;

namespace DugoutDigits.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            if (Request.IsAuthenticated) {
                DBAccessor dba = new DBAccessor();
                Person user = dba.GetPersonInformation(User.Identity.Name);

                if (user.permissions.siteAdmin) {
                    ViewBag.SiteAdmin = true;
                    return View();
                }
                else {
                    return RedirectToAction("Index", "Home");
                }
            }

            return RedirectToAction("LogOn", "Account");
        }

        public ActionResult GetCoachRequests() {
            DBAccessor dba = new DBAccessor();
            List<Request> requests = dba.GetCoachRequests();

            String result = "";
            if (requests.Count > 0) {
                result = "<table>";
                result += "<tr><th>Requestee</th><th>Email</th><th>Action</th></tr>";
                foreach (Request request in requests) {
                    string name = request.requestee.firstName + " " + request.requestee.lastName;
                    result += "<tr><td>" + name + "</td><td>" + request.requestee.email + "</td>";
                    result += "<td><img src='./../Content/images/accept.png' height='20' width='20' class='request-action-image' alt='accept' onClick='action_acceptcoachrequest(" + request.ID + ")' />";
                    result += "<img src='./../Content/images/decline.png' height='20' width='20' class='request-action-image' margin-right='5px' alt='decline' onClick='action_declinecoachrequest(" + request.ID + ")' /></td></tr>";
                }
                result += "</table>";
            }
            else {
                result = "There are no pending coach permission requests.";
            }

            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        public ActionResult AddCoachRequest() {
            String result = "Coach permission has been requested.";
            DBAccessor dba = new DBAccessor();

            // Add request to the DB
            string email = User.Identity.Name;
            Person requestee = dba.GetPersonInformation(email);
            if (requestee != null) {
                if (dba.AddNewRequest(requestee.ID, RequestType.COACH_PERMISSION)) {
                    // Send email that a request was added
                    try {
                        // Form an email
                        MailMessage newMessage = new MailMessage();
                        SmtpClient mailService = new SmtpClient();

                        //set the addresses
                        newMessage.From = new MailAddress(AppConstants.EMAIL_ADMIN);
                        newMessage.To.Add(AppConstants.EMAIL_ADMIN);

                        //set the content
                        newMessage.Subject = "Coach Permission Requested";
                        newMessage.Body = requestee.firstName + " " + requestee.lastName + " has requested coach access (email: " + requestee.email + ").";

                        //send the message
                        mailService.UseDefaultCredentials = false;
                        mailService.DeliveryMethod = SmtpDeliveryMethod.Network;
                        mailService.Host = AppConstants.EMAIL_SMTP_ADDRESS;
                        mailService.Credentials = new NetworkCredential(AppConstants.EMAIL_SMTP_USERNAME, AppConstants.EMAIL_SMTP_PASSWORD);
                        mailService.Send(newMessage);
                    }
                    catch (Exception) {
                        result = "Error notifying the site administrator.";
                    }
                }
                else {
                    result = "Couldn't add a request to the database.";
                }
            }
            else {
                result = "Couldn't find the user in the database.";
            }

            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        public ActionResult AcceptCoachRequest(long requestID) {
            String result = "Coach permission added.";
            DBAccessor dba = new DBAccessor();
            Request request = dba.GetRequest(requestID, RequestType.COACH_PERMISSION);

            if (request != null) {
                if (dba.AddCoachPermission(request.requestee.ID)) {
                    // Form an email
                    MailMessage newMessage = new MailMessage();
                    SmtpClient mailService = new SmtpClient();

                    //set the addresses
                    newMessage.From = new MailAddress(AppConstants.EMAIL_ADMIN);
                    newMessage.To.Add(request.requestee.email);

                    //set the content
                    newMessage.Subject = "Coach Permission Approved";
                    newMessage.Body = "Your request to receive coach permission at dugoutdigits.com has been approved.";

                    //send the message
                    mailService.UseDefaultCredentials = false;
                    mailService.DeliveryMethod = SmtpDeliveryMethod.Network;
                    mailService.Host = AppConstants.EMAIL_SMTP_ADDRESS;
                    mailService.Credentials = new NetworkCredential(AppConstants.EMAIL_SMTP_USERNAME, AppConstants.EMAIL_SMTP_PASSWORD);
                    mailService.Send(newMessage);

                    // Remove the request from the DB
                    if (!dba.RemoveRequest(request.ID)) {
                        result = "Error removing the coach request from the database (ID=" + request.ID + ").";
                    }
                }
                else {
                    result = "Error adding coach permission.";
                }
            }
            else {
                result = "The request couldn't be found in the database.";
            }

            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        public ActionResult DeclineCoachRequest(long requestID) {
            String result = "Request deleted.";
            DBAccessor dba = new DBAccessor();

            // Get the request from the DB
            Request request = dba.GetRequest(requestID, RequestType.COACH_PERMISSION);

            if (!dba.RemoveRequest(requestID)) {
                result = "Error deleting request.";
            }
            else {
                // Form an email
                MailMessage newMessage = new MailMessage();
                SmtpClient mailService = new SmtpClient();

                //set the addresses
                newMessage.From = new MailAddress(AppConstants.EMAIL_ADMIN);
                newMessage.To.Add(request.requestee.email);

                //set the content
                newMessage.Subject = "Coach Permission Declined";
                newMessage.Body = "Your request to receive coach permission at dugoutdigits.com has been declined.";

                //send the message
                mailService.UseDefaultCredentials = false;
                mailService.DeliveryMethod = SmtpDeliveryMethod.Network;
                mailService.Host = AppConstants.EMAIL_SMTP_ADDRESS;
                mailService.Credentials = new NetworkCredential(AppConstants.EMAIL_SMTP_USERNAME, AppConstants.EMAIL_SMTP_PASSWORD);
                mailService.Send(newMessage);
            }

            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }

        public ActionResult GetInvalidRequests() {
            DBAccessor dba = new DBAccessor();
            List<LogEntry> messages = dba.GetLogMessages(LogType.INVALID_REQUEST);

            String result = "";
            if (messages != null) {
                if (messages.Count > 0) {
                    result = "<table>";
                    result += "<tr><th>User</th><th>Message</th><th>Timestamp</th></tr>";
                    foreach (LogEntry message in messages) {
                        string name = message.User.firstName + " " + message.User.lastName;
                        result += "<tr><td>" + name + "</td><td>" + message.Message + "</td><td>" + message.Timestamp + "</td></tr>";
                    }
                    result += "</table>";
                }
                else {
                    result = "There are no invalid requests logged at the moment.";
                }
            }
            else {
                result = "An error occured getting invalid requests.";
            }

            return Json(
                new { message = result },
                JsonRequestBehavior.AllowGet
            );
        }
    }
}
