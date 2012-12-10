using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using DugoutDigits.Models;
using DugoutDigits.Objects;
using DugoutDigits.Utilities;

namespace DugoutDigits.Controllers {
    public class AccountController : Controller {

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
                string name = dba.GetUserName(User.Identity.Name);

                // Create an SMTP client to send the email
                SmtpClient client = new SmtpClient();
                client.Host = "localhost";

                // Form an email
                MailAddress to = new MailAddress(inviteEmail);
                MailAddress from = new MailAddress(AppConstants.EMAIL_ADMIN);
                MailMessage message = new MailMessage(from, to);
                String body = "See " + name + "'s message below:\n\n" + inviteMessage + "\n\nTo join Dugout Digits visit http://dugoutdigits.com/Account/Register";
                //body += "\n\n";
                message.Body = body;
                message.Subject = name + " has invited you to join Dugout Digits";

                /*MailMessage msg = new MailMessage(AppConstants.EMAIL_ADMIN, inviteEmail);
                msg.Subject = name + " has invited you to join Dugout Digits";
                msg.Body = "See " + name + "'s message below:\n\n" + inviteMessage + "\n\nTo join Dugout Digits visit http://dugoutdigits.com";*/

                // Send the message
                client.Send(message);
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

        //
        // GET: /Account/LogOn
        public ActionResult LogOn() {
            return View();
        }

        //
        // POST: /Account/LogOn
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl) {
            if (ModelState.IsValid) {

                // This should be a DB check instead of Membership.ValidateUser
                //if (Membership.ValidateUser(model.Email, model.Password)) {
                DBAccessor dba = new DBAccessor();
                String result = dba.CheckLoginCredentials(model.Email, model.Password);

                String[] resultSplit = result.Split('|');

                if (resultSplit[0].Equals("success")) {

                    FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
                    HttpCookie cookie = new HttpCookie("DD_name", resultSplit[1]);
                    cookie.Expires = DateTime.Now.AddDays(1000);
                    this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
                    
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\")) {
                        return Redirect(returnUrl);
                    }
                    else {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else {
                    ModelState.AddModelError("", resultSplit[0]);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff
        public ActionResult LogOff() {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register
        public ActionResult Register() {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        public ActionResult Register(RegisterModel model) {
            if (ModelState.IsValid) {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.Email, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    // Add the user to the MySQL DB
                    Person newUser = new Person(model.FirstName, model.LastName, model.Email, model.Password);
                    DBAccessor dba = new DBAccessor();
                    dba.AddNewUser(newUser);

                    // Set the appropriate cookies
                    FormsAuthentication.SetAuthCookie(model.Email, false /* createPersistentCookie */);
                    HttpCookie cookie = new HttpCookie("DD_name", model.FirstName+" "+model.LastName);
                    cookie.Expires = DateTime.Now.AddDays(1000);
                    this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Edit
        public ActionResult Edit() {
            String userEmail = User.Identity.Name;

            DBAccessor dba = new DBAccessor();
            Person user = dba.GetPersonInformation(userEmail);
            EditModel model = new EditModel();

            if (user != null) {
                model.FirstName = user.firstName;
                model.LastName = user.lastName;
                model.Email = user.email;
                model.Birthday = user.birthday;
                model.Height = Convert.ToInt16(user.height);
                model.Weight = Convert.ToInt16(user.weight);
                model.ImageURL = user.imageURL;
            }
            else {
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        //
        // POST: /Account/Edit
        [HttpPost]
        public ActionResult Edit(EditModel model) {
            if (ModelState.IsValid) {
                // Update the user in the MySQL DB
                String oldEmail = User.Identity.Name;
                DBAccessor dba = new DBAccessor();
                String result = dba.CheckLoginCredentials(oldEmail, model.Password);
                String[] resultSplit = result.Split('|');

                if (resultSplit[0].Equals("success")) {

                    Person updateUser = new Person(model.FirstName, model.LastName, model.Email, model.ImageURL, "", model.Birthday, model.Height, model.Weight);
                    dba.UpdateUserInformation(oldEmail, updateUser);

                    // Set the appropriate cookies
                    FormsAuthentication.SetAuthCookie(model.Email, false /* createPersistentCookie */);
                    HttpCookie cookie = new HttpCookie("DD_name", model.FirstName + " " + model.LastName);
                    cookie.Expires = DateTime.Now.AddDays(1000);
                    this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
                }
                else {
                    ModelState.AddModelError("", resultSplit[0]);
                }
            }

            return View(model);
        }

        //
        // GET: /Account/ChangePassword
        [Authorize]
        public ActionResult ChangePassword() {
            return View();
        }

        //
        // POST: /Account/ChangePassword
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model) {
            if (ModelState.IsValid) {
                String email = User.Identity.Name;
                DBAccessor dba = new DBAccessor();
                String result = dba.CheckLoginCredentials(email, model.OldPassword);
                String[] resultSplit = result.Split('|');

                if (resultSplit[0].Equals("success")) {
                    if (dba.UpdateUserPassword(email, model.NewPassword)) {
                        return RedirectToAction("ChangePasswordSuccess");
                    }
                    else {
                        ModelState.AddModelError("", "Password update failed, try again.");
                    }
                }
                else {
                    ModelState.AddModelError("", resultSplit[0]);
                }
            }
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess
        public ActionResult ChangePasswordSuccess() {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
